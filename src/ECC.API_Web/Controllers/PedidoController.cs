using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using ECC.API_Web.Hubs;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System.Data.SqlClient;
using System.Data;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.API_Web.Requests;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.EntidadeStatus;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeSms;
using ECC.EntidadeAvisos;
using ECC.EntidadeEmail;
using ECC.EntidadeEndereco;
using ECC.Servicos;
using ECC.Entidades.EntidadePessoa;
using ECC.EntidadeParametroSistema;
using ECC.Servicos.ModelService;
using ECC.EntidadeFornecedor;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.Authorize(Roles = "Admin, Fornecedor, Membro")]
    [RoutePrefix("api/pedido")]
    public class PedidoController : ApiControllerBase
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
        private readonly IEntidadeBaseRep<Cotacao> _cotacaoRep;
        private readonly IEntidadeBaseRep<Sms> _smsRep;
        private readonly IEntidadeBaseRep<Emails> _emailRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmail;
        private readonly IEntidadeBaseRep<FornecedorRegiao> _fornecedorRegiaoRep;
        private readonly IEntidadeBaseRep<Endereco> _enderecoRep;
        private readonly IUtilService _ultilService;
        private readonly ICalendarioFeriadoService _calendarioFeriadoService;
        private readonly INotificacoesAlertasService _notificacacoesAlertasService;
        private readonly IPagamentoService _pagamentoService;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;
        private readonly IEntidadeBaseRep<RemoveFornPedido> _removeFornPedidoRep;
        private readonly IEntidadeBaseRep<IndisponibilidadeProduto> _indisponibilidadeProdutoRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IEntidadeBaseRep<ResultadoCotacao> _resultadoCotacaoRep;
        private readonly IEntidadeBaseRep<FornecedorFormaPagtoMembro> _fornecedorFormaPagtoMembro;



        public PedidoController(
            IEntidadeBaseRep<Produto> produtoRep,
            IUtilService ultilService,
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
            IEntidadeBaseRep<Cotacao> cotacaoRep,
            IEntidadeBaseRep<Sms> smsRep,
            IEntidadeBaseRep<Emails> emailRep,
            IEntidadeBaseRep<TemplateEmail> templateEmail,
            IEntidadeBaseRep<FornecedorRegiao> fornecedorRegiaoRep,
            IEntidadeBaseRep<ProdutoPromocional> produtoPromocionalRep,
            IEntidadeBaseRep<Endereco> enderecoRep,
            IPagamentoService pagamentoService,
            IEntidadeBaseRep<Avisos> avisosRep,
            IEntidadeBaseRep<RemoveFornPedido> removeFornPedidoRep,
            IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
            IEntidadeBaseRep<IndisponibilidadeProduto> indisponibilidadeProdutoRep,
            IEntidadeBaseRep<ResultadoCotacao> resultadoCotacaoRep,
            IEntidadeBaseRep<FornecedorFormaPagtoMembro> fornecedorFormaPagtoMembro,

        ICalendarioFeriadoService calendarioFeriadoService,
            INotificacoesAlertasService notificacoesAlertasService,
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
            _emailRep = emailRep;
            _statusSisRep = statusSisRep;
            _ultilService = ultilService;
            _produtoRep = produtoRep;
            _itemPedidoRep = itemPedidoRep;
            _avaliacaoFornecedorRep = avaliacaoFornecedorRep;
            _membroCategoriaRep = membroCategoriaRep;
            _cotacaoRep = cotacaoRep;
            _smsRep = smsRep;
            _templateEmail = templateEmail;
            _fornecedorRegiaoRep = fornecedorRegiaoRep;
            _enderecoRep = enderecoRep;
            _pagamentoService = pagamentoService;
            _avisosRep = avisosRep;
            _removeFornPedidoRep = removeFornPedidoRep;
            _parametroSistemaRep = parametroSistemaRep;
            _notificacacoesAlertasService = notificacoesAlertasService;
            _indisponibilidadeProdutoRep = indisponibilidadeProdutoRep;
            _calendarioFeriadoService = calendarioFeriadoService;
            _resultadoCotacaoRep = resultadoCotacaoRep;
            _fornecedorFormaPagtoMembro = fornecedorFormaPagtoMembro;
        }

        [HttpPost]
        [Route("inserirPedido")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, InserirPedidoRequest pedidoRequest)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {

                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var membro = _membroRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);



                    if (membro.MembroFornecedores.Count == 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, new { ErrorMessage = "Nenhum fornecedor vinculado, faça uma solicitação para comprar com novos fornecedores." });
                        return response;
                    }

                    // na implantação do sistema terá q trocar o WorkflowStatusId id pois vai mudar o ID
                    var statusId = _statusSisRep.FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 1).Id;

                    var dtCotacao = pedidoRequest.DtCotacao?.ToDate();

                    if (dtCotacao == null)
                    {
                        var date = DateTime.Now;

                        //Impede que uma cotação seja aberta depois das 11 horas da manhã e finais de semana
                        while (date.Hour <= 11 || date.DayOfWeek == DayOfWeek.Saturday ||
                               date.DayOfWeek == DayOfWeek.Sunday)
                            date = date.AddDays(1);

                        dtCotacao = new DateTime(date.Year, date.Month, date.Day, 11, 0, 0);
                    }

                    var novoPedido = new Pedido
                    {
                        DtCotacao = dtCotacao.Value,
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        DtPedido = DateTime.Now,
                        FlgCotado = false,
                        StatusSistemaId = statusId,
                        Membro = membro,
                        Ativo = true,
                        EnderecoId = pedidoRequest.EnderecoId

                    };
                    _pedidoRep.Add(novoPedido);

                    foreach (var item in pedidoRequest.Items)
                    {
                        var novoItemPedido = new ItemPedido
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Quantidade = item.quantity,
                            PrecoNegociadoUnit = 0,
                            PrecoMedioUnit = 0,
                            Produto = _produtoRep.GetSingle(item.sku),
                            ProdutoId = item.sku,
                            EntregaId = 1,
                            AprovacaoFornecedor = false,
                            AprovacaoMembro = true,
                            Ativo = true,
                            FlgOutraMarca = item.flgOutraMarca
                        };

                        novoPedido.ItemPedidos.Add(novoItemPedido);



                    }

                    _unitOfWork.Commit();

                    //Inserir Histórico Pedido
                    var pedidoHistorico = new HistStatusPedido
                    {
                        Ativo = true,
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Pedido = novoPedido,
                        PedidoId = novoPedido.Id,
                        StatusSistemaId = 21
                    };
                    //Status do Pedido Gerado
                    _histStatusPedido.Add(pedidoHistorico);


                    //Remove Fornecedor caso não tenda região
                    List<int> FornForadaArea = new List<int>();
                    var FornecedorAtendeId = _membroFornecedorRep.GetAll().Where(m => m.MembroId == membro.Id).Select(s => s.FornecedorId).ToList();
                    var EnderecoPedidoCidadeId = _enderecoRep.GetAll().FirstOrDefault(w => w.Id == novoPedido.EnderecoId).CidadeId;
                    foreach (var fa in FornecedorAtendeId)
                    {
                        var fornRegiao = _fornecedorRegiaoRep.GetAll().Where(w => w.FornecedorId == fa
                            && w.CidadeId == EnderecoPedidoCidadeId).Any();
                        if (!fornRegiao)
                            FornForadaArea.Add(fa);

                    }

                    foreach (var itemPed in novoPedido.ItemPedidos)
                    {
                        foreach (var itemFornFora in FornForadaArea)
                        {
                            pedidoRequest.RemFornPedCot.Add(new RemFornPedCotViewModel { forn = itemFornFora, prd = itemPed.ProdutoId, viaSistema = true });
                        }
                        foreach (var item in pedidoRequest.RemFornPedCot)
                        {
                            if (itemPed.ProdutoId == item.prd)
                            {

                                var removeFornPedido = new RemoveFornPedido
                                {
                                    Ativo = !item.viaSistema,
                                    UsuarioCriacao = usuario,
                                    DtCriacao = DateTime.Now,
                                    PedidoId = novoPedido.Id,
                                    FonecedorId = item.forn,
                                    ProdutoId = item.prd,
                                    ItemPedidoId = itemPed.Id
                                };

                                _removeFornPedidoRep.Add(removeFornPedido);
                            }
                        }


                    }


                    _unitOfWork.Commit();

                    //Enviar notificação para o topo do ADM através do SignalRHub do total de pedidos para cotar
                    var totalPed = _pedidoRep.All.Count(x => x.DtCotacao <= DateTime.Now && x.FlgCotado == false);
                    var _context = GlobalHost.ConnectionManager.GetHubContext<NotificacoesHub>();
                    _context.Clients.All.getTotalPed(totalPed);

                    //apenas chama uma function via signalR que vai carregar monitor da boa compra novamente
                    var contextPrdG = GlobalHost.ConnectionManager.GetHubContext<NotificacoesHub>();
                    contextPrdG.Clients.All.getProdGroup("", "");
                    //rubio teste 2 a
                    //Envia email para o membro que o pedido foi gerado
                    _ultilService.EnviaEmailPedido(novoPedido.Id, 1, usuario);

                    // Update view model
                    response = request.CreateResponse(HttpStatusCode.OK, novoPedido.DtCotacao.AddDays(1));

                }

                return response;
            });

        }


        [HttpPost]
        [Route("inserirPedidoPromocional/{enderecoId:int}")]
        public HttpResponseMessage InserirPedidoPromocional(HttpRequestMessage request, int enderecoId, List<ItemPedidoViewModel> items)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<ItemPedidoViewModel> pedNaoGerados = new List<ItemPedidoViewModel>();
                List<ItemPedidoViewModel> promocaoForaValidade = new List<ItemPedidoViewModel>();
                var novoPedido = new Pedido();
                var pedGerados = new List<PedidoViewModel>();
                var pedGeradosPF = new List<PedidoPFViewModel>();


                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    Membro membro = _membroRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                    int statusId = _statusSisRep.FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 4).Id;

                    var agrupaFornecedorPagamento = items
                        .GroupBy(x => new { x.FornecedorId, x.FormaPagtoId })
                        .Select(c =>
                        new
                        {
                            c.Key.FornecedorId,
                            c.Key.FormaPagtoId,
                            ProdutosId = c.Select(p => p.sku).ToList()
                        })
                        .ToList();

                    foreach (var pedidos in agrupaFornecedorPagamento)
                    {
                        var idProdutosFornecedor = pedidos.ProdutosId.ToList();

                        var usuarioId = _produtoRep
                        .FirstOrDefault(p => p.ProdutoPromocional.FornecedorId == pedidos.FornecedorId && idProdutosFornecedor.Contains(p.Id))
                        .ProdutoPromocional.UsuarioCriacaoId;

                        var usuarioFornecedor = _usuarioRep.FirstOrDefault(x => x.Id == usuarioId.Value);
                        var item = new ItemPedidoViewModel();
                        novoPedido = new Pedido();

                        novoPedido.UsuarioCriacao = usuario;
                        novoPedido.DtCriacao = DateTime.Now;
                        novoPedido.DtPedido = DateTime.Now;
                        novoPedido.FlgCotado = true;
                        novoPedido.StatusSistemaId = statusId;
                        novoPedido.Membro = membro;
                        novoPedido.Ativo = true;
                        novoPedido.EnderecoId = enderecoId;
                        novoPedido.DtCotacao = DateTime.Now;

                        _pedidoRep.Add(novoPedido);
                        _unitOfWork.Commit();

                        foreach (var prod in pedidos.ProdutosId)
                        {
                            DateTime dataHoje = DateTime.Now;
                            var produto = _produtoRep.GetSingle(prod);
                            DateTime dataFimProdutoPromocao = produto.ProdutoPromocional.FimPromocao;
                            item = items.FirstOrDefault(x => x.sku == prod);

                            if (dataHoje < dataFimProdutoPromocao)
                            {
                                if (produto.ProdutoPromocional.QuantidadeProduto >= item?.quantity)
                                {
                                    var novoItemPedido = new ItemPedido
                                    {
                                        UsuarioCriacao = usuario,
                                        DtCriacao = DateTime.Now,
                                        Quantidade = item.quantity,
                                        PrecoNegociadoUnit = item.PrecoNegociadoUnit,
                                        PrecoMedioUnit = item.PrecoMedioUnit,
                                        FormaPagtoId = item.FormaPagtoId,
                                        FornecedorId = produto.ProdutoPromocional.FornecedorId,
                                        FornecedorUsuarioId = usuarioFornecedor.Id,
                                        Produto = _produtoRep.GetSingle(item.sku),
                                        PedidoId = novoPedido.Id,
                                        ProdutoId = item.sku,
                                        EntregaId = 1,
                                        AprovacaoMembro = true,
                                        Desconto = item.Desconto,
                                        Ativo = true
                                    };
                                    novoPedido.ItemPedidos.Add(novoItemPedido);

                                    //Atualiza a quantidade de produtos promocionais disponíveis para venda
                                    produto.ProdutoPromocional.QuantidadeProduto -= item.quantity;
                                    produto.ProdutoPromocional.DtAlteracao = DateTime.Now;
                                    _produtoRep.Edit(produto);
                                    _unitOfWork.Commit();

                                    var produtoPromocional = novoItemPedido.Produto;
                                    ProdutoPromocionalViewModel produtoVM = Mapper.Map<Produto, ProdutoPromocionalViewModel>(produtoPromocional);

                                    //Chama o SignalR para atualizar a quantidade de produtos promocionais disponíveis para venda na tela de promoções de membros
                                    var _context = GlobalHost.ConnectionManager.GetHubContext<NotificacoesHub>();
                                    _context.Clients.All.pesquisarProdutoPromocao(produtoVM);
                                }
                                else
                                {
                                    //Pedidos não Gerados por não ter quantidade em estoque.
                                    pedNaoGerados.Add(item);
                                }
                            }
                            else
                            {
                                //Pedidos não Gerados por estarem fora da validade da Promoção.
                                promocaoForaValidade.Add(item);
                            }

                        }

                        //Inserir Histórico Pedido
                        HistStatusPedido pedidoHistorico = new HistStatusPedido();
                        pedidoHistorico.Ativo = true;
                        pedidoHistorico.UsuarioCriacao = usuario;
                        pedidoHistorico.DtCriacao = DateTime.Now;
                        pedidoHistorico.Pedido = novoPedido;
                        pedidoHistorico.PedidoId = novoPedido.Id;
                        pedidoHistorico.StatusSistemaId = 21; //Status do Pedido Gerado

                        _histStatusPedido.Add(pedidoHistorico);
                        _unitOfWork.Commit();

                        #region Enviar Email, Sms e Notificação para fornecedor

                        //Envia email para o membro que o pedido foi gerado
                        _ultilService.EnviaEmailPedidoPromocao(novoPedido.Id, usuario);

                        //Inserir SMS na tabela de pedido Finalizado
                        Sms sms = new Sms
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Numero = usuarioFornecedor.Telefones.Select(t => t.DddCel).FirstOrDefault() +
                            usuarioFornecedor.Telefones.Select(t => t.Celular).FirstOrDefault(),
                            Mensagem = "Economiza Já - Pedido Promocional. Aguardando Aprovação do Pedido " + novoPedido.Id + ".",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.PedidoPromocionalPendenteAprovacao,
                            Ativo = true
                        };
                        _smsRep.Add(sms);


                        //Inserir aviso para fornecedor aprovar pedido promocional
                        Avisos novoAviso = new Avisos();

                        novoAviso.UsuarioCriacaoId = usuario.Id;
                        novoAviso.DtCriacao = DateTime.Now;
                        novoAviso.Ativo = true;
                        novoAviso.IdReferencia = novoPedido.Id;
                        novoAviso.DataUltimoAviso = DateTime.Now;
                        novoAviso.ExibeNaTelaAvisos = true;
                        novoAviso.TipoAvisosId = (int)TipoAviso.AprovarPedidoPromocional; //Id Aviso
                        novoAviso.URLPaginaDestino = "/#/aprovacaopromocao";
                        novoAviso.TituloAviso = "Pedido promocional pendente de aprovação";
                        novoAviso.ToolTip = "Pedido promocional pendente de aprovação";
                        string descAviso = "Pedido promocional " + novoPedido.Id + " pendente de aprovação";
                        novoAviso.DescricaoAviso = descAviso.Length > 99 ? descAviso.Substring(0, 99) : descAviso;
                        novoAviso.ModuloId = 4; //Modulo Fornecedor
                        novoAviso.UsuarioNotificadoId = usuario.Id;
                        _avisosRep.Add(novoAviso);

                        #endregion

                        _unitOfWork.Commit();


                        if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                        {

                            //Pedidos Gerados.
                            var pedidoVM = Mapper.Map<Pedido, PedidoViewModel>(novoPedido);



                            pedGerados.Add(pedidoVM);
                        }
                        else
                        {

                            var pedidoVM = Mapper.Map<Pedido, PedidoPFViewModel>(novoPedido);
                            pedGeradosPF.Add(pedidoVM);
                        }

                    }


                    if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                    {

                        response = request.CreateResponse(HttpStatusCode.OK, new { pedidosGerados = pedGerados, pedidosNaoGerados = pedNaoGerados, pedidosForaValidade = promocaoForaValidade });
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { pedidosGerados = pedGeradosPF, pedidosNaoGerados = pedNaoGerados, pedidosForaValidade = promocaoForaValidade });

                    }
                }

                return response;

            });

        }

        [HttpGet]
        [Route("pedidosMembro/{statusId:int=0}/{page:int=0}/{pageSize=4}/{idPedido:int=0}/{dtDe?}/{dtAte?}")]
        public HttpResponseMessage PedidosMembro(HttpRequestMessage request, int? statusId, int? page, int? pageSize, int? idPedido, string dtDe = null, string dtAte = null)
        {
            var currentPage = page.Value;
            var currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var statusSistema = _statusSisRep.GetAll().Where(x => x.Ativo).ToList();

                var fornecedorFormaPagtoRemove = _fornecedorFormaPagtoMembro.FindBy(x => x.MembroId == membro.Id).Select(x => new { x.FornecedorFormaPagto.FormaPagtoId, x.FornecedorId });
                var ids = new List<int>();

                List<Pedido> pedidos = new List<Pedido>();

                var cidade = membro.Pessoa.Enderecos.Select(c => c.CidadeId).FirstOrDefault();

                if (idPedido > 0)
                {

                    if (usuario.Id == 37)
                    {
                        pedidos = _pedidoRep.FindBy(x =>
                         x.ItemPedidos.Any(p => p.Produto.ProdutoPromocionalId == null) &&
                       x.Id == idPedido)
                       .ToList();
                    }
                    else
                    {
                        pedidos = _pedidoRep.FindBy(x => x.Membro.Id.Equals(membro.Id)
                                               && x.ItemPedidos.Any(p => p.Produto.ProdutoPromocionalId == null) &&
                                               x.Id == idPedido)
                                               .ToList();
                    }


                }
                else
                {
                    if (statusId != null && statusId > 0)
                    {
                        pedidos = _pedidoRep.FindBy(x => x.Membro.Id.Equals(membro.Id)
                               && x.ItemPedidos.Any(p => p.Produto.ProdutoPromocionalId == null))
                               .ToList();

                        if (!string.IsNullOrEmpty(dtDe))
                        {
                            var dt = dtDe.ToDate();
                            if (dt.HasValue)
                            {
                                var data = dt.Value.AddHoraIni();
                                pedidos = pedidos.Where(x => x.DtPedido >= data).ToList();
                            }
                        }
                        if (!string.IsNullOrEmpty(dtAte))
                        {
                            var dt = dtAte.ToDate();
                            if (dt.HasValue)
                            {
                                var data = dt.Value.AddHoraFim();
                                pedidos = pedidos.Where(x => x.DtPedido <= data).ToList();
                            }
                        }

                        if (statusId < 23)//Pedido Gerado
                            pedidos = pedidos.Where(x => x.StatusSistemaId < 23).ToList();
                        else if (statusId == 23)//Aguardando sua Aprovação
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 23).ToList();
                        else if (statusId == 24)//Aguardando aprovação do fornecedor
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 24).ToList();
                        else if (statusId > 24 && statusId < 30)//Aguardando Entrega
                            pedidos = pedidos.Where(x => x.StatusSistemaId > 24 && x.StatusSistemaId < 30).ToList();
                        else if (statusId == 30)//Finalizado
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 30).ToList();
                        else if (statusId == 35)//Itens Indisponíveis
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 35).ToList();
                        else if (statusId == 36)//Cancelado
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 36).ToList();
                    }
                }

                var totalPedidos = pedidos.Count;

                pedidos = pedidos.OrderByDescending(c => c.DtPedido)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize).ToList();

                var pedidosAbertos = pedidos.Where(a => a.StatusSistemaId == 23).ToList();

                var idsFornecedores = pedidosAbertos.Select(i => i.ItemPedidos
                .Where(x => x.FornecedorId != null)
                .Select(f => f.FornecedorId).ToList().Distinct()).ToList();

                var idFornecedores = (from f in idsFornecedores
                                      let ped = _pedidoRep.GetAll().Where(p => p.ItemPedidos
                                    .Select(i => i.FornecedorId).FirstOrDefault() == f.FirstOrDefault() &&
                                    p.StatusSistemaId != 23 && p.MembroId == membro.Id)
                                    .ToList()
                                      where ped.Count == 0
                                      select (int)f.FirstOrDefault()).ToList();


                var pedidosVM = Mapper.Map<IEnumerable<Pedido>, IEnumerable<PedidoViewModel>>(pedidos);

                var pedidoViewModels = pedidosVM as IList<PedidoViewModel> ?? pedidosVM.ToList();

                for (int i = 0; i < pedidoViewModels.Count; i++)
                {
                    if (pedidoViewModels[i].StatusId < 23)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 21).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 23)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 23).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 24)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 24).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId > 24 && pedidoViewModels[i].StatusId < 30)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 29).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 30)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 30).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 35)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 35).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 36).Select(e => e.DescStatus).FirstOrDefault();
                    }

                    #region Adiciona Prazos Semanais para os Fornecedores

                    for (int j = 0; j < pedidoViewModels[i].Itens.Count; j++)
                    {
                        List<FornecedorPrazoSemanalViewModel> prazoSemanal = new List<FornecedorPrazoSemanalViewModel>();

                        if (pedidoViewModels[i].Itens[j].Fornecedor != null)
                        {
                            // var regiaoSemana = pedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal
                            //.Where(x => x.CidadeId == cidade).ToList();
                            var regiaoDias = pedidoViewModels[i].Itens[j].Fornecedor.FornecedorRegiao
                                .FirstOrDefault(x => x.CidadeId == cidade);

                            //if (regiaoSemana.Any())
                            //{
                            //    pedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal.Clear();
                            //    pedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal.AddRange(regiaoSemana);
                            //    pedidoViewModels[i].Itens[j].Fornecedor.PrazoEntrega = 0;
                            //    pedidoViewModels[i].Itens[j].Fornecedor.VlPedidoMin =
                            //        regiaoSemana.FirstOrDefault(x => x.VlPedMinRegiao > 0).VlPedMinRegiao.ToString("C2");
                            //}
                            if (regiaoDias != null)
                            {
                                pedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal.Clear();

                                pedidoViewModels[i].Itens[j].Fornecedor.VlPedidoMin = regiaoDias.VlPedMinRegiao == 0 ?
                                    pedidoViewModels[i].Itens[j].Fornecedor.VlPedidoMin : regiaoDias.VlPedMinRegiao.ToString("C2");

                                pedidoViewModels[i].Itens[j].Fornecedor.PrazoEntrega = regiaoDias.Prazo;
                                pedidoViewModels[i].Itens[j].Fornecedor.FornecedorRegiao.Clear();
                                pedidoViewModels[i].Itens[j].Fornecedor.FornecedorRegiao.Add(regiaoDias);
                            }

                        }
                    }

                    #endregion

                    #region Adiciona Motivo Cancelamento Pedido

                    var idPed = pedidoViewModels[i].PedidoId.TryParseInt();
                    var historicoPedido = _histStatusPedido.FindBy(x => x.PedidoId == idPed && x.StatusSistemaId == 36).Select(x => new { x.DescMotivoCancelamento, x.PedidoId, UsuarioId = x.UsuarioCriacaoId, x.UsuarioCriacao.Pessoa.PessoaJuridica.NomeFantasia })
                        .ToList();

                    //var histPedidoVM = Mapper.Map<IEnumerable<HistStatusPedido>, IEnumerable<HistStatusPedidoViewModel>>(historicoPedido);

                    pedidoViewModels[i].ListaHistStatusPedido = historicoPedido;

                    #endregion


                    ids.AddRange(pedidoViewModels[i].Itens.Where(f => f.Fornecedor != null).Select(x => x.Fornecedor.Id).Distinct().ToList());

                }

                fornecedorFormaPagtoRemove = fornecedorFormaPagtoRemove.Where(x => ids.Distinct().Contains(x.FornecedorId));


                var pagSet = new PaginacaoConfig<PedidoViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalPedidos,
                    TotalPages = (int)Math.Ceiling((decimal)totalPedidos / currentPageSize),
                    Items = pedidoViewModels.OrderBy(x => x.StatusId != 23)
                    .ThenBy(x => !(x.StatusId > 23 && x.StatusId < 30))
                    .ThenBy(x => x.StatusId > 23)
                    .ThenByDescending(x => x.PedidoId).ToList()
                };
                var response = request.CreateResponse(HttpStatusCode.OK, new { pedidos = pagSet, fornecedores = idFornecedores, fornecedorFormaPagtoRemove });
                return response;
            });
        }


        [HttpGet]
        [Route("pedidosPromocaoMembro/{statusId:int=0}/{page:int=0}/{pageSize=4}/{idPedido:int=0}/{dtDe?}/{dtAte?}")]
        public HttpResponseMessage PedidosPromocaoMembro(HttpRequestMessage request, int? statusId, int? page, int? pageSize, int? idPedido, string dtDe = null, string dtAte = null)
        {
            var currentPage = page.Value;
            var currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var statusSistema = _statusSisRep.GetAll().Where(x => x.WorkflowStatusId == 12).ToList();
                List<Pedido> pedidos = new List<Pedido>();


                var cidade = membro.Pessoa.Enderecos.Select(c => c.CidadeId).FirstOrDefault();

                if (idPedido > 0)
                {
                    pedidos = _pedidoRep.GetAll().Where(x => x.Membro.Id.Equals(membro.Id) &&
                                x.ItemPedidos.Any(p => p.Produto.ProdutoPromocionalId != null) &&
                                x.Id == idPedido).ToList();

                }
                else
                {

                    if (statusId != null && statusId > 0)
                    {
                        pedidos = _pedidoRep.GetAll().Where(x => x.Membro.Id.Equals(membro.Id)
                        && x.ItemPedidos.Any(p => p.Produto.ProdutoPromocionalId != null))
                        .ToList();

                        if (!string.IsNullOrEmpty(dtDe))
                        {
                            var dt = dtDe.ToDate();
                            if (dt.HasValue)
                            {
                                var data = dt.Value.AddHoraIni();
                                pedidos = pedidos.Where(x => x.DtPedido >= data).ToList();
                            }
                        }
                        if (!string.IsNullOrEmpty(dtAte))
                        {
                            var dt = dtAte.ToDate();
                            if (dt.HasValue)
                            {
                                var data = dt.Value.AddHoraFim();
                                pedidos = pedidos.Where(x => x.DtPedido <= data).ToList();
                            }
                        }

                        #region Verificando se o pedido tem mais de dois meses.

                        //var dataHoje = DateTime.Now;
                        //var dataFinal = pedidos.Select(x => x.DtPedido).FirstOrDefault();
                        //var resultadoData = dataFinal.Subtract(dataHoje);
                        //var diasMes = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        //var diasMesSeguinte = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month + 1);

                        //if (resultadoData.Days > (diasMes + diasMesSeguinte))
                        //{
                        //    pedidos.Clear();
                        //}

                        #endregion

                        if (statusId < 24)//Pedido Gerado
                            pedidos = pedidos.Where(x => x.StatusSistemaId < 24).ToList();
                        else if (statusId == 24)//Aguardando Aprovação Fornecedor
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 24).ToList();
                        else if (statusId == 29)//Aguardando Entrega
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 29).ToList();
                        else if (statusId == 30)//Finalizado
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 30).ToList();
                        else if (statusId == 35)//Itens Indisponíveis
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 35).ToList();
                        else if (statusId == 36)//Cancelado
                            pedidos = pedidos.Where(x => x.StatusSistemaId == 36).ToList();
                    }
                }

                pedidos = pedidos.OrderByDescending(c => c.DtPedido)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize).ToList();


                var totalPedidos = pedidos.Count;

                var pedidosAbertos = pedidos.Where(a => a.StatusSistemaId == 23).ToList();

                var idsFornecedores = pedidosAbertos.Select(i => i.ItemPedidos.Select(f => f.FornecedorId).ToList().Distinct()).ToList();

                var idFornecedores = (from f in idsFornecedores let ped = _pedidoRep.GetAll().Where(p => p.ItemPedidos.Select(i => i.FornecedorId).FirstOrDefault() == f.FirstOrDefault() && p.StatusSistemaId != 23 && p.MembroId == membro.Id).ToList() where ped.Count == 0 select (int)f.FirstOrDefault()).ToList();

                var pedidosVM = Mapper.Map<IEnumerable<Pedido>, IEnumerable<PedidoViewModel>>(pedidos);

                var pedidoViewModels = pedidosVM as IList<PedidoViewModel> ?? pedidosVM.ToList();

                for (int i = 0; i < pedidoViewModels.Count(); i++)
                {
                    if (pedidoViewModels[i].StatusId < 23)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 21).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 24)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 24).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 29)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 29).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 30)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 30).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 35)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 35).Select(e => e.DescStatus).FirstOrDefault();
                    }
                    else if (pedidoViewModels[i].StatusId == 36)
                    {
                        pedidoViewModels[i].Status = statusSistema.Where(x => x.Id == 36).Select(e => e.DescStatus).FirstOrDefault();

                    }

                    #region Adiciona Prazos Semanais para os Fornecedores

                    for (int j = 0; j < pedidoViewModels[i].Itens.Count; j++)
                    {
                        if (pedidoViewModels[i].Itens[j].Fornecedor != null)
                        {
                            var prazoSemana = pedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal
                                .Where(x => x.CidadeId == cidade).ToList();

                            if (prazoSemana.Count > 0)
                            {
                                pedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal = prazoSemana;
                                pedidoViewModels[i].Itens[j].Fornecedor.VlPedidoMin =
                                    prazoSemana.Max(x => x.VlPedMinRegiao).ToString("C2");
                            }
                            else
                            {
                                pedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal.Clear();
                                var prazoDias = pedidoViewModels[i].Itens[j].Fornecedor.FornecedorRegiao
                                    .FirstOrDefault(x => x.CidadeId == cidade);
                                pedidoViewModels[i].Itens[j].Fornecedor.VlPedidoMin = prazoDias.VlPedMinRegiao.ToString("C2");
                                pedidoViewModels[i].Itens[j].Fornecedor.FornecedorRegiao.Clear();
                                pedidoViewModels[i].Itens[j].Fornecedor.FornecedorRegiao.Add(prazoDias);
                                pedidoViewModels[i].Itens[j].Fornecedor.PrazoEntrega = prazoDias.Prazo;
                            }
                        }
                    }

                    #endregion

                    #region Adiciona Motivo Cancelamento Pedido

                    var idPed = pedidoViewModels[i].PedidoId.TryParseInt();
                    var historicoPedido = _histStatusPedido.GetAll()
                    .Where(x => x.PedidoId == idPed && x.StatusSistemaId == 36)
                    .ToList();
                    var histPedidoVM = Mapper.Map<IEnumerable<HistStatusPedido>, IEnumerable<HistStatusPedidoViewModel>>(historicoPedido);

                    pedidoViewModels[i].ListaHistStatusPedido = histPedidoVM;

                    #endregion

                }

                var pagSet = new PaginacaoConfig<PedidoViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalPedidos,
                    TotalPages = (int)Math.Ceiling((decimal)totalPedidos / currentPageSize),
                    Items = pedidoViewModels.OrderBy(x => x.StatusId != 23)
                    .ThenBy(x => !(x.StatusId > 23 && x.StatusId < 30))
                    .ThenBy(x => x.StatusId > 23)
                    .ThenByDescending(x => x.PedidoId).ToList()
                };

                var response = request.CreateResponse(HttpStatusCode.OK, new { pedidos = pagSet, fornecedores = idFornecedores });

                return response;
            });
        }


        [HttpGet]
        [Route("totalPedido")]
        public HttpResponseMessage GetTotalPedido(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                //Enviar notificação para o painel do ADM através do SignalRHub
                var totalPed = _pedidoRep.All.Count(x => x.DtCotacao <= DateTime.Now && x.FlgCotado == false);

                var response = request.CreateResponse(HttpStatusCode.OK, new { totalPed });

                return response;
            });
        }


        [HttpGet]
        [Route("totalPedidoGroupAdm")]
        public HttpResponseMessage GetProdAgrupadosSemCotarAdm(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var produtos = _pedidoRep.FindBy(x => x.DtCotacao <= DateTime.Now && !x.FlgCotado)
                         .SelectMany(x => x.ItemPedidos)
                         .GroupBy(x => new
                         {
                             x.Produto.SubCategoria.Categoria.DescCategoria,
                             x.Produto.Marca.DescMarca,
                             x.Produto.UnidadeMedida.DescUnidadeMedida,
                             x.Produto.DescProduto,
                             x.ProdutoId,
                             Imagem = x.Produto.Imagens.Select(i => i.CaminhoImagem).FirstOrDefault(),
                             ImagemG = x.Produto.Imagens.Select(i => i.CaminhoImagemGrande).FirstOrDefault(),
                             subcategoria = x.Produto.SubCategoria.Id,
                             categoria = x.Produto.SubCategoria.Categoria.Id
                         })
                         .ToList();

                var prodsGroup = produtos.Select(g => new
                {
                    Especificacao = g.Select(t => t.Produto.Especificacao).FirstOrDefault(),
                    DescCategoria = g.Key.DescCategoria,
                    Marca = g.Key.DescMarca,
                    UnidadeMedida = g.Key.DescUnidadeMedida,
                    Id = g.Key.ProdutoId,
                    g.Key.DescProduto,
                    qtd = g.Sum(s => s.Quantidade),
                    g.Key.Imagem,
                    g.Key.subcategoria,
                    g.Key.categoria,
                    g.Key.ImagemG,
                    notas = g.Sum(f => f.Produto.Rankings.Sum(y => y.Nota)),
                    contNota = g.SelectMany(d => d.Produto.Rankings).Count()
                });

                var response = request.CreateResponse(HttpStatusCode.OK, new { prodsGroup, cam = ConfigurationManager.AppSettings[(Environment.GetEnvironmentVariable("Amb_EconomizaJa")) + "_CamImagensExibi"] });

                return response;
            });
        }


        [HttpGet]
        [Route("totalPedidoGroup/{dataCotacao?}")]
        public HttpResponseMessage GetProdAgrupadosSemCotar(HttpRequestMessage request, string dataCotacao = null)
        {
            return CreateHttpResponse(request, () =>
            {
                Random generator = new Random();

                //pega as categorias que o membro logado trabalha para exibir no monitor da boa compra somente produtos 
                //que pertence as categorias dele
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.FirstOrDefault(m => m.PessoaId == usuario.PessoaId);
                List<int> membroCategoria = _membroCategoriaRep.GetAll().Where(x => x.MembroId == membro.Id).Select(x => x.CategoriaId).ToList();
                var horaCotacao = Convert.ToDateTime(_parametroSistemaRep.FirstOrDefault(x => x.Codigo == "HORA_COTACAO").Valor).TimeOfDay;
                var response = new HttpResponseMessage();

                var dateCotaPedidos = string.IsNullOrEmpty(dataCotacao)
                    ? DateTime.Now
                    : dataCotacao.ToDate().Value.Add(horaCotacao);

                if (membro.FranquiaId != null)
                {
                    var produtos =
                  _pedidoRep.GetAll()
                  .Where(x => !x.FlgCotado && x.DtCotacao == dateCotaPedidos)
                          //faz o contains pegando somente as categorias que o membro esta vinculado
                          .SelectMany(x => x.ItemPedidos.Where(c => membroCategoria.Contains(c.Produto.SubCategoria.CategoriaId) &&
                          c.Produto.Franquias.Any(f => f.FranquiaId == membro.FranquiaId)))
                          .GroupBy(x => new
                          {
                              x.Produto.SubCategoria.Categoria.DescCategoria,
                              x.Produto.Marca.DescMarca,
                              x.Produto.UnidadeMedida.DescUnidadeMedida,
                              x.Produto.DescProduto,
                              x.ProdutoId,
                              Imagem = x.Produto.Imagens.Select(i => i.CaminhoImagem).FirstOrDefault(),
                              ImagemG = x.Produto.Imagens.Select(i => i.CaminhoImagemGrande).FirstOrDefault(),
                              subcategoria = x.Produto.SubCategoria.Id,
                              categoria = x.Produto.SubCategoria.Categoria.Id
                          })
                          .ToList();

                    var prodsGroup = produtos.Select(g => new
                    {
                        Especificacao = g.Select(t => t.Produto.Especificacao).FirstOrDefault(),
                        DescCategoria = g.Key.DescCategoria,
                        Marca = g.Key.DescMarca,
                        UnidadeMedida = g.Key.DescUnidadeMedida,
                        Id = g.Key.ProdutoId,
                        g.Key.DescProduto,
                        qtd = g.Sum(s => s.Quantidade),
                        g.Key.Imagem,
                        g.Key.subcategoria,
                        g.Key.categoria,
                        g.Key.ImagemG,
                        notas = g.Sum(f => f.Produto.Rankings.Sum(y => y.Nota)),
                        contNota = g.SelectMany(d => d.Produto.Rankings).Count(),
                        radomPercent = generator.Next(1, 85)

                    });

                    response = request.CreateResponse(HttpStatusCode.OK, new { prodsGroup, cam = ConfigurationManager.AppSettings[(Environment.GetEnvironmentVariable("Amb_EconomizaJa")) + "_CamImagensExibi"] });


                }
                else
                {
                    var produtos =
                   _pedidoRep.GetAll().Where(x => !x.FlgCotado && x.DtCotacao == dateCotaPedidos)
                           //faz o contains pegando somente as categorias que o membro esta vinculado
                           .SelectMany(x => x.ItemPedidos.Where(c => membroCategoria.Contains(c.Produto.SubCategoria.CategoriaId)))
                           .GroupBy(x => new
                           {
                               x.Produto.SubCategoria.Categoria.DescCategoria,
                               x.Produto.Marca.DescMarca,
                               x.Produto.UnidadeMedida.DescUnidadeMedida,
                               x.Produto.DescProduto,
                               x.ProdutoId,
                               Imagem = x.Produto.Imagens.Select(i => i.CaminhoImagem).FirstOrDefault(),
                               ImagemG = x.Produto.Imagens.Select(i => i.CaminhoImagemGrande).FirstOrDefault(),
                               subcategoria = x.Produto.SubCategoria.Id,
                               categoria = x.Produto.SubCategoria.Categoria.Id
                           })
                           .ToList();

                    var prodsGroup = produtos.Select(g => new
                    {
                        Especificacao = g.Select(t => t.Produto.Especificacao).FirstOrDefault(),
                        DescCategoria = g.Key.DescCategoria,
                        Marca = g.Key.DescMarca,
                        UnidadeMedida = g.Key.DescUnidadeMedida,
                        Id = g.Key.ProdutoId,
                        g.Key.DescProduto,
                        qtd = g.Sum(s => s.Quantidade),
                        g.Key.Imagem,
                        g.Key.subcategoria,
                        g.Key.categoria,
                        g.Key.ImagemG,
                        notas = g.Sum(f => f.Produto.Rankings.Sum(y => y.Nota)),
                        contNota = g.SelectMany(d => d.Produto.Rankings).Count(),
                        radomPercent = generator.Next(1, 85)
                    });

                    response = request.CreateResponse(HttpStatusCode.OK, new { prodsGroup, cam = ConfigurationManager.AppSettings[(Environment.GetEnvironmentVariable("Amb_EconomizaJa")) + "_CamImagensExibi"] });
                }

                return response;
            });
        }


        [HttpGet]
        [Route("totalPedidosPorData")]
        public HttpResponseMessage GetTotalPedidosPorData(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var datas = new List<string>();
                var horaCotacao = Convert.ToDateTime(_parametroSistemaRep.FirstOrDefault(x => x.Codigo == "HORA_COTACAO").Valor).TimeOfDay;

                var categorias =
                    _membroCategoriaRep.GetAll()
                        .Where(x => x.Membro.PessoaId == usuario.PessoaId)
                        .Select(c => c.CategoriaId).ToList();

                var dataPedidos = _pedidoRep.GetAll()
                .Where(x => x.FlgCotado == false && x.ItemPedidos.Any(p => categorias.Contains(p.Produto.SubCategoria.CategoriaId)))
                .Select(p => new { data = p.DtCotacao })
                .ToList();

                dataPedidos.ForEach(c => datas.Add(c.data.ToShortDateString()));

                var qtdPedsData = datas.GroupBy(x => x)
                .Select(b => new { data = b.Key, qtd = b.Count() })
                .OrderBy(x => x.data);

                var response = request.CreateResponse(HttpStatusCode.OK,
                    new
                    {
                        listaQtdPedData = qtdPedsData,
                        DataHojeServidor = DateTime.Now,
                        HoraCotacao = horaCotacao
                    });

                return response;
            });
        }

        [HttpGet]
        [Route("atualizaFlgCotado")]
        public HttpResponseMessage AtualizaFlgCotadoPedido(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                // na implantação do sistema terá q trocar o WorkflowStatusId id pois vai mudar o ID
                var statusId = _statusSisRep.FirstOrDefault(x => x.WorkflowStatusId == 14 && x.Ordem == 1).Id;

                var pOut = new SqlParameter
                {
                    ParameterName = "@cotacaoId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                _pedidoRep.ExecuteWithStoreProcedure("stp_upd_pedidos_sem_cotar @cotacaoId out", pOut);

                var response = request.CreateResponse(HttpStatusCode.OK);

                //Resgata ID da cotação inserida
                var cotacaoID = pOut.Value.TryParseInt();

                //Historico de Cotação
                var cotacaoHistorico = new HistStatusCotacao
                {
                    Ativo = true,
                    CotacaoId = cotacaoID,
                    DtCriacao = DateTime.Now,
                    UsuarioCriacao = usuario,
                    StatusSistemaId = statusId
                };

                _histStatusCotacao.Add(cotacaoHistorico);

                var cot = _cotacaoRep.GetSingle(cotacaoID);
                var naService = new NotificacoesAlertasService();
                var listaUsuario = naService.TrataNovaCotacaoFornecedor(cot, 7);

                foreach (var retAviso in listaUsuario)
                {
                    naService.AddAvisos(
                                new Avisos
                                {
                                    Ativo = true,
                                    IdReferencia = cot.Id,
                                    DataUltimoAviso = DateTime.Now,
                                    ExibeNaTelaAvisos = true,
                                    TipoAvisosId = (int)TipoAviso.NovaCotacao, //Id Tipo aviso para esse aviso
                                    URLPaginaDestino = "/#/cotacoes",
                                    TituloAviso = "Cotação Pendente de Resposta",
                                    ToolTip = "Cotação Pendente de Resposta",
                                    DescricaoAviso = "Cotação " + cot.Id + " Pendente de Resposta",
                                    ModuloId = 4, //Modulo Fornecedor
                                    UsuarioNotificadoId = retAviso.Usuario.Id
                                });
                }

                _unitOfWork.Commit();

                var usuarios = listaUsuario.Select(x => x.Usuario).Distinct();

                new NotificacoesHub(this._unitOfWork).NotificaNovaCotacao(new GeraCotacaoViewModel { CotacaoId = cotacaoID, ListaTokenUsuarios = usuarios.Select(x => x.TokenSignalR).ToList() });

                return response;
            });
        }

        [HttpPost]
        [Route("aprovar")]
        public HttpResponseMessage Aprovar(HttpRequestMessage request, List<ItemPedidoViewModel> itemPedidoVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                int idPedido = itemPedidoVm[0].PedidoId;

                var pedido = _pedidoRep.GetSingle(idPedido);

                var itensFornecedores = _itemPedidoRep.GetAll().Where(i => i.PedidoId == idPedido).ToList();

                foreach (var item in itemPedidoVm)
                {
                    //Alterando Forma de Pagamento do Ítem de pedido.
                    var itensFornecedoresSelecionados = itensFornecedores.FirstOrDefault(f => f.FornecedorId == item.FornecedorId && f.Id == item.Id);

                    itensFornecedoresSelecionados.FormaPagtoId = item.FormaPagtoId;
                    itensFornecedoresSelecionados.UsuarioAlteracao = usuario;
                    itensFornecedoresSelecionados.DtAlteracao = DateTime.Now;
                    itensFornecedoresSelecionados.Quantidade = item.quantity;
                    itensFornecedoresSelecionados.Desconto = item.Desconto;
                    itensFornecedoresSelecionados.TaxaEntrega = item.TaxaEntrega;

                    _itemPedidoRep.Edit(itensFornecedoresSelecionados);

                }
                var itensSemFornecedor = itensFornecedores.Where(f => f.FornecedorId == null).ToList();

                foreach (var i in itensSemFornecedor)
                {
                    i.Observacao = "Sem estoque";
                    //i.AprovacaoFornecedor = true;
                    i.AprovacaoFornecedor = false;
                    i.UsuarioAlteracao = usuario;
                    i.DtAlteracao = DateTime.Now;
                    i.Ativo = false;
                    _itemPedidoRep.Edit(i);
                }
                _unitOfWork.Commit();

                //Limpar Aviso
                var naService = new NotificacoesAlertasService();
                naService.RemoverAvisoMembroPedido(usuario.PessoaId, idPedido, (int)TipoAviso.PedidoPendentedeAceiteMembro);

                #region Envia EMAIL E SMS para fornecedor

                var fornecedoresUsuarios = itensFornecedores.Where(x => x.Ativo && x.AprovacaoMembro)
                .SelectMany(f => f.Fornecedor.Pessoa.Usuarios).Distinct();

                foreach (var usuariofornecedor in fornecedoresUsuarios)
                {
                    if (naService.PodeEnviarNotificacao(usuariofornecedor.Id, TipoAviso.PedidoPendentedeAceiteFornecedor.TryParseInt(), TipoAlerta.EMAIL))
                    {


                        if (usuariofornecedor.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                        {
                            var templateEmail = _templateEmail.FirstOrDefault(x => x.Id == 18).Template.Replace("#NomeFantasia#", usuariofornecedor.Pessoa.PessoaJuridica.NomeFantasia);

                            Emails emails = new Emails
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                AssuntoEmail = "Aguardando Aprovação do Pedido " + idPedido + ".",
                                EmailDestinatario = usuariofornecedor.UsuarioEmail,
                                CorpoEmail = templateEmail.Trim(),
                                Status = Status.NaoEnviado,
                                Origem = Origem.MembroAprovaPedido,
                                Ativo = true
                            };

                            _emailRep.Add(emails);
                        }
                        else
                        {
                            var templateEmail = _templateEmail.FirstOrDefault(x => x.Id == 18).Template.Replace("#NomeFantasia#", usuariofornecedor.Pessoa.PessoaFisica.Nome);

                            Emails emails = new Emails
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                AssuntoEmail = "Aguardando Aprovação do Pedido " + idPedido + ".",
                                EmailDestinatario = usuariofornecedor.UsuarioEmail,
                                CorpoEmail = templateEmail.Trim(),
                                Status = Status.NaoEnviado,
                                Origem = Origem.MembroAprovaPedido,
                                Ativo = true
                            };
                            _emailRep.Add(emails);
                        }

                    }

                    if (naService.PodeEnviarNotificacao(usuariofornecedor.Id, TipoAviso.PedidoPendentedeAceiteFornecedor.TryParseInt(), TipoAlerta.SMS))
                    {
                        Sms sms = new Sms
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Numero = usuariofornecedor.Telefones.Select(t => t.DddCel).FirstOrDefault() + usuariofornecedor.Telefones.Select(t => t.Celular).FirstOrDefault(),
                            Mensagem = "Economiza Já - Corra e aprove seu pedido " + idPedido + " , vamos faturar, evite o cancelamento.",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.MembroAprovaPedido,
                            Ativo = true
                        };

                        _smsRep.Add(sms);
                    }

                    //Inseri aviso para o fornecedor aprovar o pedido
                    if (usuario.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                    {
                        _ultilService.InserirAvisos(usuariofornecedor,
                                       TipoAviso.PedidoPendentedeAceiteFornecedor,
                                       "Pedido pendente de aprovação",
                                       "Pedido " + idPedido + " pendente de aprovação",
                                       "Pedido pendente de aprovação",
                                       "/#/pedidos",
                                       4,
                                       idPedido);
                    }

                    else
                    {
                        _ultilService.InserirAvisos(usuariofornecedor,
                                      TipoAviso.PedidoPendentedeAceiteFornecedor,
                                      "Pedido pendente de aprovação",
                                      "Pedido " + idPedido + " pendente de aprovação",
                                      "Pedido pendente de aprovação",
                                      "/#/pedidospessoafisica",
                                      4,
                                      idPedido);
                    }


                }

                #endregion

                //Atualiza para o próximo status.
                int statusId = _statusSisRep.FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 4).Id;

                pedido.StatusSistemaId = statusId;
                pedido.UsuarioAlteracao = usuario;
                pedido.DtAlteracao = DateTime.Now;
                _pedidoRep.Edit(pedido);

                _unitOfWork.Commit();

                #region Envia Notificação para Fornecedores

                var notificacaoHub = new NotificacoesHub();
                notificacaoHub.CarregaAvisosHub(fornecedoresUsuarios);

                #endregion

                var pedidoVM = Mapper.Map<Pedido, PedidoViewModel>(pedido);

                response = request.CreateResponse(HttpStatusCode.OK, pedidoVM);

                return response;
            });
        }


        [HttpPost]
        [Route("cancelarPedido/{pedidoId:int=0}")]
        public HttpResponseMessage CancelarPedido(HttpRequestMessage request, int pedidoId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var pedido = _pedidoRep.GetSingle(pedidoId);

                var itensPedido = pedido.ItemPedidos;

                foreach (var item in itensPedido)
                {
                    item.FormaPagtoId = item.FormaPagtoId;
                    item.UsuarioAlteracao = usuario;
                    item.DtAlteracao = DateTime.Now;
                    item.Ativo = false;

                    _itemPedidoRep.Edit(item);
                }

                _unitOfWork.Commit();

                //Limpar Aviso
                var naService = new NotificacoesAlertasService();
                naService.RemoverAvisoMembroPedido(usuario.PessoaId, pedidoId,
                    (int)TipoAviso.PedidoPendentedeAceiteMembro);


                //Atualiza para o próximo status.
                int statusId = _statusSisRep.GetAll().FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 10 && x.Ativo).Id;

                pedido.StatusSistemaId = statusId;
                pedido.UsuarioAlteracao = usuario;
                pedido.DtAlteracao = DateTime.Now;

                _pedidoRep.Edit(pedido);

                _unitOfWork.Commit();

                var pedidoVM = Mapper.Map<Pedido, PedidoViewModel>(pedido);

                response = request.CreateResponse(HttpStatusCode.OK, pedidoVM);

                return response;
            });
        }


        [HttpPost]
        [Route("itensPedidoEntregue")]
        public HttpResponseMessage ItensPedidoEntregue(HttpRequestMessage request, List<ItemPedidoViewModel> itenspedidoVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var idPedido = itenspedidoVm?.FirstOrDefault().PedidoId;
                var pedido = _pedidoRep.FirstOrDefault(x => x.Id == idPedido);

                //Pegamos os ítens de pedidos que foram aprovados.
                var itensPedido = pedido.ItemPedidos
                    .Where(x => x.AprovacaoFornecedor && x.Ativo && x.FornecedorId == fornecedor.Id).ToList();

                foreach (var item in itensPedido)
                {
                    item.UsuarioAlteracao = usuario;
                    item.DataEntregaFornecedor = DateTime.Now;
                    item.EntregaId = 2;
                    _itemPedidoRep.Edit(item);
                }

                #region Envia EMAIL
                var usuariosMembro = pedido.Membro.Pessoa.Usuarios;

                foreach (var usuarioMembro in usuariosMembro)
                {
                    if (_notificacacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, (int)TipoAviso.PedidoEntregueFornecedor, TipoAlerta.EMAIL))
                    {
                        _ultilService.EnviaEmailPedido(pedido.Id, 5, usuario);
                    }
                }




                #endregion

                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.OK, new { success = true, pedidoId = pedido.Id });

                return response;
            });
        }


        /// <summary>
        /// Verifica se todos os ítens já foram entregue.
        /// Em caso positivo atualizamos o status do pedido.
        /// </summary>
        /// <param name="pedido"></param>
        private void ValidarEntregaPedido(List<ItemPedido> itensPedido, Pedido pedido, Usuario usuario)
        {
            //Verifica se existem pedidos que não foram entregues.
            // Se todos forem entregues atualizamos o status.
            if (!itensPedido.Any(x => x.EntregaId == 1))
            {
                //Atualiza para o próximo status.
                int statusId = _statusSisRep.GetAll().FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 8).Id;
                pedido.StatusSistemaId = statusId;
                pedido.DtAlteracao = DateTime.Now;
                pedido.UsuarioAlteracao = usuario;

                _pedidoRep.Edit(pedido);
            }

        }

        [HttpGet]
        [Route("pedidosFornecedorAndamento/{page:int=0}/{pageSize=4}/{dtDe?}/{dtAte?}/{pedidoId:int=0}")]
        public HttpResponseMessage PedidosFornecedorAndamento(HttpRequestMessage request, int? page, int? pageSize, string dtDe = null, string dtAte = null, int? pedidoId = 0)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                List<Pedido> pedidos = null;
                HttpResponseMessage response = null;
                var totalPedidos = new int();

                // Caso não seja informado a data, pegamos os pedidos de 30 dias.
                dtDe = string.IsNullOrEmpty(dtDe) ? DateTime.MinValue.AddDays(0.1).ToString() : dtDe;
                dtAte = string.IsNullOrEmpty(dtAte) ? DateTime.Now.ToString() : dtAte;

                DateTime? DtIni = dtDe.ToDate().Value.AddHoraIni();
                DateTime? DtFim = dtAte.ToDate().Value.AddHoraFim();

                var queryItensPedido = new List<ItemPedido>();

                if (pedidoId > 0)
                {
                    queryItensPedido = _itemPedidoRep.GetAll()
                    .Where(x => x.FornecedorId == fornecedor.Id && x.AprovacaoFornecedor && x.Pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica &&
                     (x.Pedido.DtPedido >= DtIni.Value && x.Pedido.DtPedido <= DtFim.Value) &&
                     x.PedidoId == pedidoId && x.Pedido.StatusSistemaId >= 24)
                    .OrderByDescending(c => c.Pedido.DtPedido)
                    .ThenBy(o => o.PedidoId)
                    .ThenBy(o => o.Id)
                    .ToList();
                }
                else
                {
                    queryItensPedido = _itemPedidoRep.GetAll()
                    .Where(x => x.FornecedorId == fornecedor.Id && x.AprovacaoFornecedor && x.Pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica &&
                    (x.Pedido.DtPedido >= DtIni.Value && x.Pedido.DtPedido <= DtFim.Value) &&
                    x.Pedido.StatusSistemaId >= 24)
                    .OrderByDescending(c => c.Pedido.DtPedido)
                    .ThenBy(o => o.PedidoId)
                    .ThenBy(o => o.Id)
                    .ToList();
                }

                pedidos = queryItensPedido.Select(x => x.Pedido)
                .Distinct()
                .Skip(currentPage * currentPageSize)
                .Take(currentPageSize).ToList();

                totalPedidos = queryItensPedido.Select(x => x.Pedido).Distinct().ToList().Count;

                var pedidosVM = Mapper.Map<IEnumerable<Pedido>, IEnumerable<PedidoViewModel>>(pedidos);

                foreach (var vm in pedidosVM)
                {
                    var regiaoDias = fornecedor.FornecedorRegiao
                    .FirstOrDefault(x => x.CidadeId == vm.Endereco.CidadeId);

                    var regiaoSemanal = fornecedor.FornecedorRegiaoSemanal
                    .Where(x => x.CidadeId == vm.Endereco.CidadeId).ToList();

                    if (regiaoDias != null)
                    {
                        vm.PrazoEntrega = regiaoDias.Prazo;
                        vm.ValorPedidoMinimo = regiaoDias.VlPedMinRegiao;
                    }
                    if (regiaoSemanal.Any())
                    {
                        vm.PrazoEntrega = 0;
                        var prazoSemanalVM = Mapper.Map<IEnumerable<FornecedorPrazoSemanal>,
                            IEnumerable<FornecedorPrazoSemanalViewModel>>(regiaoSemanal);
                        vm.FornecedorPrazoSemanal = prazoSemanalVM.ToList();
                        vm.ValorPedidoMinimo = regiaoSemanal.FirstOrDefault(x => x.VlPedMinRegiao > 0).VlPedMinRegiao;
                    }

                }

                var itensVM = Mapper.Map<IEnumerable<ItemPedido>, IEnumerable<ItemPedidoViewModel>>(queryItensPedido);

                foreach (var pedido in pedidosVM)
                {
                    pedido.Itens = new List<ItemPedidoViewModel>();
                    pedido.Itens = itensVM.Where(x => x.PedidoId == pedido.PedidoId && x.AprovacaoMembro).ToList();
                    pedido.QtdItem = pedido.Itens.Where(x => x.AprovacaoFornecedor).Sum(p => p.quantity);
                    pedido.ValorTotal = pedido.Itens.Sum(x => x.quantity * x.PrecoNegociadoUnit);
                    pedido.StatusId = pedido.Itens.Select(x => x.EntregaId == 1).FirstOrDefault() ? 29 : 30; // Muda o status somente na tela.
                }

                var pagSet = new PaginacaoConfig<PedidoViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalPedidos,
                    TotalPages = (int)Math.Ceiling((decimal)totalPedidos / currentPageSize),
                    Items = pedidosVM.OrderBy(x => x.StatusId != 29).ThenBy(x => x.PedidoId).ToList()
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }


        [HttpGet]
        [Route("pedidosFornecedorAndamentoPf/{page:int=0}/{pageSize=4}/{dtDe?}/{dtAte?}/{pedidoId:int=0}")]
        public HttpResponseMessage PedidosFornecedorAndamentoPf(HttpRequestMessage request, int? page, int? pageSize, string dtDe = null, string dtAte = null, int? pedidoId = 0)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                List<Pedido> pedidos = null;
                HttpResponseMessage response = null;
                var totalPedidos = new int();

                // Caso não seja informado a data, pegamos os pedidos de 30 dias.
                dtDe = string.IsNullOrEmpty(dtDe) ? DateTime.MinValue.AddDays(0.1).ToString() : dtDe;
                dtAte = string.IsNullOrEmpty(dtAte) ? DateTime.Now.ToString() : dtAte;

                DateTime? DtIni = dtDe.ToDate().Value.AddHoraIni();
                DateTime? DtFim = dtAte.ToDate().Value.AddHoraFim();

                var queryItensPedido = new List<ItemPedido>();

                if (pedidoId > 0)
                {
                    queryItensPedido = _itemPedidoRep.GetAll()
                    .Where(x => x.FornecedorId == fornecedor.Id && x.AprovacaoFornecedor && x.Pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica &&
                     (x.Pedido.DtPedido >= DtIni.Value && x.Pedido.DtPedido <= DtFim.Value) &&
                     x.PedidoId == pedidoId && x.Pedido.StatusSistemaId >= 24)
                    .OrderByDescending(c => c.Pedido.DtPedido)
                    .ThenBy(o => o.PedidoId)
                    .ThenBy(o => o.Id)
                     .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();
                }
                else
                {
                    queryItensPedido = _itemPedidoRep.GetAll()
                    .Where(x => x.FornecedorId == fornecedor.Id && x.AprovacaoFornecedor && x.Pedido.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica &&
                    (x.Pedido.DtPedido >= DtIni.Value && x.Pedido.DtPedido <= DtFim.Value) &&
                    x.Pedido.StatusSistemaId >= 24)
                    .OrderByDescending(c => c.Pedido.DtPedido)
                    .ThenBy(o => o.PedidoId)
                    .ThenBy(o => o.Id)
                    .ToList();
                }

                pedidos = queryItensPedido.Select(x => x.Pedido)
                .Distinct()
                .Skip(currentPage * currentPageSize)
                .Take(currentPageSize).ToList();

                totalPedidos = queryItensPedido.Select(x => x.Pedido).Distinct().ToList().Count;

                var pedidosVM = Mapper.Map<IEnumerable<Pedido>, IEnumerable<PedidoPFViewModel>>(pedidos);

                foreach (var vm in pedidosVM)
                {
                    var regiaoDias = fornecedor.FornecedorRegiao
                    .FirstOrDefault(x => x.CidadeId == vm.Endereco.CidadeId);

                    var regiaoSemanal = fornecedor.FornecedorRegiaoSemanal
                    .Where(x => x.CidadeId == vm.Endereco.CidadeId).ToList();

                    if (regiaoDias != null)
                    {
                        vm.PrazoEntrega = regiaoDias.Prazo;
                        vm.ValorPedidoMinimo = regiaoDias.VlPedMinRegiao;
                    }
                    if (regiaoSemanal.Any())
                    {
                        vm.PrazoEntrega = 0;
                        var prazoSemanalVM = Mapper.Map<IEnumerable<FornecedorPrazoSemanal>,
                            IEnumerable<FornecedorPrazoSemanalViewModel>>(regiaoSemanal);
                        vm.FornecedorPrazoSemanal = prazoSemanalVM.ToList();
                        vm.ValorPedidoMinimo = regiaoSemanal.FirstOrDefault(x => x.VlPedMinRegiao > 0).VlPedMinRegiao;
                    }

                }

                var itensVM = Mapper.Map<IEnumerable<ItemPedido>, IEnumerable<ItemPedidoViewModel>>(queryItensPedido);

                foreach (var pedido in pedidosVM)
                {
                    pedido.Itens = new List<ItemPedidoViewModel>();
                    pedido.Itens = itensVM.Where(x => x.PedidoId == pedido.PedidoId && x.AprovacaoMembro).ToList();
                    pedido.QtdItem = pedido.Itens.Where(x => x.AprovacaoFornecedor).Sum(p => p.quantity);
                    pedido.ValorTotal = pedido.Itens.Sum(x => x.quantity * x.PrecoNegociadoUnit);
                    pedido.StatusId = pedido.Itens.Select(x => x.EntregaId == 1).FirstOrDefault() ? 29 : 30; // Muda o status somente na tela.
                }

                var pagSet = new PaginacaoConfig<PedidoPFViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalPedidos,
                    TotalPages = (int)Math.Ceiling((decimal)totalPedidos / currentPageSize),
                    Items = pedidosVM.OrderBy(x => x.StatusId != 29).ThenBy(x => x.PedidoId).ToList()
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("pesquisaAguardandoEntregaPedido/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetAguardandoEntregaPedido(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Pedido> pedido = null;

                int totalAguardandoEntregaPedido = new int();

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                Membro membro = _membroRep.FindBy(m => m.Pessoa.Id == usuario.Pessoa.Id).FirstOrDefault();


                var statusSistema = _statusSisRep.GetAll().Where(x => x.Ativo).ToList();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    pedido = _pedidoRep.FindBy(p => p.Id.ToString().Contains(filter) && p.MembroId == membro.Id && p.StatusSistemaId == 29)
                                                   .OrderBy(p => p.Id)
                                                   .Skip(currentPage * currentPageSize)
                                                   .Take(currentPageSize)
                                                   .ToList();

                    //Filtra itens do pedido com a condição do membro ter aprovado o produto na tela de meus pedidos
                    foreach (var item in pedido)
                    {
                        var itensPed = item.ItemPedidos.ToList();
                        item.ItemPedidos.Clear();

                        foreach (var itens in itensPed)
                        {
                            if (itens.AprovacaoMembro && itens.FornecedorId != null && itens.Ativo)
                            {
                                item.ItemPedidos.Add(itens);
                            }
                        }
                    }
                    totalAguardandoEntregaPedido =
                        _pedidoRep.FindBy(p => p.Id.ToString().Contains(filter) && p.MembroId == membro.Id && p.StatusSistemaId == 29)
                        .Count();

                }
                else
                {

                    pedido = _pedidoRep.GetAll().Where(p => p.MembroId == membro.Id && p.StatusSistemaId == 29)
                      .OrderBy(p => p.Id)
                      .Skip(currentPage * currentPageSize)
                      .Take(currentPageSize).ToList();

                    //Filtra itens do pedido com a condição do membro ter aprovado o produto na tela de meus pedidos
                    foreach (var item in pedido)
                    {
                        var itensPed = item.ItemPedidos.ToList();
                        item.ItemPedidos.Clear();

                        foreach (var itens in itensPed)
                        {
                            if (itens.AprovacaoMembro && itens.FornecedorId != null && itens.Ativo)
                            {
                                item.ItemPedidos.Add(itens);
                            }
                        }
                    }
                    totalAguardandoEntregaPedido = _pedidoRep.GetAll()
                    .Count(p => p.MembroId == membro.Id && p.StatusSistemaId == 29);

                }


                var idPedidos = pedido.Select(p => p.Id).ToList();
                List<int> pedidosNaoAvaliados = new List<int>();
                List<Fornecedor> forn = new List<Fornecedor>();

                foreach (var idped in idPedidos)
                {
                    var itemPed = _itemPedidoRep.GetAll().Where(i => i.PedidoId == idped && i.Ativo).ToList();


                    var fornecedoresItens = itemPed.Select(f => f.FornecedorId).ToList().Distinct();

                    foreach (var idFornecedor in fornecedoresItens)
                    {
                        var avFornecedor = _avaliacaoFornecedorRep.FindBy(a => a.FornecedorId == idFornecedor && a.PedidoId == idped).ToList().Count();
                        var itensFornecedor = itemPed.Where(i => i.FornecedorId == idFornecedor && i.AprovacaoMembro).ToList().Count;
                        var itenAvaliados = itemPed.Count(a => a.EntregaId == 3 && a.FornecedorId == idFornecedor);

                        if (itensFornecedor == itenAvaliados && avFornecedor < 1)
                        {
                            pedidosNaoAvaliados.Add(idped);
                            var fn = itemPed.Where(p => p.PedidoId == idped && p.FornecedorId == idFornecedor).Select(f => f.Fornecedor).FirstOrDefault();
                            forn.Add(fn);
                        }

                    }
                }

                IEnumerable<PedidoViewModel> AguardandoEntregaPedidoVM = Mapper.Map<IEnumerable<Pedido>, IEnumerable<PedidoViewModel>>(pedido);

                IEnumerable<FornecedorViewModel> fnVM = Mapper.Map<IEnumerable<Fornecedor>, IEnumerable<FornecedorViewModel>>(forn);

                var AguardandoEntregaPedidoViewModels = AguardandoEntregaPedidoVM as IList<PedidoViewModel> ?? AguardandoEntregaPedidoVM.ToList();

                for (int y = 0; y < AguardandoEntregaPedidoViewModels.Count; y++)
                {
                    if (AguardandoEntregaPedidoViewModels[y].StatusId > 23 && AguardandoEntregaPedidoViewModels[y].StatusId < 30)
                    {
                        AguardandoEntregaPedidoViewModels[y].Status = statusSistema.FirstOrDefault(x => x.Id == 29).DescStatus;
                    }
                }

                #region Adiciona Prazos Semanais para os Fornecedores

                for (int i = 0; i < AguardandoEntregaPedidoViewModels.Count; i++)
                {
                    var cidadeId = AguardandoEntregaPedidoViewModels[i].Endereco.CidadeId;

                    for (int j = 0; j < AguardandoEntregaPedidoViewModels[i].Itens.Count; j++)
                    {
                        if (AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor != null)
                        {
                            var regioesDias = AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor.FornecedorRegiao
                            .FirstOrDefault(x => x.CidadeId == cidadeId);

                            var regiaoSemanal = AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal
                            .Where(x => x.CidadeId == cidadeId).ToList();

                            if (regioesDias != null)
                            {
                                AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor.PrazoEntrega = regioesDias.Prazo;
                                AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor.VlPedidoMin =
                                    regioesDias.VlPedMinRegiao.ToString("C2");
                                AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal.Clear();
                            }
                            if (regiaoSemanal.Any())
                            {
                                AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor.PrazoEntrega = 0;
                                AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor.FornecedorPrazoSemanal = regiaoSemanal;
                                AguardandoEntregaPedidoViewModels[i].Itens[j].Fornecedor.VlPedidoMin = regiaoSemanal
                                    .FirstOrDefault(x => x.VlPedMinRegiao > 0).VlPedMinRegiao.ToString("C2");
                            }
                        }
                    }
                }

                #endregion

                PaginacaoConfig<PedidoViewModel> pagSet = new PaginacaoConfig<PedidoViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalAguardandoEntregaPedido,
                    TotalPages = (int)Math.Ceiling((decimal)totalAguardandoEntregaPedido / currentPageSize),
                    Items = AguardandoEntregaPedidoViewModels
                };


                response = request.CreateResponse(HttpStatusCode.OK, new { pedidos = pagSet, pedidosNavaliados = pedidosNaoAvaliados.Distinct(), fornecedores = fnVM.Select(i => i.Id).Distinct() });

                return response;
            });
        }


        [HttpPost]
        [Route("itensPedidoEntregueMembro/{statusEntregue:bool}")]
        public HttpResponseMessage AtualizaItensPedidoEntregueMembro(HttpRequestMessage request, ItemPedidoViewModel itenspedidoVm, bool statusEntregue)
        {
            return CreateHttpResponse(request, () =>
            {

                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                ItemPedido _itensPedido = _itemPedidoRep.GetSingle(itenspedidoVm.Id);

                if (statusEntregue)
                {
                    ItemPedidoViewModel itemPedidoMembroVM = new ItemPedidoViewModel();
                    itemPedidoMembroVM.DataEntregaMembro = DateTime.Now;
                    itemPedidoMembroVM.EntregaId = 3;

                    _itensPedido.AtualizarItensPedidoMembro(itemPedidoMembroVM, usuario);
                }
                else
                {
                    ItemPedidoViewModel itemPedidoMembroVM = new ItemPedidoViewModel();
                    itemPedidoMembroVM.DataEntregaMembro = DateTime.Now;
                    itemPedidoMembroVM.EntregaId = 1;

                    _itensPedido.AtualizarItensPedidoMembro(itemPedidoMembroVM, usuario);
                }


                _unitOfWork.Commit();

                var ItensPedidos = _itemPedidoRep.GetAll().Where(p => p.PedidoId == itenspedidoVm.PedidoId && p.EntregaId != 3 && p.FornecedorId == itenspedidoVm.FornecedorId && p.AprovacaoMembro && p.Ativo).ToList();

                var Pedidos = _pedidoRep.FindBy(p => p.Id == itenspedidoVm.PedidoId).ToList();

                var avaliavcaoFornecedor = _avaliacaoFornecedorRep.FindBy(a => a.PedidoId == itenspedidoVm.PedidoId && a.FornecedorId == itenspedidoVm.FornecedorId);

                IEnumerable<PedidoViewModel> ItemPedidoPedidoMembroVM = Mapper.Map<IEnumerable<Pedido>, IEnumerable<PedidoViewModel>>(Pedidos);

                response = request.CreateResponse(HttpStatusCode.OK, new { success = true, itens = ItensPedidos.Count(), avaliacao = avaliavcaoFornecedor.Count(), pedidos = ItemPedidoPedidoMembroVM });

                return response;
            });
        }


        [HttpPost]
        [Route("inserirAvaliacaoMembroFornecedorPedido")]
        public HttpResponseMessage InserirAvaliacaoMembroFornecedorPedido(HttpRequestMessage request, AvaliacaoFornecedorViewModel avaliacaoFornecedorViewModel)
        {
            return CreateHttpResponse(request, () =>
            {

                HttpResponseMessage response = null;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


                    AvaliacaoFornecedor avaliacaoFornecedor = new AvaliacaoFornecedor
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        FornecedorId = avaliacaoFornecedorViewModel.FornecedorId,
                        PedidoId = avaliacaoFornecedorViewModel.PedidoId,
                        NotaQualidadeProdutos = avaliacaoFornecedorViewModel.QualidadeProdutos,
                        NotaTempoEntrega = avaliacaoFornecedorViewModel.TempoEntrega,
                        ObsQualidade = avaliacaoFornecedorViewModel.ObsQualidade,
                        ObsEntrega = avaliacaoFornecedorViewModel.ObsEntrega,
                        ObsAtendimento = avaliacaoFornecedorViewModel.ObsAtendimento,
                        NotaAtendimento = avaliacaoFornecedorViewModel.Atendimento,
                        Ativo = true

                    };

                    _avaliacaoFornecedorRep.Add(avaliacaoFornecedor);

                    _unitOfWork.Commit();

                    AvaliacaoFornecedorViewModel avaliacaoFornecedorVM = Mapper.Map<AvaliacaoFornecedor, AvaliacaoFornecedorViewModel>(avaliacaoFornecedor);

                    response = request.CreateResponse(HttpStatusCode.OK, avaliacaoFornecedorVM);

                }

                return response;
            });
        }


        [HttpGet]
        [Route("pesquisaItensPedidosEntregues/{idPedido:int}")]
        public HttpResponseMessage GetItensPedidosEntregues(HttpRequestMessage request, int? idPedido)
        {

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<ItemPedido> itensPedidos = new List<ItemPedido>();
                bool fornecedoresAv = false;
                int fornecedoresAdd = 0;

                if (idPedido > 0)
                {
                    //Pega todos os itens do Pedido
                    var todosItensPedido = _itemPedidoRep.GetAll()
                        .Where(i => i.PedidoId == idPedido && i.AprovacaoFornecedor && i.FornecedorId != null && i.Ativo)
                        .ToList();

                    //Verificamos se existe algum item do pedido que ainda não foi checado como entregue
                    itensPedidos = todosItensPedido
                        .Where(i => i.EntregaId != 3 && i.AprovacaoMembro)
                        .ToList();

                    //Pega os ids dos Fornecedores
                    var idFornecedores = todosItensPedido.Where(x => x.AprovacaoMembro).Select(i => i.FornecedorId).Distinct().ToList();

                    //Verifica fornecedor por fornecedor se existe algum Avaliação para este pedido
                    foreach (var id in idFornecedores)
                    {
                        var resultadoAvaliacao = _avaliacaoFornecedorRep
                        .FindBy(x => x.PedidoId == idPedido && x.FornecedorId == id).FirstOrDefault();

                        if (resultadoAvaliacao == null)
                        {
                            fornecedoresAdd++;
                        }
                    }
                    //Se não tiver nenhuma Avaliação entra no if
                    if (fornecedoresAdd == 0)
                    {
                        fornecedoresAv = true;
                    }
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true, qtdItens = itensPedidos.Count, fornecedoresAvaliados = fornecedoresAv });
                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, "Não foi possível devolver a quantidade de itens do pedido " + idPedido);
                }
                return response;
            });
        }


        [HttpPost]
        [Route("atualizaStatusPedidoFinalizado/{idpedido:int}")]
        public HttpResponseMessage AtualizaStatusPedidoFinalizado(HttpRequestMessage request, int idpedido)
        {
            return CreateHttpResponse(request, () =>
            {

                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                Pedido pedido = _pedidoRep.GetSingle(idpedido);

                if (idpedido > 0)
                {
                    PedidoViewModel pedVM = new PedidoViewModel();
                    pedVM.StatusId = 30;
                    pedido.AtualizarStatusPedidoFinalizado(pedVM, usuario);
                }

                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }


        [HttpPost]
        [Route("atualizaItemPedidoMembro/{aprovacaoMembro:bool}")]
        public HttpResponseMessage AtualizaItemPedidoMembro(HttpRequestMessage request, ItemPedidoViewModel itenspedidoVm, bool aprovacaoMembro)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                ItemPedido _itensPedido = _itemPedidoRep.GetSingle(itenspedidoVm.Id);

                if (aprovacaoMembro)
                {
                    _itensPedido.AprovacaoMembro = true;
                    _itensPedido.UsuarioAlteracao = usuario;
                    _itensPedido.DtAlteracao = DateTime.Now;

                    _itemPedidoRep.Edit(_itensPedido);
                }
                else
                {
                    _itensPedido.AprovacaoMembro = false;
                    _itensPedido.UsuarioAlteracao = usuario;
                    _itensPedido.DtAlteracao = DateTime.Now;
                    _itemPedidoRep.Edit(_itensPedido);
                }

                _unitOfWork.Commit();

                itenspedidoVm = Mapper.Map<ItemPedido, ItemPedidoViewModel>(_itensPedido);

                var pedido = _pedidoRep.FirstOrDefault(x => x.Id == itenspedidoVm.PedidoId);

                var qtdItensAprovados = pedido.ItemPedidos.Count(p => p.FornecedorId == itenspedidoVm.FornecedorId && p.AprovacaoMembro);
                var totalItens = pedido.ItemPedidos.Count(x => x.AprovacaoMembro);

                response = request.CreateResponse(HttpStatusCode.OK, new { QtdItens = qtdItensAprovados, TotalItensPedido = totalItens });

                return response;
            });
        }


        [HttpPost]
        [Route("AtualizaItensPedido/{aprovacaoMembro:bool}")]
        public HttpResponseMessage AtualizaItensPedido(HttpRequestMessage request, List<ItemPedidoViewModel> itenspedidoVm, bool aprovacaoMembro)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var idsItens = itenspedidoVm.Select(x => x.Id).ToList();
                var countAprovados = 0;
                var countReprovados = 0;

                List<ItemPedido> _listItensPedido = _itemPedidoRep.FindBy(x => idsItens.Contains(x.Id)).ToList();

                for (int i = 0; i < _listItensPedido.Count; i++)
                {
                    if (aprovacaoMembro)
                    {
                        _listItensPedido[i].AprovacaoMembro = true;
                        _listItensPedido[i].UsuarioAlteracao = usuario;
                        _listItensPedido[i].DtAlteracao = DateTime.Now;
                        _itemPedidoRep.Edit(_listItensPedido[i]);
                        countAprovados++;
                    }
                    else
                    {
                        _listItensPedido[i].AprovacaoMembro = false;
                        _listItensPedido[i].UsuarioAlteracao = usuario;
                        _listItensPedido[i].DtAlteracao = DateTime.Now;
                        _itemPedidoRep.Edit(_listItensPedido[i]);
                        countReprovados++;
                    }
                }

                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.OK, new { ItensAprovados = countAprovados, ItensReprovados = countReprovados });

                return response;
            });
        }

        [HttpGet]
        [Route("mapa/{pedidoId:int=0}")]
        public HttpResponseMessage Mapa(HttpRequestMessage request, int pedidoId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var enderecoFornecedor = usuario.Pessoa.Enderecos.FirstOrDefault();
                var idFornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId).Id;

                //Caso a localização do usuário não esteja cadastrado, atualiza a posição geográfica do usuário
                if (enderecoFornecedor.Localizacao == null)
                    enderecoFornecedor.LocalizacaoGoogle();

                var fornecedor = new
                {
                    Id = usuario.PessoaId,
                    Nome = usuario.Pessoa.PessoaJuridica.NomeFantasia,
                    enderecoFornecedor.Localizacao.Latitude,
                    enderecoFornecedor.Localizacao.Longitude
                };

                var pedidoSel = this._pedidoRep.GetSingle(pedidoId);
                var enderecoPedidoSel = pedidoSel.Endereco;
                if (enderecoPedidoSel.Localizacao == null)
                    enderecoPedidoSel.LocalizacaoGoogle();
                var distancia = enderecoPedidoSel.Localizacao.Distance(enderecoFornecedor.Localizacao);

                var desconto = pedidoSel.ItemPedidos.Where(i => i.FornecedorId == idFornecedor)
                    .Sum(x => (x.PrecoNegociadoUnit ?? 0) * x.Quantidade) *
                    pedidoSel.ItemPedidos.FirstOrDefault(i => i.FornecedorId == idFornecedor)?.Desconto / 100;

                var valorPedido = (pedidoSel.ItemPedidos.Where(i => i.FornecedorId == idFornecedor)
                    .Sum(x => (x.PrecoNegociadoUnit ?? 0) * x.Quantidade) - desconto)?.ToString("C2");


                var pedidoSelecionado = new
                {
                    PedidoId = pedidoId,
                    Id = enderecoPedidoSel.PessoaId,
                    //  Nome = enderecoPedidoSel.Pessoa.PessoaJuridica.NomeFantasia,
                    Nome = enderecoPedidoSel.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? enderecoPedidoSel.Pessoa.PessoaJuridica.NomeFantasia : enderecoPedidoSel.Pessoa.PessoaFisica.Nome,
                    enderecoPedidoSel.Localizacao.Latitude,
                    enderecoPedidoSel.Localizacao.Longitude,
                    Distancia = distancia >= 1000 ? string.Format("{0:F2} km", distancia / 1000.0) : string.Format("{0:F2} m", distancia),
                    Total = valorPedido
                };

                var cotacao = this._cotacaoRep.GetAll().FirstOrDefault(x => x.CotacaoPedidos.Any(y => y.PedidoId.Equals(pedidoId)));
                var pedidos = cotacao.CotacaoPedidos.Where(x => !x.PedidoId.Equals(pedidoId)).Select(x => x.Pedido).ToList();
                var pedidosId = cotacao.CotacaoPedidos.Where(x => !x.PedidoId.Equals(pedidoId)).Select(x => x.Pedido.Id).ToList();


                var indisponibilidade = _indisponibilidadeProdutoRep
                                            .FindBy(i => i.FornecedorId == fornecedor.Id)
                                            .Select(p => p.ProdutoId).ToList();

                //pega todos os pedido da tabela remover fornecedor que esteja na cotação = ProdutoId
                var removeItemForn = _removeFornPedidoRep
                        .GetAll().Where(x => x.FonecedorId == fornecedor.Id
                        && pedidosId.Contains(x.PedidoId)
                        ).Select(p => p.ItemPedidoId).ToList();


                var pedidosCotacao = new List<object>();

                foreach (var pedido in pedidos.Where(w => w.ItemPedidos.Where(i => !indisponibilidade.Contains(i.ProdutoId)
                                                        && i.FornecedorId == idFornecedor
                                                        && i.Ativo && i.AprovacaoMembro
                                                        && !removeItemForn.Contains(i.Id)).Any()
                                                        && w.StatusSistemaId == 24).Distinct())
                {
                    var endereco = pedido.Endereco;
                    if (endereco.Localizacao == null)
                        endereco.LocalizacaoGoogle();

                    if (endereco.Localizacao == null) continue;

                    distancia = endereco.Localizacao.Distance(enderecoFornecedor.Localizacao);


                    var descontoPed = pedido.ItemPedidos.Where(i => i.FornecedorId == idFornecedor)
                    .Sum(x => (x.PrecoNegociadoUnit ?? 0) * x.Quantidade) *
                    pedido.ItemPedidos.FirstOrDefault(i => i.FornecedorId == idFornecedor)?.Desconto / 100;

                    decimal descPed = 0;
                    decimal.TryParse(descontoPed.ToString(), out descPed);

                    pedidosCotacao.Add(new
                    {
                        PedidoId = pedido.Id,
                        Id = endereco.PessoaId,
                        Nome = endereco.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? endereco.Pessoa.PessoaJuridica.NomeFantasia : endereco.Pessoa.PessoaFisica.Nome,
                        endereco.Localizacao.Latitude,
                        endereco.Localizacao.Longitude,
                        Distancia = distancia >= 1000 ? string.Format("{0:F2} km", distancia / 1000.0) : string.Format("{0:F2} m", distancia),
                        Total = (pedido.ItemPedidos.Where(i => !indisponibilidade.Contains(i.ProdutoId)
                                                        && i.FornecedorId == idFornecedor
                                                        && i.Ativo && i.AprovacaoMembro
                                                        && !removeItemForn.Contains(i.Id)).Sum(x => (x.PrecoNegociadoUnit ?? 0) * x.Quantidade) - descPed).ToString("C2")
                    });
                }

                response = request.CreateResponse(HttpStatusCode.OK, new
                {
                    Fornecedor = fornecedor,
                    PedidoSelecionado = pedidoSelecionado,
                    PedidosCotacao = pedidosCotacao
                });

                this._unitOfWork.Commit();

                return response;
            });
        }


        [HttpPost]
        [Route("pesquisaFornecedorProduto")]
        public HttpResponseMessage PesquisaFornecedorProduto(HttpRequestMessage request, List<ItemPedidoViewModel> items)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Produto> prod = new List<Produto>();

                var idItens = items.Select(x => x.sku).ToList();

                foreach (int id in idItens)
                {
                    var itemPedido = _produtoRep.FirstOrDefault(x => x.Id == id);
                    prod.Add(itemPedido);
                }

                IEnumerable<ProdutoPromocionalViewModel> produtoVM = Mapper.Map<IEnumerable<Produto>, IEnumerable<ProdutoPromocionalViewModel>>(prod);

                response = request.CreateResponse(HttpStatusCode.OK, produtoVM);

                return response;
            });
        }


        [HttpPost]
        [Route("aprovarPedidoPromocao/{aprovacao:bool}")]
        public HttpResponseMessage AprovarPedido(HttpRequestMessage request, PedidoViewModel pedidoViewModel, bool aprovacao)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var notificacoesAlertasService = new NotificacoesAlertasService();

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var idFornecedor = _fornecedorRep.FindBy(x => x.PessoaId == usuario.PessoaId).Select(x => x.Id).FirstOrDefault();

                var ped = _pedidoRep.GetSingle(pedidoViewModel.PedidoId);
                var usuarioMembro = ped.Membro.Pessoa.Usuarios.FirstOrDefault(x => x.Id == ped.UsuarioCriacaoId);
                var itensPed = ped.ItemPedidos.Where(x => x.FornecedorId == idFornecedor).ToList();

                if (aprovacao)
                {
                    foreach (var item in itensPed)
                    {
                        var itemPedido = _itemPedidoRep.GetSingle(item.Id);
                        itemPedido.AprovacaoFornecedor = true;
                        itemPedido.UsuarioAlteracao = usuario;
                        itemPedido.DtAlteracao = DateTime.Now;
                        itemPedido.AprovacaoMembro = true;
                        itemPedido.Ativo = true;

                        _itemPedidoRep.Edit(itemPedido);
                    }

                    Sms sms = new Sms()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Numero = usuarioMembro.Telefones.Select(t => t.DddCel).FirstOrDefault() + usuarioMembro.Telefones.Select(t => t.Celular).FirstOrDefault(),
                        Mensagem = "Economiza Já - Seu pedido promocional de número " + ped.Id + " foi aprovado.",
                        Status = StatusSms.NaoEnviado,
                        OrigemSms = TipoOrigemSms.FornecedorAprovaItensPedido,
                        Ativo = true

                    };

                    #region Verifica se o membro deseja receber EMAIL e SMS


                    int tipoAvisoId = (int)TipoAviso.PedidoItensAprovados;


                    if (notificacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, tipoAvisoId, TipoAlerta.EMAIL))
                    {
                        //Envia EMAIL para membro
                        _ultilService.EnviaEmailPedido(pedidoViewModel.PedidoId, 4, usuario);
                    }

                    if (notificacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, tipoAvisoId, TipoAlerta.SMS))
                    {
                        //Envia SMS para membro
                        _smsRep.Add(sms);
                    }


                    #endregion

                    ped.StatusSistemaId = _statusSisRep.FindBy(x => x.WorkflowStatusId == 12 && x.Ordem == 5 && x.Ativo).Select(o => o.Id).FirstOrDefault();
                    _pedidoRep.Edit(ped);


                    //Inserir Histórico Pedido
                    var pedidoHistorico = new HistStatusPedido();
                    pedidoHistorico.Ativo = true;
                    pedidoHistorico.UsuarioCriacao = usuario;
                    pedidoHistorico.DtCriacao = DateTime.Now;
                    pedidoHistorico.Pedido = ped;
                    pedidoHistorico.PedidoId = ped.Id;
                    pedidoHistorico.StatusSistemaId = 29; //Status do Pedido Aguardando Entrega
                    _histStatusPedido.Add(pedidoHistorico);
                }
                else
                {
                    foreach (var item in itensPed)
                    {
                        var itemPedido = _itemPedidoRep.GetSingle(item.Id);
                        itemPedido.AprovacaoFornecedor = false;
                        itemPedido.UsuarioAlteracao = usuario;
                        itemPedido.DtAlteracao = DateTime.Now;
                        itemPedido.AprovacaoMembro = false;
                        itemPedido.Ativo = false;

                        _itemPedidoRep.Edit(itemPedido);
                    }

                    //Envia e-mail para Membro que o pedido foi cancelado
                    var corpoEmail = _templateEmail.FindBy(e => e.Id == 23).Select(e => e.Template).FirstOrDefault();

                    #region Inserir Email e SMS

                    if (corpoEmail != null)
                    {

                        Emails emails = null;
                        if (ped.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                        {
                            var corpoEmailPedPromocaoCancelada = corpoEmail.Replace("#NomeMembro#",
                                ped.Membro.Pessoa.PessoaJuridica.NomeFantasia)
                                .Replace("#Id#", ped.Id.ToString())
                                .Replace("#Motivo#", pedidoViewModel.DescMotivoCancelamento.Trim());

                            emails = new Emails()
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                AssuntoEmail = "Pedido " + ped.Id + " cancelado - Seu pedido promocional foi cancelado.",
                                EmailDestinatario = usuarioMembro.UsuarioEmail,
                                CorpoEmail = corpoEmailPedPromocaoCancelada.Trim(),
                                Status = Status.NaoEnviado,
                                Origem = Origem.FornecedorCancelaPedidoPromocional,
                                Ativo = true

                            };
                        }
                        else
                        {

                            var corpoEmailPedPromocaoCancelada = corpoEmail.Replace("#NomeMembro#",
                             ped.Membro.Pessoa.PessoaFisica.Nome)
                             .Replace("#Id#", ped.Id.ToString())
                             .Replace("#Motivo#", pedidoViewModel.DescMotivoCancelamento.Trim());

                            emails = new Emails()
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                AssuntoEmail = "Pedido " + ped.Id + " cancelado - Seu pedido promocional foi cancelado.",
                                EmailDestinatario = usuarioMembro.UsuarioEmail,
                                CorpoEmail = corpoEmailPedPromocaoCancelada.Trim(),
                                Status = Status.NaoEnviado,
                                Origem = Origem.FornecedorCancelaPedidoPromocional,
                                Ativo = true

                            };
                        }



                        Sms sms = new Sms()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Numero = usuarioMembro.Telefones.Select(t => t.DddCel).FirstOrDefault() + usuarioMembro.Telefones.Select(t => t.Celular).FirstOrDefault(),
                            Mensagem = "Economiza Já - Pedido cancelado. Seu pedido promocional " + ped.Id + " foi cancelado.",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.FornecedorCancelaPedidoPromocional,
                            Ativo = true

                        };

                        //Verifica se o membro deseja receber EMAIL OU SMS

                        int tipoAvisoId = (int)TipoAviso.PedidoCancelado;

                        if (notificacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, tipoAvisoId, TipoAlerta.EMAIL))
                        {
                            //Envia EMAIL para fornecedor
                            _emailRep.Add(emails);
                        }

                        if (notificacoesAlertasService.PodeEnviarNotificacao(usuarioMembro.Id, tipoAvisoId, TipoAlerta.SMS))
                        {
                            //Envia SMS para fonecedor
                            _smsRep.Add(sms);
                        }

                    }

                    #endregion

                    #region Inserir Aviso para o Membro

                    Avisos novoAviso = new Avisos();

                    novoAviso = new Avisos();

                    novoAviso.UsuarioCriacaoId = 1;
                    novoAviso.DtCriacao = DateTime.Now;
                    novoAviso.Ativo = true;
                    novoAviso.IdReferencia = ped.Id;
                    novoAviso.DataUltimoAviso = DateTime.Now;
                    novoAviso.ExibeNaTelaAvisos = true;
                    novoAviso.TipoAvisosId = (int)TipoAviso.PedidoPromocionalCancelado; //Id Aviso
                    novoAviso.URLPaginaDestino = "/#/meusPedidosPromocao";
                    novoAviso.TituloAviso = "Pedido promocional cancelado";
                    novoAviso.ToolTip = "Pedido promocional cancelado";
                    string descAviso = "Pedido promocional " + ped.Id + " cancelado";
                    novoAviso.DescricaoAviso = descAviso.Length > 99 ? descAviso.Substring(0, 99) : descAviso;
                    novoAviso.ModuloId = 3; //Modulo Membro
                    novoAviso.UsuarioNotificadoId = usuarioMembro.Id;
                    _avisosRep.Add(novoAviso);


                    #endregion


                    ped.StatusSistemaId = _statusSisRep.FindBy(x => x.WorkflowStatusId == 12 && x.Ordem == 10 && x.Ativo)
                    .Select(o => o.Id).FirstOrDefault();

                    _pedidoRep.Edit(ped);

                    //Inserir Histórico Pedido
                    var pedidoHistorico = new HistStatusPedido();
                    pedidoHistorico.Ativo = true;
                    pedidoHistorico.UsuarioCriacao = usuario;
                    pedidoHistorico.DtCriacao = DateTime.Now;
                    pedidoHistorico.Pedido = ped;
                    pedidoHistorico.PedidoId = ped.Id;
                    pedidoHistorico.DescMotivoCancelamento = pedidoViewModel.DescMotivoCancelamento;
                    pedidoHistorico.StatusSistemaId = 36; //Status do Pedido Cancelado

                    _histStatusPedido.Add(pedidoHistorico);

                }

                _unitOfWork.Commit();

                if (aprovacao)
                    this._pagamentoService.GerarComissao(usuario.Id, pedidoViewModel.PedidoId, idFornecedor);

                response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;

            });
        }

        [HttpGet]
        [Route("verificaDataPedidoFeriado/{data?}/{estado?}")]
        public HttpResponseMessage VerificaDataPedidoFeriado(HttpRequestMessage request, string data = null, string estado = null)
        {
            HttpResponseMessage response = null;

            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var dataPedido = data.ToDate().Value;

                var dataFeriado = _calendarioFeriadoService.VerificaFeriadoMembro(dataPedido, membro, estado);

                var calendarioFeriadoVM = Mapper.Map<CalendarioFeriado, CalendarioFeriadoViewModel>(dataFeriado.CalendarioFeriado);

                response = request.CreateResponse(HttpStatusCode.OK, new { calendarioFeriadoVM, dataFeriado.TipoRetornoFeriado });

                return response;
            });
        }


    }
}
