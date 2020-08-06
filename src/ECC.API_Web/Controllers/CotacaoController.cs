using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Globalization;
using ECC.API_Web.Hubs;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.EntidadeCotacao;
using ECC.EntidadeEmail;
using ECC.EntidadeEndereco;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeSms;
using ECC.EntidadeStatus;
using ECC.Servicos.Abstrato;
using ECC.EntidadeAvisos;
using ECC.Servicos;
using System.Data.SqlClient;
using ECC.Entidades.EntidadeProduto;
using WebGrease.Css.Extensions;
using ECC.EntidadeProduto;
using ECC.EntidadeRecebimento;
using ECC.Entidades.EntidadePessoa;
using ECC.API_Web.Responses;
using ECC.EntidadeComum;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Fornecedor")]
    [RoutePrefix("api/cotacao")]
    public class CotacaoController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Cotacao> _cotacaoRep;
        private readonly IEntidadeBaseRep<CotacaoPedidos> _cotacaoPedidos;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedoRep;
        private readonly IEntidadeBaseRep<ResultadoCotacao> _resultadoCotacao;
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedor;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<ItemPedido> _itemPedidoRep;
        private readonly IEntidadeBaseRep<HistStatusPedido> _histStatusPedido;
        private readonly IEntidadeBaseRep<StatusSistema> _statusSisRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IEntidadeBaseRep<Sms> _smsRep;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;
        private readonly IEntidadeBaseRep<FornecedorProduto> _fornecedorProdutoRep;
        private readonly IEntidadeBaseRep<RemoveFornPedido> _removeFornPedidoRep;
        private readonly IEmailService _emailService;
        private readonly IUtilService _ultilService;
        private readonly IPagamentoService _pagamentoService;
        private readonly IEntidadeBaseRep<IndisponibilidadeProduto> _indisponibilidadeProdutoRep;

        public CotacaoController(
            IEntidadeBaseRep<MembroFornecedor> membroFornecedor,
            IEntidadeBaseRep<ResultadoCotacao> resultadoCotacao,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<CotacaoPedidos> cotacaoPedidos,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<Cotacao> cotacaoRep,
            IEntidadeBaseRep<Pedido> pedidoRep,
            IEntidadeBaseRep<ItemPedido> itemPedidoRep,
            IEntidadeBaseRep<HistStatusPedido> histStatusPedido,
            IEntidadeBaseRep<StatusSistema> statusSisRep,
            IEntidadeBaseRep<TemplateEmail> templateEmailRep,
            IEntidadeBaseRep<Emails> emailsRep,
            IUtilService ultilService,
            IEntidadeBaseRep<Sms> smsRep,
            IEntidadeBaseRep<IndisponibilidadeProduto> indisponibilidadeProdutoRep,
            IEntidadeBaseRep<Erro> errosRepository,
            IEntidadeBaseRep<Avisos> avisosRep,
            IEntidadeBaseRep<FornecedorProduto> fornecedorProdutoRep,
            IEntidadeBaseRep<RemoveFornPedido> removeFornPedidoRep,
            IEmailService emailService,
            IPagamentoService pagamentoService,
            IUnitOfWork unitOfWork)
            : base(usuarioRep, errosRepository, unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _cotacaoRep = cotacaoRep;
            _fornecedoRep = fornecedorRep;
            _cotacaoPedidos = cotacaoPedidos;
            _resultadoCotacao = resultadoCotacao;
            _membroFornecedor = membroFornecedor;
            _pedidoRep = pedidoRep;
            _itemPedidoRep = itemPedidoRep;
            _histStatusPedido = histStatusPedido;
            _emailService = emailService;
            _templateEmailRep = templateEmailRep;
            _statusSisRep = statusSisRep;
            _emailsRep = emailsRep;
            _ultilService = ultilService;
            _smsRep = smsRep;
            _pagamentoService = pagamentoService;
            _avisosRep = avisosRep;
            _fornecedorProdutoRep = fornecedorProdutoRep;
            _indisponibilidadeProdutoRep = indisponibilidadeProdutoRep;
            _removeFornPedidoRep = removeFornPedidoRep;
        }

        [HttpGet]
        [Route("cotacaoFornecedor/{statusId:int=0}/{page:int=0}/{pageSize=10}/{dtDe?}/{dtAte?}")]
        public HttpResponseMessage CotacaoFornecedor(HttpRequestMessage request, int? statusId, int? page, int? pageSize, string dtDe = null, string dtAte = null)
        {
            var currentPage = page ?? 0;
            var currentPageSize = pageSize ?? 10;

            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedoRep.FirstOrDefault(x => x.Pessoa.Id == usuario.Pessoa.Id);
                var membrosForn = _membroFornecedor.FindBy(x => x.FornecedorId == fornecedor.Id && x.Ativo).Select(o => o.Membro.Id).ToList();

                //Chama Procedure para limpar tabela de produtos insdisponiveis caso data fim já tenha sido passada.
                DeletaProdutosIndisponiveis(fornecedor);

                HttpResponseMessage response;
                int totalCotacoes;

                var indisponibilidade = _indisponibilidadeProdutoRep
                                            .FindBy(i => i.FornecedorId == fornecedor.Id)
                                            .Select(p => p.ProdutoId).ToList();

                //necessario declarar antes deviso ao problema do servidor amazon fuso horario
                DateTime dtCt = DateTime.Now;

                if (statusId > 0)
                {
                    // Pega cotações que a data de fechamento é maior que a data atual
                    var cotacoesAbertas = _cotacaoRep.FindBy(x => x.StatusSistemaId == statusId && x.DtFechamento >= dtCt).Select(x => x.Id).ToList();

                    //pega todos os pedido desta cotação
                    var pedidoCotacao = _cotacaoPedidos.FindBy(x => membrosForn.Contains(x.Pedido.Membro.Id) &&
                    cotacoesAbertas.Contains(x.CotacaoId)).Select(r => r.PedidoId); ;

                    //pega todos os pedido da tabela remover fornecedor que esteja na cotação = ProdutoId
                    var removeItemForn = _removeFornPedidoRep
                            .GetAll().Where(x => x.FonecedorId == fornecedor.Id && pedidoCotacao.Contains(x.PedidoId))
                            .Select(p => p.ItemPedidoId).ToList();

                    var cotacoesAll = _cotacaoPedidos.FindBy(x => membrosForn.Contains(x.Pedido.Membro.Id)
                                               && cotacoesAbertas.Contains(x.CotacaoId))
                                               .GroupBy(x => new
                                               {
                                                   x.CotacaoId,
                                                   x.Cotacao.DtCriacao,
                                                   x.Cotacao.StatusSistemaId,
                                                   x.Cotacao.StatusSistema.DescStatus,
                                                   x.Cotacao.StatusSistema.Ordem,
                                                   x.Cotacao,
                                                   x.Cotacao.DtFechamento
                                               }).Select(u => new
                                               {
                                                   u.Key.CotacaoId,
                                                   u.Key.DtCriacao,
                                                   StatusId = u.Key.StatusSistemaId,
                                                   Status = u.Key.DescStatus,
                                                   OrdemStatus = u.Key.Ordem,
                                                   u.Key.DtFechamento,
                                                   QtdPedidos = u.Key.Cotacao.CotacaoPedidos.Count(d => membrosForn.Contains(d.Pedido.Membro.Id)),
                                                   QtdItem = u.Key.Cotacao.CotacaoPedidos.Where(d => membrosForn.Contains(d.Pedido.Membro.Id))
                                                               .SelectMany(x => x.Pedido.ItemPedidos.Where(i => !indisponibilidade.Contains(i.ProdutoId) && !removeItemForn.Contains(i.Id)))
                                                               .Sum(q => q.Quantidade) > 0 ?
                                                               u.Key.Cotacao.CotacaoPedidos.Where(d => membrosForn.Contains(d.Pedido.Membro.Id))
                                                               .SelectMany(x => x.Pedido.ItemPedidos.Where(i => !indisponibilidade.Contains(i.ProdutoId) && !removeItemForn.Contains(i.Id)))
                                                               .Sum(q => q.Quantidade) :
                                                               0
                                               });

                    var cotacoesFornecedor = cotacoesAll.Where(x => x.QtdItem > 0)
                            .OrderByDescending(c => c.DtCriacao)
                            .Skip(currentPage * currentPageSize)
                            .Take(currentPageSize)
                            .ToList();

                    totalCotacoes = cotacoesAll.Count();

                    var pagSet = new PaginacaoConfig<CotacaoViewModel>
                    {
                        Page = currentPage,
                        TotalCount = totalCotacoes,
                        TotalPages = (int)Math.Ceiling((decimal)totalCotacoes / currentPageSize)
                    };

                    response = request.CreateResponse(HttpStatusCode.OK, new { pagSet, cotacoesFornecedor, dateCotaNow = DateTime.Now });
                }
                else
                {
                    var cotacoesFornecedor = _cotacaoPedidos.FindBy(x => x.Cotacao.DtFechamento >= dtCt && membrosForn.Contains(x.Pedido.Membro.Id))
                            .GroupBy(x => new
                            {
                                x.CotacaoId,
                                x.Cotacao.DtCriacao,
                                x.Cotacao.StatusSistemaId,
                                x.Cotacao.StatusSistema.DescStatus,
                                x.Cotacao.StatusSistema.Ordem,
                                x.Cotacao,
                                x.Cotacao.DtFechamento
                            })
                            .Select(u => new
                            {
                                u.Key.CotacaoId,
                                u.Key.DtCriacao,
                                StatusId = u.Key.StatusSistemaId,
                                Status = u.Key.DescStatus,
                                OrdemStatus = u.Key.Ordem,
                                u.Key.DtFechamento,
                                QtdPedidos = u.Key.Cotacao.CotacaoPedidos.Count(d => membrosForn.Contains(d.Pedido.Membro.Id)),
                                QtdItem = u.Key.Cotacao.CotacaoPedidos.Where(d => membrosForn.Contains(d.Pedido.Membro.Id))
                                                .SelectMany(x => x.Pedido.ItemPedidos.Where(i => !indisponibilidade.Contains(i.ProdutoId)))
                                                .Sum(q => q.Quantidade)
                            })
                            .OrderByDescending(c => c.DtCriacao)
                            .Skip(currentPage * currentPageSize)
                            .Take(currentPageSize)
                            .ToList();

                    totalCotacoes = _cotacaoPedidos
                        .FindBy(x => x.Cotacao.DtFechamento >= dtCt && membrosForn.Contains(x.Pedido.Membro.Id))
                        .GroupBy(g => g.Cotacao).Count();



                    var pagSet = new PaginacaoConfig<CotacaoViewModel>
                    {
                        Page = currentPage,
                        TotalCount = totalCotacoes,
                        TotalPages = (int)Math.Ceiling((decimal)totalCotacoes / currentPageSize)
                    };

                    response = request.CreateResponse(HttpStatusCode.OK, new { pagSet, cotacoesFornecedor, dateCotaNow = DateTime.Now });
                }

                return response;
            });
        }

        [HttpGet]
        [Route("cotacaoProdGroup/{cotacaoId:int}")]
        public HttpResponseMessage CotacaoProdGroup(HttpRequestMessage request, int cotacaoId)
        {
            return CreateHttpResponse(request, () =>
            {
                //Limpa avisos dessa cotação para o fornecedor atual

                //var naService = new NotificacoesAlertasService();

                //naService.RemoverAvisoFornecedorCotacao(usuario.PessoaId, cotacaoId, (int)TipoAviso.NovaCotacao);

                //_unitOfWork.Commit();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedoRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var faturaPendente = _pagamentoService.VerificaFaturasFornecedor(usuario.PessoaId);

                var cotacaoProdsGroup = this.CotacaoProdsGroup(usuario.Id, cotacaoId);

                var response = request.CreateResponse(HttpStatusCode.OK, new { cotacaoProdsGroup, dateCotaNow = DateTime.Now, faturaFornecedorPendente = faturaPendente });
                return response;
            });
        }

        [HttpGet]
        [Route("cotacaoPedidosAprovarPf")]
        public HttpResponseMessage CotacaoPedidosAprovarPf(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                IEnumerable<PedidoViewModel> pedPromocaoVM = null;
                var listaPedPromocao = new List<PedidoPFViewModel>();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedoRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var cotacoesVM = new List<CotacaoItensPedidoPFViewModel>();

                //Pegar todos os ítens de pedido que o Fornecedor ganhou e não aprovou.
                var itensPedido = _itemPedidoRep.GetAll().Where(x => x.FornecedorId == fornecedor.Id && x.Pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica
                                                                     && !x.AprovacaoFornecedor && x.Ativo
                                                                     && x.AprovacaoMembro
                                                                     && (x.Pedido.StatusSistemaId.Equals(24))).Include("Pedido").ToList();

                if (itensPedido.Any())
                {
                    //Retorna os pedidos dos items.
                    var pedidos = itensPedido.Select(x => x.Pedido).ToList();
                    var pedidosIds = pedidos.Select(x => x.Id);

                    var pedPromocionais =
                    pedidos.Where(c => c.StatusSistemaId.Equals(24) &&
                    c.ItemPedidos.Select(p => p.Produto.ProdutoPromocionalId != null)
                    .FirstOrDefault())
                    .Distinct();


                    //Retorna as cotações dos pedidos
                    var cotacoes = _cotacaoPedidos.GetAll().Where(x => pedidosIds.Contains(x.PedidoId)).Select(x => x.Cotacao).Distinct().Include("CotacaoPedidos").ToList();

                    foreach (var cotacao in cotacoes)
                    {
                        var cotacaoVM = new CotacaoItensPedidoPFViewModel(cotacao.Id);
                        pedidosIds = cotacao.CotacaoPedidos.Select(x => x.PedidoId).Distinct().ToList();

                        foreach (var pedido in pedidos.Where(x => pedidosIds.Contains(x.Id)).Distinct())
                        {
                            var enderecoVM = Mapper.Map<Endereco, EnderecoViewModel>(pedido.Endereco);
                            var membroVM = Mapper.Map<Membro, MembroPFViewModel>(pedido.Membro);

                            var pedidoVM = new PedidoPFViewModel
                            {
                                PedidoId = pedido.Id,
                                Endereco = enderecoVM,
                                DtPedido = pedido.DtPedido,
                                Membro = membroVM,
                                StatusId = pedido.StatusSistemaId,
                                Itens = new List<ItemPedidoViewModel>()
                            };

                            foreach (var item in itensPedido.Where(x => x.PedidoId.Equals(pedido.Id)).Distinct())
                            {
                                var itemVM = Mapper.Map<ItemPedido, ItemPedidoViewModel>(item);
                                pedidoVM.Itens.Add(itemVM);
                            }

                            var regiaoDias = fornecedor.FornecedorRegiao
                                .FirstOrDefault(x => x.CidadeId == pedidoVM.Endereco.CidadeId);
                            var regiaoSemanal = fornecedor.FornecedorRegiaoSemanal
                                .Where(x => x.CidadeId == pedidoVM.Endereco.CidadeId).ToList();

                            if (regiaoSemanal.Any())
                            {
                                pedidoVM.PrazoEntrega = 0;
                                pedidoVM.ValorPedidoMinimo = regiaoSemanal.FirstOrDefault(x => x.VlPedMinRegiao > 0)
                                .VlPedMinRegiao.Value;
                                var semanalVM = Mapper.Map<IEnumerable<FornecedorPrazoSemanal>, IEnumerable<FornecedorPrazoSemanalViewModel>>(regiaoSemanal);
                                pedidoVM.FornecedorPrazoSemanal = semanalVM.ToList();
                            }

                            if (regiaoDias != null)
                            {
                                pedidoVM.PrazoEntrega = regiaoDias.Prazo;
                                pedidoVM.ValorPedidoMinimo = regiaoDias.VlPedMinRegiao.Value;
                            }

                            pedidoVM.QtdItem = pedidoVM.Itens.Sum(x => x.quantity);
                            pedidoVM.ValorTotal = pedidoVM.Itens.Sum(x => x.quantity * x.PrecoNegociadoUnit ?? 0);
                            cotacaoVM.Pedidos.Add(pedidoVM);
                        }

                        cotacoesVM.Add(cotacaoVM);
                    }

                    if (pedPromocionais.Any())
                    {
                        foreach (var pedido in pedPromocionais)
                        {
                            var enderecoVM = Mapper.Map<Endereco, EnderecoViewModel>(pedido.Endereco);
                            var membroVM = Mapper.Map<Membro, MembroPFViewModel>(pedido.Membro);


                            var pedidoVM = new PedidoPFViewModel
                            {
                                PedidoId = pedido.Id,
                                Endereco = enderecoVM,
                                Membro = membroVM,
                                DtPedido = pedido.DtPedido,
                                Itens = new List<ItemPedidoViewModel>()
                            };

                            foreach (var item in itensPedido.Where(x => x.PedidoId.Equals(pedido.Id)).Distinct())
                            {
                                var itemVM = Mapper.Map<ItemPedido, ItemPedidoViewModel>(item);
                                pedidoVM.Itens.Add(itemVM);
                            }

                            pedidoVM.QtdItem = pedidoVM.Itens.Sum(x => x.quantity);
                            pedidoVM.ValorTotal = pedidoVM.Itens.Sum(x => x.quantity * x.PrecoNegociadoUnit ?? 0);
                            pedidoVM.PrazoEntrega = fornecedor.FornecedorRegiao.Where(x => x.CidadeId == pedidoVM.Endereco.CidadeId).Select(p => p.Prazo).FirstOrDefault();

                            listaPedPromocao.Add(pedidoVM);
                        }
                    }
                }

                response = request.CreateResponse(HttpStatusCode.OK, new { Cotacoes = cotacoesVM, PedidosPromocao = listaPedPromocao ?? new List<PedidoPFViewModel>() });

                return response;
            });
        }

        [HttpGet]
        [Route("cotacaoPedidosAprovar")]
        public HttpResponseMessage CotacaoPedidosAprovar(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                IEnumerable<PedidoViewModel> pedPromocaoVM = null;
                var listaPedPromocao = new List<PedidoViewModel>();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedoRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var cotacoesVM = new List<CotacaoItensPedidoViewModel>();

                //Pegar todos os ítens de pedido que o Fornecedor ganhou e não aprovou.
                var itensPedido = _itemPedidoRep.GetAll().Where(x => x.FornecedorId == fornecedor.Id && x.Pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica
                                                                     && !x.AprovacaoFornecedor && x.Ativo
                                                                     && x.AprovacaoMembro
                                                                     && (x.Pedido.StatusSistemaId.Equals(24))).Include("Pedido").ToList();

                if (itensPedido.Any())
                {
                    //Retorna os pedidos dos items.
                    var pedidos = itensPedido.Select(x => x.Pedido).ToList();
                    var pedidosIds = pedidos.Select(x => x.Id);

                    var pedPromocionais =
                    pedidos.Where(c => c.StatusSistemaId.Equals(24) &&
                    c.ItemPedidos.Select(p => p.Produto.ProdutoPromocionalId != null)
                    .FirstOrDefault())
                    .Distinct();


                    //Retorna as cotações dos pedidos
                    var cotacoes = _cotacaoPedidos.GetAll().Where(x => pedidosIds.Contains(x.PedidoId)).Select(x => x.Cotacao).Distinct().Include("CotacaoPedidos").ToList();

                    foreach (var cotacao in cotacoes)
                    {
                        var cotacaoVM = new CotacaoItensPedidoViewModel(cotacao.Id);
                        pedidosIds = cotacao.CotacaoPedidos.Select(x => x.PedidoId).Distinct().ToList();

                        foreach (var pedido in pedidos.Where(x => pedidosIds.Contains(x.Id)).Distinct())
                        {
                            var enderecoVM = Mapper.Map<Endereco, EnderecoViewModel>(pedido.Endereco);
                            var membroVM = Mapper.Map<Membro, MembroViewModel>(pedido.Membro);

                            var pedidoVM = new PedidoViewModel
                            {
                                PedidoId = pedido.Id,
                                Endereco = enderecoVM,
                                DtPedido = pedido.DtPedido,
                                Membro = membroVM,
                                StatusId = pedido.StatusSistemaId,
                                Itens = new List<ItemPedidoViewModel>()
                            };

                            foreach (var item in itensPedido.Where(x => x.PedidoId.Equals(pedido.Id)).Distinct())
                            {
                                var itemVM = Mapper.Map<ItemPedido, ItemPedidoViewModel>(item);
                                pedidoVM.Itens.Add(itemVM);
                            }

                            var regiaoDias = fornecedor.FornecedorRegiao
                                .FirstOrDefault(x => x.CidadeId == pedidoVM.Endereco.CidadeId);

                            var regiaoSemanal = fornecedor.FornecedorRegiaoSemanal
                                .Where(x => x.CidadeId == pedidoVM.Endereco.CidadeId).ToList();

                            if (regiaoSemanal.Any())
                            {
                                pedidoVM.PrazoEntrega = 0;
                                pedidoVM.ValorPedidoMinimo = regiaoSemanal.FirstOrDefault(x => x.VlPedMinRegiao > 0)
                                .VlPedMinRegiao.Value;
                                var semanalVM = Mapper.Map<IEnumerable<FornecedorPrazoSemanal>, IEnumerable<FornecedorPrazoSemanalViewModel>>(regiaoSemanal);
                                pedidoVM.FornecedorPrazoSemanal = semanalVM.ToList();
                            }

                            if (regiaoDias != null)
                            {
                                pedidoVM.PrazoEntrega = regiaoDias.Prazo;
                                pedidoVM.ValorPedidoMinimo = regiaoDias.VlPedMinRegiao.Value;
                            }

                            pedidoVM.QtdItem = pedidoVM.Itens.Sum(x => x.quantity);
                            pedidoVM.ValorTotal = pedidoVM.Itens.Sum(x => x.quantity * x.PrecoNegociadoUnit ?? 0);
                            cotacaoVM.Pedidos.Add(pedidoVM);
                        }

                        cotacoesVM.Add(cotacaoVM);
                    }

                    if (pedPromocionais.Any())
                    {
                        foreach (var pedido in pedPromocionais)
                        {
                            var enderecoVM = Mapper.Map<Endereco, EnderecoViewModel>(pedido.Endereco);
                            var membroVM = Mapper.Map<Membro, MembroViewModel>(pedido.Membro);


                            var pedidoVM = new PedidoViewModel
                            {
                                PedidoId = pedido.Id,
                                Endereco = enderecoVM,
                                Membro = membroVM,
                                DtPedido = pedido.DtPedido,
                                Itens = new List<ItemPedidoViewModel>()
                            };

                            foreach (var item in itensPedido.Where(x => x.PedidoId.Equals(pedido.Id)).Distinct())
                            {
                                var itemVM = Mapper.Map<ItemPedido, ItemPedidoViewModel>(item);
                                pedidoVM.Itens.Add(itemVM);
                            }

                            pedidoVM.QtdItem = pedidoVM.Itens.Sum(x => x.quantity);
                            pedidoVM.ValorTotal = pedidoVM.Itens.Sum(x => x.quantity * x.PrecoNegociadoUnit ?? 0);
                            pedidoVM.PrazoEntrega = fornecedor.FornecedorRegiao.Where(x => x.CidadeId == pedidoVM.Endereco.CidadeId).Select(p => p.Prazo).FirstOrDefault();

                            listaPedPromocao.Add(pedidoVM);
                        }
                    }
                }

                response = request.CreateResponse(HttpStatusCode.OK, new { Cotacoes = cotacoesVM, PedidosPromocao = listaPedPromocao ?? new List<PedidoViewModel>() });

                return response;
            });
        }

        [HttpPost]
        [Route("gravarPrecoForn/{cotacaoId:int}")]
        public HttpResponseMessage GravarPrecoFornecedor(HttpRequestMessage request, int cotacaoId, IEnumerable<ItemCotacaoViewModel> itemCotacaoVm)
        {
            return CreateHttpResponse(request, () =>
            {
                var naService = new NotificacoesAlertasService();
                var lstResultadoCotacao = new List<ResultadoCotacao>();
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var fornecedor = _fornecedoRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                if (fornecedor == null) return request.CreateResponse(HttpStatusCode.OK, new { success = false });

                var cotacao = this._cotacaoRep.GetSingle(cotacaoId);


                //28=cotação encerrada. Caso o usuário tentar enviar lance com o tempo da cotação expirado o sistema indentifica aqui e redireciona
                if (cotacao.StatusSistemaId == 28)
                {
                    return request.CreateResponse(HttpStatusCode.OK, new { cotacaoEncerrada = true });

                }


                TimeSpan time = cotacao.DtFechamento.Subtract(DateTime.Now);
                var dezMinutos = new TimeSpan(0, 0, 10, 0);

                if (time < dezMinutos)
                {
                    cotacao.DtFechamento = cotacao.DtFechamento.AddSeconds(dezMinutos.Subtract(time).TotalSeconds);
                    cotacao.UsuarioAlteracao = usuario;
                    cotacao.DtAlteracao = DateTime.Now;

                    this._cotacaoRep.Edit(cotacao);
                    _unitOfWork.Commit();
                }

                var resultCotaJa = _resultadoCotacao.GetAll().Where(x => x.CotacaoId == cotacaoId && x.FornecedorId == fornecedor.Id);

                if (resultCotaJa.Any())
                    _resultadoCotacao.DeleteAll(resultCotaJa);

                lstResultadoCotacao.AddRange(itemCotacaoVm.Select(item => new ResultadoCotacao
                {
                    UsuarioCriacao = usuario,
                    DtCriacao = DateTime.Now,
                    Ativo = true,
                    FornecedorId = fornecedor.Id,
                    Observacao = item.Observacao,
                    CotacaoId = cotacaoId,
                    ProdutoId = item.ProdutoId,
                    PrecoNegociadoUnit = item.PrecoNegociadoUnit.TryParseDecimal(),
                    Qtd = item.qtd,
                    FlgOutraMarca = item.flgOutraMarca
                }));

                _resultadoCotacao.AddAll(lstResultadoCotacao);

                this._unitOfWork.Commit();

                var categorias = cotacao.CotacaoPedidos.SelectMany(x => x.Pedido.ItemPedidos.Select(y => y.Produto.SubCategoria.CategoriaId));
                var fornecedores = this._fornecedoRep.FindBy(x => x.FornecedorCategorias.Any(y => categorias.Contains(y.CategoriaId))).Select(x => x.PessoaId);

                var usuarios = this._usuarioRep.FindBy(x => fornecedores.Contains(x.PessoaId)).ToList();

                // Remove avisos de cotação pendente de dar preços
                naService.RemoverAvisoUsuarioFornecedorCotacao(usuario.PessoaId, cotacaoId, (int)TipoAviso.NovaCotacao);

                var hub = new NotificacoesHub(this._unitOfWork);

                usuarios.ForEach(x =>
                {
                    var cotacaoProdsGroup = this.CotacaoProdsGroup(x.Id, cotacaoId);
                    if (cotacaoProdsGroup != null)
                        hub.CotacaoNovoPreco(x.TokenSignalR, cotacaoId, cotacao.DtFechamento, cotacaoProdsGroup, DateTime.Now);
                });

                hub.CotacoesAtualizar();

                return request.CreateResponse(HttpStatusCode.OK, new { success = true });
            });
        }

        [HttpPost]
        [Route("aprovarPedido/{aprovacao:bool}")]
        public HttpResponseMessage AprovarPedido(HttpRequestMessage request, PedidoViewModelAprovaCancel pedidoViewModel, bool aprovacao)
        {
            return CreateHttpResponse(request, () =>
            {
                var notificacoesAlertasService = new NotificacoesAlertasService();
                var itensPed = new List<ItemPedido>();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var idFornecedor = _fornecedoRep.FindBy(x => x.PessoaId == usuario.PessoaId).Select(x => x.Id).FirstOrDefault();

                var ped = _pedidoRep.GetSingle(pedidoViewModel.PedidoId);
                var membro = ped.Membro;
                var usuarioMembro = membro.Pessoa.Usuarios.FirstOrDefault(x => x.Id == ped.UsuarioCriacaoId);


                if (aprovacao)
                {
                    itensPed = ped.ItemPedidos.Where(x => x.FornecedorId == idFornecedor).ToList();

                    foreach (var item in itensPed)
                    {
                        var itemPedido = _itemPedidoRep.GetSingle(item.Id);
                        itemPedido.AprovacaoFornecedor = true;
                        itemPedido.UsuarioAlteracao = usuario;
                        itemPedido.DtAlteracao = DateTime.Now;
                        itemPedido.Ativo = true;

                        _itemPedidoRep.Edit(itemPedido);
                    }

                    Sms sms = new Sms
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,

                        Numero = usuarioMembro.Telefones.Where(x => x.UsuarioTelId == usuarioMembro.Id).Select(t => t.DddCel).FirstOrDefault() +
                          usuarioMembro.Telefones.Where(x => x.UsuarioTelId == usuarioMembro.Id).Select(t => t.Celular).FirstOrDefault(),

                        Mensagem = "Economiza Já - Itens do pedido aprovados. Alguns itens do seu pedido " + ped.Id + " foram aprovados.",
                        Status = StatusSms.NaoEnviado,
                        OrigemSms = TipoOrigemSms.FornecedorAprovaItensPedido,
                        Ativo = true

                    };

                    #region Remove avisos de aprovação pedido no sino (Topo) para Fornecedores.

                    notificacoesAlertasService.LimparAvisoPedidoFornecedor(ped, (int)TipoAviso.PedidoPendentedeAceiteFornecedor, usuario.Id);

                    #endregion

                    #region Verifica se o membro deseja receber EMAIL e SMS

                    int tipoAvisoId = (int)TipoAviso.PedidoItensAprovados;

                    if (notificacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, tipoAvisoId, TipoAlerta.EMAIL))
                    {
                        //Envia EMAIL para membro
                        _ultilService.EnviaEmailPedido(pedidoViewModel.PedidoId, 3, usuario);
                    }
                    if (notificacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, tipoAvisoId, TipoAlerta.SMS))
                    {
                        //Envia SMS para membro
                        _smsRep.Add(sms);
                    }


                    #endregion

                }
                else
                {

                    itensPed = ped.ItemPedidos.Where(x => x.FornecedorId == idFornecedor && !x.AprovacaoFornecedor).ToList();

                    foreach (var item in itensPed)
                    {
                        var itemPedido = _itemPedidoRep.GetSingle(item.Id);
                        itemPedido.AprovacaoFornecedor = false;
                        itemPedido.UsuarioAlteracao = usuario;
                        itemPedido.DtAlteracao = DateTime.Now;
                        itemPedido.Ativo = false;

                        _itemPedidoRep.Edit(itemPedido);
                    }

                    //Envia e-mail para Membro que o pedido foi cancelado
                    var corpoEmail = _templateEmailRep.FindBy(e => e.Id == 24).Select(e => e.Template).FirstOrDefault();

                    #region Verifica se o membro deseja receber EMAIL e SMS

                    if (corpoEmail != null)
                    {
                        var corpoEmailPedPromocaoCancelada = corpoEmail.Replace("#NomeMembro#",
                            membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? membro.Pessoa.PessoaJuridica.NomeFantasia : membro.Pessoa.PessoaFisica.Nome)
                            .Replace("#Id#", ped.Id.ToString())
                            .Replace("#Motivo#", pedidoViewModel.DescMotivoCancelamento.Trim());

                        Emails emails = new Emails()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Pedido " + ped.Id.ToString() + " com itens CANCELADOS-Corre Tente trocar para outro fornecedor.",
                            EmailDestinatario = usuarioMembro.UsuarioEmail,
                            CorpoEmail = corpoEmailPedPromocaoCancelada.Trim(),
                            Status = Status.NaoEnviado,
                            Origem = Origem.FornecedorCancelaPedidoPromocional,
                            Ativo = true

                        };

                        Sms sms = new Sms()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,

                            Numero = usuarioMembro.Telefones.Where(x => x.UsuarioTelId == usuarioMembro.Id).Select(t => t.DddCel).FirstOrDefault() +
                            usuarioMembro.Telefones.Where(x => x.UsuarioTelId == usuarioMembro.Id).Select(t => t.Celular).FirstOrDefault(),

                            Mensagem = "Economiza Já-Alguns itens do seu pedido " + ped.Id + " foram CANCELADOS.Corre Tente trocar por outro fornecedor",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.FornecedorCancelaPedidoPromocional,
                            Ativo = true

                        };

                        //Verifica se o membro deseja receber EMAIL OU SMS

                        int tipoAvisoId = (int)TipoAviso.PedidoCancelado;


                        if (notificacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, tipoAvisoId, TipoAlerta.EMAIL))
                        {
                            //Envia EMAIL para membro
                            _emailsRep.Add(emails);
                        }

                        if (notificacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, tipoAvisoId, TipoAlerta.SMS))
                        {
                            //Envia SMS para membro
                            _smsRep.Add(sms);
                        }

                    }

                    #endregion

                    #region Inserir Aviso para o Membro de Pedido Cancelado


                    Avisos novoAviso = new Avisos();

                    novoAviso.UsuarioCriacaoId = 1;
                    novoAviso.DtCriacao = DateTime.Now;
                    novoAviso.Ativo = true;
                    novoAviso.IdReferencia = ped.Id;
                    novoAviso.DataUltimoAviso = DateTime.Now;
                    novoAviso.ExibeNaTelaAvisos = true;
                    novoAviso.TipoAvisosId = (int)TipoAviso.PedidoCotacaoItensCancelados; //Id Aviso
                    novoAviso.URLPaginaDestino = "/#/meusPedidos";
                    novoAviso.TituloAviso = "Iten(s) do pedido não aprovado(s)";
                    novoAviso.ToolTip = "Iten(s) do pedido não aprovado(s)";
                    string descAviso = "Alguns itens do pedido " + ped.Id + " não foram aprovados";
                    novoAviso.DescricaoAviso = descAviso.Length > 99 ? descAviso.Substring(0, 99) : descAviso;
                    novoAviso.ModuloId = 3; //Modulo Membro
                    novoAviso.UsuarioNotificadoId = usuarioMembro.Id;
                    _avisosRep.Add(novoAviso);

                    #endregion

                    #region Remove avisos de aprovação pedido no sino (Topo) para Fornecedores.

                    notificacoesAlertasService.LimparAvisoPedidoFornecedor(ped, (int)TipoAviso.PedidoPendentedeAceiteFornecedor, usuario.Id);

                    #endregion
                }

                var cotacaoId = _cotacaoPedidos.FindBy(x => x.PedidoId == pedidoViewModel.PedidoId).Select(p => p.CotacaoId).FirstOrDefault();

                ValidarAprovacaoPedido(cotacaoId);
                _unitOfWork.Commit();

                if (aprovacao)
                    this._pagamentoService.GerarComissao(usuario.Id, pedidoViewModel.PedidoId, idFornecedor);

                #region Inserir motivo do cancelamento dos itens na tabela de Histórico Pedido

                if (!string.IsNullOrEmpty(pedidoViewModel.DescMotivoCancelamento))
                {
                    var pedidoHistorico = new HistStatusPedido();
                    pedidoHistorico.Ativo = true;
                    pedidoHistorico.UsuarioCriacao = usuario;
                    pedidoHistorico.DtCriacao = DateTime.Now;
                    pedidoHistorico.Pedido = ped;
                    pedidoHistorico.PedidoId = ped.Id;
                    pedidoHistorico.DescMotivoCancelamento = pedidoViewModel.DescMotivoCancelamento;
                    pedidoHistorico.StatusSistemaId = 36; //Status do Pedido Cancelado

                    _histStatusPedido.Add(pedidoHistorico);
                    _unitOfWork.Commit();
                }
                #endregion

                var response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }

        [HttpGet]
        [Route("verificaSeCotacaoForn")]
        public HttpResponseMessage VerificaSeCotacaoForn(HttpRequestMessage request, int cotacaoId)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedoRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var membrosForn = _membroFornecedor.GetAll().Where(x => x.FornecedorId == fornecedor.Id && x.Ativo).Select(o => o.Membro.Id).ToList();

                var totalPed = _cotacaoPedidos.GetAll().Count(x => x.CotacaoId == cotacaoId && membrosForn.Contains(x.Pedido.Membro.Id));

                var response = request.CreateResponse(HttpStatusCode.OK, totalPed);
                return response;
            });
        }

        [HttpGet]
        [Route("mapa/{cotacaoId:int=0}")]
        public HttpResponseMessage Mapa(HttpRequestMessage request, int cotacaoId)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var enderecoFornecedor = usuario.Pessoa.Enderecos.FirstOrDefault();
                var fornecedorBD = _fornecedoRep.FirstOrDefault(x => x.Pessoa.Id == usuario.Pessoa.Id);
                //Caso a localização do usuário não esteja cadastrado, atualiza a posição geográfica do usuário
                if (enderecoFornecedor.Localizacao == null)
                    enderecoFornecedor.LocalizacaoGoogle();

                var fornecedor = new
                {
                    Id = fornecedorBD.Id,
                    PessoaId = usuario.PessoaId,
                    Nome = usuario.Pessoa.PessoaJuridica.NomeFantasia,
                    enderecoFornecedor.Localizacao.Latitude,
                    enderecoFornecedor.Localizacao.Longitude
                };

                var cotacao = this._cotacaoRep.GetSingle(cotacaoId);

                var pedidos = new List<object>();

                //Aplicar todas as regras de Relacionamento Fornecedor X Pedido
                //Trazer apenas pedidos que o Fornecedor Ganhou
                var membrosForn = _membroFornecedor.FindBy(x => x.FornecedorId == fornecedor.Id && x.Ativo).Select(o => o.Membro.Id).ToList();

                var indisponibilidade = _indisponibilidadeProdutoRep
                                            .FindBy(i => i.FornecedorId == fornecedor.Id)
                                            .Select(p => p.ProdutoId).ToList();


                // Pega cotações que a data de fechamento é maior que a data atual
                var cotacoesAbertas = _cotacaoRep.FindBy(x => x.Id == cotacaoId).Select(x => x.Id).ToList();

                //pega todos os pedido desta cotação
                var pedidoCotacao = _cotacaoPedidos.FindBy(x => membrosForn.Contains(x.Pedido.Membro.Id) &&
                cotacoesAbertas.Contains(x.CotacaoId)).Select(r => r.PedidoId);

                //pega todos os pedido da tabela remover fornecedor que esteja na cotação = ProdutoId
                var removeItemForn = _removeFornPedidoRep
                        .GetAll().Where(x => x.FonecedorId == fornecedor.Id && pedidoCotacao.Contains(x.PedidoId))
                        .Select(p => p.ItemPedidoId).ToList();

                //Antes da cotação, verificar na tabela de Fornecedor excluido.
                foreach (var ped in cotacao.CotacaoPedidos.Where(w => pedidoCotacao.Contains(w.PedidoId) &&
                                w.Pedido.ItemPedidos.Where(i => !indisponibilidade.Contains(i.ProdutoId) && !removeItemForn.Contains(i.Id)).Any()).Select(x => x.Pedido).Distinct())
                {
                    var endereco = ped.Endereco;
                    if (endereco.Localizacao == null)
                        endereco.LocalizacaoGoogle();

                    if (endereco.Localizacao == null) continue;

                    var distancia = endereco.Localizacao.Distance(enderecoFornecedor.Localizacao);

                    pedidos.Add(new
                    {
                        Id = endereco.PessoaId,
                        Nome = endereco.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? endereco.Pessoa.PessoaJuridica.NomeFantasia : endereco.Pessoa.PessoaFisica.Nome,
                        endereco.Localizacao.Latitude,
                        endereco.Localizacao.Longitude,
                        Distancia = distancia > 1000 ? $"{distancia / 1000:F2} km" : $"{distancia:F2} m"
                    });
                }

                var response = request.CreateResponse(HttpStatusCode.OK, new
                {
                    Fornecedor = fornecedor,
                    Pedidos = pedidos
                });

                this._unitOfWork.Commit();

                return response;
            });
        }

        [HttpGet]
        [Route("cotacaoItensFornecedor/{cotacaoId:int}")]
        public HttpResponseMessage CotacaoItensFornecedor(HttpRequestMessage request, int cotacaoId)
        {
            return CreateHttpResponse(request, () =>
            {

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var fornecedor = _fornecedoRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);


                var listparameters = new[]
                {
                    new SqlParameter { ParameterName = "@COTACAOID", SqlDbType = System.Data.SqlDbType.BigInt, Value = cotacaoId },
                    new SqlParameter { ParameterName = "@FORNECEDORID", SqlDbType = System.Data.SqlDbType.BigInt, Value = fornecedor.Id }

                 };

                var result = _usuarioRep.ExecWithStoreProcedure<ResultadoCotacaoFornecedorResponse>("stp_result_cotacao_part_forn @FORNECEDORID, @COTACAOID", listparameters);


                var response = request.CreateResponse(HttpStatusCode.OK, new { itensCotacao = result });

                return response;
            });
        }


        #region Metodos Auxiliares

        private List<ResultadoPrecoCotacaoFornecedor> CotacaoProdsGroup(int usuarioId, int cotacaoId)
        {
            var usuario = _usuarioRep.GetSingle(usuarioId);
            var fornecedor = _fornecedoRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);


            var listparameters = new[]
           {
                    new SqlParameter { ParameterName = "@idCotacao", SqlDbType = System.Data.SqlDbType.BigInt, Value = cotacaoId },
                    new SqlParameter { ParameterName = "@FORNECEDORID", SqlDbType = System.Data.SqlDbType.BigInt, Value = fornecedor.Id }

           };

            var cotacaoProdsGroup = _usuarioRep.ExecWithStoreProcedure<ResultadoPrecoCotacaoFornecedor>("stp_fornecedor_preco_cotacao @FORNECEDORID, @idCotacao", listparameters).ToList();

            return cotacaoProdsGroup;
        }


        /// <summary>
        /// Valida se o pedido ainda possui itens não aprovados por outros fornecedores
        /// se existirem manda e-mail para os fornecedores. Se todos os ítens estiverem aprovados
        /// mudamos o status do pedido.
        /// </summary>
        /// <param name="cotacaoId"></param>
        private void ValidarAprovacaoPedido(int cotacaoId)
        {
            var cotacao = _cotacaoRep.GetSingle(cotacaoId);

            foreach (var pedido in cotacao.CotacaoPedidos)
            {
                //var itensPedido = _itemPedidoRep.GetAll().Where(x => x.PedidoId == pedido.PedidoId);
                var _pedido = _pedidoRep.GetSingle(pedido.PedidoId);
                var itensPedido = _pedido.ItemPedidos;

                var pedidosNaoAprovados = itensPedido.Where(x => !x.AprovacaoFornecedor && x.AprovacaoMembro && x.Ativo).ToList();

                // Se existir um item de pedido false (não aprovado), manda email para o fornecedor aprovar
                if (pedidosNaoAprovados.Count > 0)
                {
                    //Agrupa os ID's dos fornecedores que ainda não aprovaram.
                    var fornecedoresPedidosNaoAprovados = (from item in pedidosNaoAprovados
                                                           group item by item.FornecedorId into g
                                                           select new { fornecedorId = Convert.ToInt32(g.Key) })
                                                           .Distinct()
                                                           .ToList();

                    //Enviando e-mail para os fornecedores que ainda não aprovaram.
                    //foreach (var forn in fornecedoresPedidosNaoAprovados)
                    //{
                    //    var fornecedor = _fornecedoRep.GetSingle(forn.fornecedorId);
                    //    if (fornecedor != null)
                    //    {
                    //        var usuario = _usuarioRep.GetAll().FirstOrDefault(x => x.PessoaId == fornecedor.PessoaId);

                    //        var template = _templateEmailRep.GetSingle(18).Template;

                    //        var corpoEmail = template.Replace("#NomeFantasia#", fornecedor.Pessoa.PessoaJuridica.NomeFantasia);

                    //        Emails email1 = new Emails()
                    //        {
                    //            EmailDestinatario = usuario.UsuarioEmail,
                    //            CorpoEmail =
                    //                _emailService.MontaEmail(itensPedido.Where(x => x.FornecedorId == fornecedor.Id),
                    //                    corpoEmail),
                    //            AssuntoEmail = "Cotação " + cotacaoId + " -  Aguardando Aprovação de Pedido  " + pedido.Id,
                    //            Status = Status.NaoEnviado,
                    //            Origem = Origem.PedidoMembroGerado,
                    //            DtCriacao = DateTime.Now,
                    //            UsuarioCriacao = usuario

                    //        };

                    //        _emailsRep.Add(email1);
                    //        _unitOfWork.Commit();


                    //        Sms sms = new Sms
                    //        {
                    //            UsuarioCriacao = usuario,
                    //            DtCriacao = DateTime.Now,
                    //            Numero = usuario.Telefones.Select(t => t.DddCel).FirstOrDefault() + usuario.Telefones.Select(t => t.Celular).FirstOrDefault(),
                    //            Mensagem = "Economiza Já - Cotação "+ cotacaoId+ ". Aguardando Aprovação de Pedidos" + pedido.Id,
                    //            Status = StatusSms.NaoEnviado,
                    //            OrigemSms = TipoOrigemSms.PedidosPendentesAprovacaoFronecedor,
                    //            Ativo = true
                    //        };


                    //        _smsRep.Add(sms);
                    //        _unitOfWork.Commit();
                    //    }
                    //}
                }
                else
                {
                    var tipoFinalizacao = itensPedido.Any(x => x.Ativo);

                    //Mudar status do Pedido para Aguardando Entrega pois o fornecedor já deu o aceite.
                    if (tipoFinalizacao)
                    {
                        _pedido.StatusSistemaId =
                            _statusSisRep.FindBy(x => x.WorkflowStatusId == 12 && x.Ordem == 5 && x.Ativo)
                                .Select(i => i.Id)
                                .FirstOrDefault();
                    }
                    else
                    {
                        _pedido.StatusSistemaId =
                            _statusSisRep.FindBy(x => x.WorkflowStatusId == 12 && x.Ordem == 10)
                            .Select(i => i.Id)
                            .FirstOrDefault();
                    }
                    _pedidoRep.Edit(_pedido);

                }

            }


        }

        public int GetCotacaoIdByPedidoId(int pedidoId)
        {
            //Cotações dos pedidos.
            return _cotacaoPedidos.GetSingle(pedidoId).PedidoId;
        }

        private void DeletaProdutosIndisponiveis(Fornecedor fornecedor)
        {
            _indisponibilidadeProdutoRep.ExecuteWithStoreProcedure("stp_upd_indisponibilidade_produto @FornecedorId",
                new SqlParameter("@FornecedorId", fornecedor.Id));
        }
        #endregion
    }
}
