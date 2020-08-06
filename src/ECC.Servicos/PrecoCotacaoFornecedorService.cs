using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeAvisos;
using ECC.EntidadeComum;
using ECC.EntidadeCotacao;
using ECC.EntidadeEmail;
using ECC.EntidadeParametroSistema;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.Entidades.EntidadeComum;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeStatus;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using ECC.Servicos.ModelService;


namespace ECC.Servicos
{
    public class PrecoCotacaoFornecedorService : IPrecoCotacaoFornecedorService
    {

        #region Variáveis
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<FornecedorProduto> _fornecedorProdutoRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedoRep;
        private readonly IEntidadeBaseRep<Cotacao> _cotacaoRep;
        private readonly IEntidadeBaseRep<ResultadoCotacao> _resultadoCotacao;
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedor;
        private readonly IEntidadeBaseRep<IndisponibilidadeProduto> _indisponibilidadeProdutoRep;
        private readonly IEntidadeBaseRep<CotacaoPedidos> _cotacaoPedidos;
        private readonly IEntidadeBaseRep<RemoveFornPedido> _removeFornPedidoRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmail;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Propriedades

        public TimeSpan HoraCotacao { get; set; }

        #endregion

        #region Contrutor

        public PrecoCotacaoFornecedorService() : this(new DbFactory()) { }

        public PrecoCotacaoFornecedorService(DbFactory dbFactory)
            : this(
                new EntidadeBaseRep<Usuario>(dbFactory),
                new EntidadeBaseRep<Pedido>(dbFactory),
                new EntidadeBaseRep<FornecedorProduto>(dbFactory),
                new EntidadeBaseRep<Fornecedor>(dbFactory),
                new EntidadeBaseRep<Cotacao>(dbFactory),
                new EntidadeBaseRep<ResultadoCotacao>(dbFactory),
                new EntidadeBaseRep<MembroFornecedor>(dbFactory),
                new EntidadeBaseRep<IndisponibilidadeProduto>(dbFactory),
                new EntidadeBaseRep<CotacaoPedidos>(dbFactory),
                new EntidadeBaseRep<RemoveFornPedido>(dbFactory),
                new EntidadeBaseRep<TemplateEmail>(dbFactory),
                new EntidadeBaseRep<Emails>(dbFactory),
        new UnitOfWork(dbFactory))
        { }

        public PrecoCotacaoFornecedorService(
                IEntidadeBaseRep<Usuario> usuarioRep,
                IEntidadeBaseRep<Pedido> pedidoRep,
                IEntidadeBaseRep<FornecedorProduto> fornecedorProdutoRep,
                IEntidadeBaseRep<Fornecedor> fornecedorRep,
                IEntidadeBaseRep<Cotacao> cotacaoRep,
                IEntidadeBaseRep<ResultadoCotacao> resultadoCotacaoRep,
                IEntidadeBaseRep<MembroFornecedor> membroFornecedorRep,
                IEntidadeBaseRep<IndisponibilidadeProduto> indisponibilidadeProdutoRep,
                IEntidadeBaseRep<CotacaoPedidos> cotacaoPedidosRep,
                IEntidadeBaseRep<RemoveFornPedido> removeFornPedidoRep,
                IEntidadeBaseRep<TemplateEmail> templateEmail,
                IEntidadeBaseRep<Emails> emailsRep,
        IUnitOfWork unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _pedidoRep = pedidoRep;
            _fornecedorProdutoRep = fornecedorProdutoRep;
            _fornecedoRep = fornecedorRep;
            _cotacaoRep = cotacaoRep;
            _resultadoCotacao = resultadoCotacaoRep;
            _membroFornecedor = membroFornecedorRep;
            _indisponibilidadeProdutoRep = indisponibilidadeProdutoRep;
            _cotacaoPedidos = cotacaoPedidosRep;
            _removeFornPedidoRep = removeFornPedidoRep;
            _templateEmail = templateEmail;
            _emailsRep = emailsRep;
            _unitOfWork = unitOfWork;
        }


        #endregion

        #region Métodos

