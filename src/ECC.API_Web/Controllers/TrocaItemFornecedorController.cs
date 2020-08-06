using ECC.API_Web.InfraWeb;
using System.Web.Http;
using System.Web.Http.Cors;

using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
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
using Microsoft.AspNet.Identity;
using AutoMapper;
using ECC.EntidadeRelatorio;
using ECC.EntidadeEmail;
using ECC.EntidadeSms;
using ECC.EntidadeAvisos;
using ECC.Servicos.Abstrato;
using ECC.EntidadeFormaPagto;

namespace ECC.API_Web.Controllers
{


    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Fornecedor, Membro")]
    [RoutePrefix("api/TrocaItemFornecedor")]
    public class TrocaItemFornecedorController : ApiControllerBase
    {

        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<HistStatusPedido> _histStatusPedido;
        private readonly IEntidadeBaseRep<HistStatusCotacao> _histStatusCotacao;
        private readonly IEntidadeBaseRep<Produto> _produtoRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<StatusSistema> _statusSisRep;
        private readonly IEntidadeBaseRep<ItemPedido> _itemPedidoRep;
        private readonly IEntidadeBaseRep<AvaliacaoFornecedor> _avaliacaoFornecedorRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<ResultadoCotacao> _resultadoCotacaoRep;
        private readonly IEntidadeBaseRep<Cotacao> _cotacaoRep;
        private readonly IEntidadeBaseRep<CotacaoPedidos> _cotacaoPedidosRep;
        private readonly IEntidadeBaseRep<FornecedorFormaPagto> _fornecedorFormaPagtoRep;
        private readonly INotificacoesAlertasService _notificacoesAlertaService;
        private readonly IUtilService _utilServiceRep;


        public TrocaItemFornecedorController(IEntidadeBaseRep<Produto> produtoRep,
        IEntidadeBaseRep<StatusSistema> statusSisRep,
        IEntidadeBaseRep<Membro> membroRep,
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
         IEntidadeBaseRep<FornecedorFormaPagto> fornecedorFormaPagtoRep,

        INotificacoesAlertasService notificacoesAlertaService,
        IUtilService utilServiceRep,

    IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
        : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
            _pedidoRep = pedidoRep;
            _fornecedorRep = fornecedorRep;
            _histStatusPedido = histStatusPedido;
            _histStatusCotacao = histStatusCotacao;
            _membroRep = membroRep;
            _statusSisRep = statusSisRep;
            _produtoRep = produtoRep;
            _itemPedidoRep = itemPedidoRep;
            _avaliacaoFornecedorRep = avaliacaoFornecedorRep;
            _membroCategoriaRep = membroCategoriaRep;
            _resultadoCotacaoRep = resultadoCotacaoRep;
            _cotacaoRep = cotacaoRep;
            _cotacaoPedidosRep = cotacaoPedidosRep;
            _fornecedorFormaPagtoRep = fornecedorFormaPagtoRep;

            _notificacoesAlertaService = notificacoesAlertaService;
            _utilServiceRep = utilServiceRep;
        }




