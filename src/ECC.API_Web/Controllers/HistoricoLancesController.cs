using ECC.API_Web.InfraWeb;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.EntidadeRelatorio;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeStatus;
using ECC.EntidadeUsuario;
using ECC.API_Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using AutoMapper;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Fornecedor, Membro")]
    [RoutePrefix("api/historicolance")]
    public class HistoricoLancesController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<HistStatusPedido> _histStatusPedido;
        private readonly IEntidadeBaseRep<HistStatusCotacao> _histStatusCotacao;
        private readonly IEntidadeBaseRep<Produto> _produtoRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedorRep;
        private readonly IEntidadeBaseRep<StatusSistema> _statusSisRep;
        private readonly IEntidadeBaseRep<ItemPedido> _itemPedidoRep;
        private readonly IEntidadeBaseRep<AvaliacaoFornecedor> _avaliacaoFornecedorRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<ResultadoCotacao> _resultadoCotacaoRep;
        private readonly IEntidadeBaseRep<Cotacao> _cotacaoRep;
        private readonly IEntidadeBaseRep<CotacaoPedidos> _cotacaoPedidosRep;

        public HistoricoLancesController(IEntidadeBaseRep<Produto> produtoRep,
            IEntidadeBaseRep<StatusSistema> statusSisRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<MembroFornecedor> membroFornecedorRep,
            IEntidadeBaseRep<Pedido> pedidoRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<HistStatusPedido> histStatusPedido,
            IEntidadeBaseRep<HistStatusCotacao> histStatusCotacao,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<ItemPedido> itemPedidoRep,
            IEntidadeBaseRep<AvaliacaoFornecedor> avaliacaoFornecedorRep,
            IEntidadeBaseRep<MembroCategoria> membroCategoriaRep,
            IEntidadeBaseRep<ResultadoCotacao> resultadoCotacaoRep,
            IEntidadeBaseRep<Cotacao> cotacaoRep,
            IEntidadeBaseRep<CotacaoPedidos> cotacaoPedidosRep,
        IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
            _pedidoRep = pedidoRep;
            _fornecedorRep = fornecedorRep;
            _histStatusPedido = histStatusPedido;
            _histStatusCotacao = histStatusCotacao;
            _membroRep = membroRep;
            _membroFornecedorRep = membroFornecedorRep;
            _statusSisRep = statusSisRep;
            _produtoRep = produtoRep;
            _itemPedidoRep = itemPedidoRep;
            _avaliacaoFornecedorRep = avaliacaoFornecedorRep;
            _membroCategoriaRep = membroCategoriaRep;
            _resultadoCotacaoRep = resultadoCotacaoRep;
            _cotacaoRep = cotacaoRep;
            _cotacaoPedidosRep = cotacaoPedidosRep;
        }



        [HttpPost]
        [Route("histLancesFornecedorPorPedido/{pedidoId:int}")]
        public HttpResponseMessage histLancesFornecedorPorPedido(HttpRequestMessage request, int pedidoId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                Membro _membro = _membroRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                List<int> _fornecedoresMembro = _membroFornecedorRep.FindBy(x => x.MembroId == _membro.Id).Select(f => f.FornecedorId).ToList();

                if (pedidoId > 0)
                {
                    Pedido pedidos = this._pedidoRep.GetAll().Where(x => x.Id.Equals(pedidoId)).FirstOrDefault();

                    List<ItemPedidosFornecedores> listaPedidoFornecedor = new List<ItemPedidosFornecedores>();
                    List<FornecedorLance> listaFornecedor = new List<FornecedorLance>();
                    CotacaoPedidos CP = _cotacaoPedidosRep.GetAll().Where(cp => cp.PedidoId == pedidos.Id).FirstOrDefault();
                    foreach (var item in pedidos.ItemPedidos)
                    {
                        listaFornecedor = new List<FornecedorLance>();


                        List<ResultadoCotacao> lResultCotacao = null;


                        lResultCotacao = _resultadoCotacaoRep.FindBy(rc => rc.CotacaoId == CP.CotacaoId
                                                                        && rc.ProdutoId == item.ProdutoId
                                                                        && rc.FlgOutraMarca == item.FlgOutraMarca
                                                                        && _fornecedoresMembro.Contains(rc.FornecedorId)).ToList();



                        foreach (var itemResultado in lResultCotacao)
                        {
                            listaFornecedor.Add(new FornecedorLance
                            {
                                ItemFornecedorLance = itemResultado.Fornecedor,
                                valorLance = itemResultado.PrecoNegociadoUnit,
                                obs = itemResultado.Observacao
                            });


                        }
                        listaPedidoFornecedor.Add(new ItemPedidosFornecedores
                        {
                            ItemPedidos = item,
                            ListFornecedorLance = listaFornecedor
                        });

                    }

                    PedidoFornecedoresPorITem histPedidoCotacao = new PedidoFornecedoresPorITem();

                    histPedidoCotacao.Id = pedidos.Id;
                    histPedidoCotacao.DtPedido = pedidos.DtPedido;
                    histPedidoCotacao.FlgCotado = pedidos.FlgCotado;
                    histPedidoCotacao.StatusSistemaId = pedidos.StatusSistemaId;
                    histPedidoCotacao.MembroId = pedidos.MembroId;
                    histPedidoCotacao.EnderecoId = pedidos.EnderecoId;
                    histPedidoCotacao.ItemPedidosFornecedor = listaPedidoFornecedor;

                    PedidoFornecedoresPorItemViewModel histPedidoCotacaoVM = Mapper.Map<PedidoFornecedoresPorITem, PedidoFornecedoresPorItemViewModel>(histPedidoCotacao);


                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true, lancesFornecedorPedido = histPedidoCotacaoVM });

                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Não foi possível devolver a dados de itens do pedido " + pedidoId);
                }

                return response;
            });
        }

    }
}