        public List<CotacaoUsuarios> PrecificarCotacaoFornecedor()
        {
            // Verificar se existe fatura em atraso, para não precificar para este fornecedor
            List<KeyValuePair<int, DateTime>> listCotacaoId = new List<KeyValuePair<int, DateTime>>();
            List<int> categoriasFornecedores = new List<int>();

            var fornecedores = _fornecedorProdutoRep.GetAll()
                .Where(x => x.Fornecedor.Ativo && x.Ativo)
                .Select(f => f.Fornecedor).Distinct().ToList();

            fornecedores.ForEach(f =>
            {
                listCotacaoId.Clear();
                categoriasFornecedores.Clear();

                var membrosForn = _membroFornecedor.GetAll()
               .Where(m => m.FornecedorId == f.Id && m.Ativo)
               .Select(o => o.Membro.Id)
               .ToList();

                var indispProdutoFornecedor = _indisponibilidadeProdutoRep.GetAll()
                                    .Where(i => i.FornecedorId == f.Id
                                          && i.InicioIndisponibilidade <= DateTime.Now
                                          && i.FimIndisponibilidade >= DateTime.Now)
                                    .Select(c => c.Produto.Id).ToList();


                var resultCotacao = _resultadoCotacao.GetAll()
                                    .Where(x => x.Cotacao.DtFechamento >= DateTime.Now && x.FornecedorId == f.Id)
                                    .Select(c=>c.CotacaoId)
                                    .ToList();

                var fornecedorCategorias = f.FornecedorCategorias.Select(x => x.CategoriaId).ToList();


                //pega todos os pedido da tabela remover fornecedor que estaja na cotação = ProdutoId
                var itemPedidos = _cotacaoPedidos.GetAll()
                .Where(x => !resultCotacao.Contains(x.CotacaoId) &&
                            x.Cotacao.DtFechamento >= DateTime.Now)
                    .SelectMany(i => i.Pedido.ItemPedidos
                                .Where(r => !indispProdutoFornecedor.Contains(r.ProdutoId) &&
                                            fornecedorCategorias.Contains(r.Produto.SubCategoria.CategoriaId))
                                .Select(p => p.Id))
                .ToList();

                // Verifica se o fornecedor foi removido para dar preço no item
                var intensPedidoRemoveItemForn = _removeFornPedidoRep
                                   .FindBy(x => x.FonecedorId == f.Id && itemPedidos.Contains(x.ItemPedidoId))
                                   .Select(p => p.ItemPedidoId)
                                   .ToList();

                var cotacaoPedidos = _cotacaoPedidos.GetAll()
                .Where(x => !resultCotacao.Contains(x.CotacaoId) &&
                            membrosForn.Contains(x.Pedido.Membro.Id) &&
                            x.Cotacao.DtFechamento >= DateTime.Now &&
                            x.Pedido.ItemPedidos.Any(p => !intensPedidoRemoveItemForn.Contains(p.Id) &&
                            fornecedorCategorias.Contains(p.Produto.SubCategoria.CategoriaId)))
                .GroupBy(g => g.CotacaoId)
                .ToList();

                for (int i = 0; i < cotacaoPedidos.Count; i++)
                {

                    var pedidos = cotacaoPedidos[i].Select(x => x.Pedido).ToList();

                    var prod = f.Produtos.Select(x => x.ProdutoId).ToList();

                    var itensCotacao = cotacaoPedidos[i].SelectMany(x => x.Pedido.ItemPedidos.Where(p => prod.Contains(p.ProdutoId)))
                    .Select(c => new { c.ProdutoId, c.Quantidade })
                    .GroupBy(s => s.ProdutoId)
                    .Select(p => new { ProdutoId = p.Key, Quantidade = p.Sum(t => t.Quantidade) })
                    .ToList();

                    itensCotacao.ForEach(z =>
                    {
                        var categoria = f.Produtos.FirstOrDefault(x => x.ProdutoId == z.ProdutoId).Produto.SubCategoria.CategoriaId;

                        var existeProduto = f.Produtos.Count(x => x.ProdutoId == z.ProdutoId) > 0;

                        var existeProdutosFornecedor = f.Produtos.Count(x => x.ProdutoId == z.ProdutoId) > 0 &&
                            f.Produtos.FirstOrDefault(x => x.ProdutoId == z.ProdutoId).ListaQuantidadeDesconto
                            .Count(p => p.ValidadeQtdDesconto >= DateTime.Now && p.QuantidadeMinima <= z.Quantidade) > 0;

                        if (existeProduto)
                        {
                            var resultadoCotacao = new ResultadoCotacao();

                            resultadoCotacao.UsuarioCriacao = f.UsuarioCriacao;
                            resultadoCotacao.DtCriacao = DateTime.Now;
                            resultadoCotacao.Ativo = true;
                            resultadoCotacao.FornecedorId = f.Id;
                            //resultadoCotacao.Observacao = "Precificação automática";
                            resultadoCotacao.CotacaoId = cotacaoPedidos[i].FirstOrDefault().CotacaoId;
                            resultadoCotacao.ProdutoId = z.ProdutoId;
                            resultadoCotacao.PrecoNegociadoUnit = !f.Produtos.Any() ? 0 :
                                existeProdutosFornecedor
                                ?
                                Math.Round((f.Produtos.FirstOrDefault(x => x.ProdutoId == z.ProdutoId).Valor -
                                (f.Produtos.FirstOrDefault(x => x.ProdutoId == z.ProdutoId)
                                .ListaQuantidadeDesconto.Where(w => w.QuantidadeMinima <= z.Quantidade).OrderByDescending(ob
                                =>
                                ob.QuantidadeMinima).FirstOrDefault().PercentualDesconto *
                                f.Produtos.FirstOrDefault(x => x.ProdutoId == z.ProdutoId).Valor
                                ) / 100), 2)
                                :
                                f.Produtos.FirstOrDefault(x => x.ProdutoId == z.ProdutoId).Valor;

                            resultadoCotacao.Qtd = z.Quantidade;

                            _resultadoCotacao.Add(resultadoCotacao);


                            if (!listCotacaoId.Select(x => x.Key).Contains(resultadoCotacao.CotacaoId))
                                listCotacaoId.Add(new KeyValuePair<int, DateTime>(resultadoCotacao.CotacaoId, cotacaoPedidos[i].FirstOrDefault().Cotacao.DtFechamento));

                            if (!categoriasFornecedores.Contains(categoria))
                                categoriasFornecedores.Add(categoria);

                        }
                    });


                    Emails emailPrecificacao = new Emails()
                    {
                        EmailDestinatario = f.Pessoa.Usuarios.FirstOrDefault().UsuarioEmail,
                        CorpoEmail = _templateEmail.GetSingle(34).Template.Replace("#CotacaoId#", cotacaoPedidos[i].FirstOrDefault().CotacaoId.ToString()),
                        AssuntoEmail = $"Cotação {cotacaoPedidos[i].FirstOrDefault().CotacaoId} respondida automaticamente.",
                        Status = Status.NaoEnviado,
                        Origem = Origem.PrecificacaoAutomaticaCotacaoFornecedor,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacao = f.Pessoa.Usuarios.FirstOrDefault(),
                        Ativo = true
                    };

                    _emailsRep.Add(emailPrecificacao);

                    _unitOfWork.Commit();
                }

            });

            var listCotacaoUsuarios = new List<CotacaoUsuarios>();
            var fornecedor = this._fornecedoRep.FindBy(x => x.FornecedorCategorias.Any(y => categoriasFornecedores.Contains(y.CategoriaId))).Select(x => x.PessoaId).ToList();
            var usuarios = this._usuarioRep.FindBy(x => fornecedor.Contains(x.PessoaId) && !string.IsNullOrEmpty(x.TokenSignalR)).ToList();

            if (listCotacaoId.Count > 0 && usuarios.Count > 0)
            {
                listCotacaoId.ForEach(x =>
                {
                    usuarios.ForEach(u =>
                    {
                        var cotacaoGroup = CotacaoProdsGroup(u.Id, (int)x.Key);

                        listCotacaoUsuarios.Add(new CotacaoUsuarios
                        {
                            CotacaoId = x.Key,
                            TokenUsuario = u.TokenSignalR,
                            CotacaoGrupo = cotacaoGroup,
                            DataFechamentoCotacao = x.Value

                        });
                    });

                });
            }

            return listCotacaoUsuarios;
        }

        #endregion


        private List<ResultadoPrecoCotacaoFornecedor> CotacaoProdsGroup(int usuarioId, int cotacaoId)
        {
            var usuario = _usuarioRep.GetSingle(usuarioId);
            var fornecedor = _fornecedoRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

            var _param = new SqlParameter { ParameterName = "@FORNECEDORID", SqlDbType = System.Data.SqlDbType.BigInt, Value = fornecedor.Id };
            var cotacaoProdsGroup = _usuarioRep.ExecWithStoreProcedure<ResultadoPrecoCotacaoFornecedor>("stp_fornecedor_preco_cotacao @FORNECEDORID", _param).ToList();

            return cotacaoProdsGroup;
          
        }

    }

}