        [HttpPost]
        [Route("trocaFornecedorItem/{pedidoId:int}/{itemProdutoId:int}/{flgOutraMarcaTroca}")]
        public HttpResponseMessage TrocaFornecedorItem(HttpRequestMessage request, int pedidoId, int itemProdutoId, bool flgOutraMarcaTroca)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                if (pedidoId > 0)
                {
                    Pedido pedido = this._pedidoRep.GetAll().Where(x => x.Id.Equals(pedidoId)).FirstOrDefault();
                    CotacaoPedidos CP = _cotacaoPedidosRep.FindBy(cp => cp.PedidoId == pedido.Id).FirstOrDefault();

                    var listFornecedoresId = pedido.ItemPedidos.Where(x => x.AprovacaoMembro && !x.AprovacaoFornecedor && !x.Ativo && x.ProdutoId == itemProdutoId)
                                                               .Select(f => f.FornecedorId).ToList();


                    var listPrecoItemFornecedor = _resultadoCotacaoRep.FindBy(rc => rc.CotacaoId == CP.CotacaoId
                                                                    && rc.ProdutoId == itemProdutoId
                                                                    && rc.FlgOutraMarca == flgOutraMarcaTroca
                                                                    && !listFornecedoresId.Contains(rc.FornecedorId))
                                                                      .Select(r => new
                                                                      {
                                                                          fornecedor = r.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia,
                                                                          r.PrecoNegociadoUnit,
                                                                          r.FornecedorId,
                                                                          r.Observacao,
                                                                          r.Qtd,
                                                                          SubTotal = (r.Qtd * r.PrecoNegociadoUnit),
                                                                          FornecedorFormaPagtos = r.Fornecedor
                                                                          .FornecedorFormaPagtos
                                                                          .Select(f => new TrocaItemFornecedorResponseViewModel
                                                                          {
                                                                              DescFormaPagto = f.FormaPagto.DescFormaPagto,
                                                                              Desconto = f.Desconto,
                                                                              QtdParcelas = f.FormaPagto.QtdParcelas,
                                                                              FormaPagtoId = f.FormaPagtoId,
                                                                              ValorMinParcela = f.ValorMinParcela == null ? 0 : f.ValorMinParcela.Value,
                                                                              ValorMinParcelaPedido = f.ValorMinParcelaPedido == null ? 0 : f.ValorMinParcelaPedido.Value,

                                                                          }).ToList()
                                                                      }).ToList();

                    listPrecoItemFornecedor.ForEach(x =>
                    {
                        var lista = new List<TrocaItemFornecedorResponseViewModel>();

                        x.FornecedorFormaPagtos.ForEach(f =>
                        {
                            if (pedido.ItemPedidos.Count(i => i.FornecedorId == x.FornecedorId) > 0)
                            {
                                var totalItem = pedido.ItemPedidos.Where(i => i.FornecedorId == x.FornecedorId).Sum(s => s.PrecoNegociadoUnit);
                                var soma = (totalItem + x.PrecoNegociadoUnit) + (((totalItem + x.PrecoNegociadoUnit) * f.Desconto) / 100);

                                if (f.ValorMinParcelaPedido > 0)
                                    f.PodeSelecionar = soma > f.ValorMinParcelaPedido;
                                else if (f.ValorMinParcela > 0)
                                    f.PodeSelecionar = (soma / f.QtdParcelas) > f.ValorMinParcela;
                                else
                                    f.PodeSelecionar = true;

                                if (f.PodeSelecionar)
                                    lista.Add(f);

                            }
                            else
                            {
                                if (f.ValorMinParcelaPedido > 0)
                                    f.PodeSelecionar = (((x.PrecoNegociadoUnit * f.Desconto) / 100) + x.PrecoNegociadoUnit) > f.ValorMinParcelaPedido;
                                else if (f.ValorMinParcela > 0)
                                    f.PodeSelecionar = (((x.PrecoNegociadoUnit * f.Desconto) / 100) + x.PrecoNegociadoUnit / f.ValorMinParcela) >= f.QtdParcelas;
                                else
                                    f.PodeSelecionar = true;

                                if (f.PodeSelecionar)
                                    lista.Add(f);
                            }                         

                        });

                        x.FornecedorFormaPagtos.Clear();
                        x.FornecedorFormaPagtos.AddRange(lista);
                    });

                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true, listPrecoItemFornecedor });

                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Não foi possível devolver a dados de  Lista de fornecedores que deram preço" + pedidoId);
                }

                return response;
            });
        }



        [HttpPost]
        [Route("trocaItemFornecedor/{pedidoId:int}/{fornecedorId:int}")]
        public HttpResponseMessage TrocaItemFornecedor(HttpRequestMessage request, int pedidoId, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                if (pedidoId > 0)
                {
                    Pedido pedido = this._pedidoRep.GetAll().Where(x => x.Id.Equals(pedidoId)).FirstOrDefault();

                    List<ItemPedidosFornecedores> listaPedidoFornecedor = new List<ItemPedidosFornecedores>();

                    List<FornecedorLance> listaFornecedor = new List<FornecedorLance>();

                    CotacaoPedidos CP = _cotacaoPedidosRep.FindBy(cp => cp.PedidoId == pedido.Id).FirstOrDefault();

                    foreach (var item in pedido.ItemPedidos)
                    {
                        listaFornecedor = new List<FornecedorLance>();

                        List<ResultadoCotacao> lResultCotacao = _resultadoCotacaoRep.FindBy(rc => rc.CotacaoId == CP.CotacaoId
                                                                    && rc.FornecedorId == fornecedorId
                                                                    && rc.ProdutoId == item.ProdutoId
                                                                    && rc.FlgOutraMarca == item.FlgOutraMarca).ToList();

                        foreach (var itemResultado in lResultCotacao)
                        {


                            if (itemResultado.PrecoNegociadoUnit > 0)
                                listaFornecedor.Add(new FornecedorLance
                                {
                                    ItemFornecedorLance = itemResultado.Fornecedor,
                                    valorLance = itemResultado.PrecoNegociadoUnit,
                                    obs = itemResultado.Observacao,
                                    qtd = item.Quantidade
                                });
                        }

                        if (listaFornecedor.Count > 0)
                            listaPedidoFornecedor.Add(new ItemPedidosFornecedores
                            {
                                ItemPedidos = item,
                                ListFornecedorLance = listaFornecedor
                            });

                    }

                    PedidoFornecedoresPorITem histPedidoCotacao = new PedidoFornecedoresPorITem();

                    //histPedidoCotacao.Id = pedido.Id;
                    //histPedidoCotacao.DtPedido = pedido.DtPedido;
                    //histPedidoCotacao.FlgCotado = pedido.FlgCotado;
                    //histPedidoCotacao.StatusSistemaId = pedido.StatusSistemaId;
                    //histPedidoCotacao.MembroId = pedido.MembroId;
                    //histPedidoCotacao.EnderecoId = pedido.EnderecoId;



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




        [HttpPost]
        [Route("totalItensRespondidos/{pedidoId:int}/{fornecedorId:int}")]
        public HttpResponseMessage TotalItensRespondidos(HttpRequestMessage request, int pedidoId, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                if (pedidoId > 0)
                {
                    Pedido pedido = this._pedidoRep.GetAll().Where(x => x.Id.Equals(pedidoId)).FirstOrDefault();
                    List<ResultadoCotacao> lResultCotacao = null;
                    var idCp = _cotacaoPedidosRep.GetAll().Where(cp => cp.PedidoId == pedido.Id).Select(x => x.CotacaoId).FirstOrDefault();

                    var totalItensRespondidoFornecedor = 0;
                    foreach (var item in pedido.ItemPedidos)
                    {

                        if (
                            _resultadoCotacaoRep.FindBy(rc => rc.CotacaoId == idCp
                                                           && rc.FornecedorId == fornecedorId
                                                           && rc.ProdutoId == item.ProdutoId
                                                           && rc.FlgOutraMarca == item.FlgOutraMarca).Any()
                           )
                        {
                            totalItensRespondidoFornecedor++;
                        }


                    }

                    var totalItens = pedido.ItemPedidos.Count;


                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true, totalItens, totalItensRespondidoFornecedor });

                }

                return response;
            });
        }




        [HttpPost]
        [Route("salvaTrocaItemFornecedor")]
        public HttpResponseMessage SalvaTrocaItemFornecedor(HttpRequestMessage request, List<TrocaItemFornecedorViewModel> itensTroca)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                foreach (var item in itensTroca)
                {
                    var itemToChange = _itemPedidoRep.GetSingle(item.ItemId);

                    itemToChange.Observacao = item.Observacao;
                    itemToChange.FornecedorId = item.FornecedorId;
                    itemToChange.PrecoNegociadoUnit = item.ValorItemFornecedor;
                    itemToChange.UsuarioAlteracaoId = usuario.Id;
                    _itemPedidoRep.Edit(itemToChange);
                    _unitOfWork.Commit();
                }

                response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }


        [HttpPost]
        [Route("salvaTrocaMembroItemFornecedor")]
        public HttpResponseMessage SalvaTrocaMembroItemFornecedor(HttpRequestMessage request, TrocaItemFornecedorViewModel itemTroca)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var itemToChange = _itemPedidoRep.GetSingle(itemTroca.ItemId);
                var _pedido = _pedidoRep.GetSingle(itemToChange.PedidoId);


                //Verifca se o fornecedor escolhido ainda nao aprovou os itens que o mesmo ganhou       
                var existePedParaAprovar = _pedido.ItemPedidos.Any(x => x.FornecedorId == itemTroca.FornecedorId && !x.AprovacaoFornecedor && x.Ativo);


                if (existePedParaAprovar)
                {
                    var formaPagtoIdFornecedorChosen = _pedido.ItemPedidos.Where(x => x.FornecedorId == itemTroca.FornecedorId).Select(x => x.FormaPagtoId).FirstOrDefault();


                    itemToChange.Observacao = itemTroca.Observacao;
                    itemToChange.FornecedorId = itemTroca.FornecedorId;
                    itemToChange.PrecoNegociadoUnit = itemTroca.ValorItemFornecedor;
                    itemToChange.UsuarioAlteracaoId = usuario.Id;
                    itemToChange.FormaPagtoId = formaPagtoIdFornecedorChosen;
                    itemToChange.AprovacaoFornecedor = false;
                    itemToChange.Ativo = true;

                    _itemPedidoRep.Edit(itemToChange);
                    _unitOfWork.Commit();

                    _utilServiceRep.EnviaEmailSmsNovoPedidoItemfornecedor(_pedido.Id, itemTroca.ItemId, usuario, existePedParaAprovar);
                }
                else
                {

                    if (itemTroca.FornecedorPagtoIdChosen == 0)
                    {

                        ModelState.AddModelError("FormaPagto de Pagamento", "Selecione uma forma de pagamento para o fornecedor selecionado");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                        return response;
                    }
                    else
                    {
                        var formaPagtoIdFornecedorChosen = _pedido.ItemPedidos.Where(x => x.FornecedorId == itemTroca.FornecedorId).Select(x => x.FormaPagtoId).FirstOrDefault();


                        itemToChange.Observacao = itemTroca.Observacao;
                        itemToChange.FornecedorId = itemTroca.FornecedorId;
                        itemToChange.PrecoNegociadoUnit = itemTroca.ValorItemFornecedor;
                        itemToChange.UsuarioAlteracaoId = usuario.Id;
                        itemToChange.FormaPagtoId = formaPagtoIdFornecedorChosen != null && formaPagtoIdFornecedorChosen > 0 ? formaPagtoIdFornecedorChosen : itemTroca.FornecedorPagtoIdChosen;
                        itemToChange.AprovacaoFornecedor = false;
                        itemToChange.Ativo = true;

                        _itemPedidoRep.Edit(itemToChange);
                        _unitOfWork.Commit();

                        _utilServiceRep.EnviaEmailSmsNovoPedidoItemfornecedor(_pedido.Id, itemTroca.ItemId, usuario, existePedParaAprovar);
                    }

                }

                if (_pedido.StatusSistemaId == 36 || _pedido.StatusSistemaId == 29)
                {
                    _pedido.StatusSistemaId = 24;
                    _pedidoRep.Edit(_pedido);
                    _unitOfWork.Commit();
                }



                response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }


        [HttpPost]
        [Route("salvaTrocaFornecedor")]
        public HttpResponseMessage SalvaTrocaFornecedor(HttpRequestMessage request, TrocaItemFornecedorViewModel itemTroca)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


                var itemToChange = _itemPedidoRep.GetSingle(itemTroca.ItemId);

                itemToChange.Observacao = itemTroca.Observacao;
                itemToChange.FornecedorId = itemTroca.FornecedorId;
                itemToChange.PrecoNegociadoUnit = itemTroca.ValorItemFornecedor;
                itemToChange.UsuarioAlteracaoId = usuario.Id;
                _itemPedidoRep.Edit(itemToChange);
                _unitOfWork.Commit();


                response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }



    }
}