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
using ECC.API_Web.InfraWeb;
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeEmail;
using ECC.EntidadeParametroSistema;
using ECC.EntidadePessoa;
using ECC.EntidadeRecebimento;
using ECC.Entidades;
using ECC.Entidades.EntidadeRecebimento;
using ECC.EntidadeSms;
using ECC.EntidadeUsuario;
using ECC.Servicos;
using ECC.Servicos.Abstrato;
using Gerencianet.SDK;
using Gerencianet.SDK.Requests;
using Gerencianet.SDK.Responses;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace ECC.API_Web.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "Admin, Membro, Fornecedor,Franquia")]
    [RoutePrefix("api/pagamentos")]
    public class PagamentosController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Fatura> _faturaRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Mensalidade> _mensalidadeRep;
        private readonly IPagamentoService _pagamentoService;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateRep;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IEntidadeBaseRep<PlanoMensalidade> _planosMensalidadeRep;
        private readonly IEntidadeBaseRep<CartaoCredito> _cartaoCreditoRep;
        private readonly IEntidadeBaseRep<PlanoSegmento> _planoSegmentoRep;
        public PagamentosController(
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<Mensalidade> mensalidadeRep,
            IEntidadeBaseRep<Fatura> faturaRep,
            IPagamentoService pagamentoService,
            IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
            IEntidadeBaseRep<Emails> emailsRep,
            IEntidadeBaseRep<TemplateEmail> templateRep,
            IEntidadeBaseRep<Erro> _errosRepository,
            IEntidadeBaseRep<PlanoMensalidade> planosMensalidadeRep,
            IEntidadeBaseRep<CartaoCredito> cartaoCreditoRep,
             IEntidadeBaseRep<PlanoSegmento> planoSegmentoRep,
            IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _membroRep = membroRep;
            _fornecedorRep = fornecedorRep;
            _mensalidadeRep = mensalidadeRep;
            _faturaRep = faturaRep;
            _pagamentoService = pagamentoService;
            _parametroSistemaRep = parametroSistemaRep;
            _templateRep = templateRep;
            _emailsRep = emailsRep;
            _planosMensalidadeRep = planosMensalidadeRep;
            _cartaoCreditoRep = cartaoCreditoRep;
            _planoSegmentoRep = planoSegmentoRep;
        }

        [HttpGet]
        [Route("fatura")]
        public HttpResponseMessage GetFatura(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var faturas = _faturaRep.GetAll().Where(x => x.FornecedorId == fornecedor.Id);

                //var diaVencimento = this._pagamentoService.Parametros.FornecedorDiaVencimento;
                var diaFechamento = _pagamentoService.Parametros.FornecedorDiaFechamento;

                var day = DateTime.Now.Day;
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;


                if (month == 12)
                {
                    month = 1;
                    year++;
                }
                else if (day > diaFechamento)
                {
                    month++;
                }


                var fechamentoAtual = new DateTime(year, month, diaFechamento);
                var fechamentoAnterior = fechamentoAtual.AddMonths(-1);

                var faturaAnterior = faturas.FirstOrDefault(x => x.DtFechamento == fechamentoAnterior);

                var faturaAtual = faturas.FirstOrDefault(x => x.DtFechamento == fechamentoAtual);

                //var faturaProxima = faturas.FirstOrDefault(x => x.DtFechamento == fechamentoProximo);

                var faturasRT = new
                {
                    Anterior = Mapper.Map<Fatura, FaturaViewModel>(faturaAnterior),
                    Atual = Mapper.Map<Fatura, FaturaViewModel>(faturaAtual)
                    //Proxima = Mapper.Map<Fatura, FaturaViewModel>(faturaProxima)
                };

                response = request.CreateResponse(HttpStatusCode.OK, faturasRT);

                return response;
            });
        }

        [HttpPost]
        [Route("gerarboletofornecedor")]
        public HttpResponseMessage GerarBoletoFornecedor(HttpRequestMessage request, FaturaViewModel faturaVm)
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

                    var fatura = _faturaRep.GetSingle(faturaVm.Id);
                    var param = new List<string> { "MULTA_MORA", "MULTA_ATRASO" };

                    //Pega parâmetros de multa
                    var parametrosAtrasoPagtoFatura = _parametroSistemaRep.GetAll().Where(x => param.Contains(x.Codigo)).ToList();
                    var multaDiaria = (decimal)parametrosAtrasoPagtoFatura.FirstOrDefault(c => c.Codigo == "MULTA_MORA")?.Valor.TryParseInt() / 30;
                    var multaAtraso = parametrosAtrasoPagtoFatura.FirstOrDefault(c => c.Codigo == "MULTA_ATRASO")?.Valor.TryParseInt();

                    var pessoa = fatura.Fornecedor.Pessoa;
                    var telefone = pessoa.Telefones.FirstOrDefault(x => x.Ativo);
                    var endereco = pessoa.Enderecos.FirstOrDefault();
                    //endereco.Bairro = new Bairro();
                    //endereco.Estado = new Estado();

                    var diasAtraso = DateTime.Now.Subtract(fatura.DtVencimento).Days;
                    var valorAtrasoDias = fatura.Comissoes.Sum(x => x.Valor) * (diasAtraso * multaDiaria);
                    var valorMultaAtraso = fatura.Comissoes.Sum(x => x.Valor) * multaAtraso / 100;
                    var valorMulta = valorMultaAtraso + valorAtrasoDias;

                    var gerenciaNet = new GerencianetAPI();

                    #region Criando Transação


                    List<Item> itens = new List<Item>();

                    fatura.Comissoes.ForEach(c => itens.Add(new Item
                    {
                        Name = $"Pedido: {c.PedidoId}",
                        Value = (int)Convert.ToDecimal(string.Format("{0:N}", c.Valor).Replace(",", "").Replace(".", ""))
                    }));

                    itens.Add(new Item
                    {
                        Name = "Multa e Juros",
                        Value = (int)Convert.ToDecimal(string.Format("{0:N}", valorMulta).Replace(",", "").Replace(".", ""))
                    });


                    var metadata = new Metadata
                    {
                        CustomId = fatura.Id.ToString(),
                        NotificationURL = $"{ConfigurationManager.AppSettings[$"{Environment.GetEnvironmentVariable("Amb_EconomizaJa")}_UrlRetorno"]}api/pagamentos/retornoFatura"
                    };

                    var transacao = new TransactionRequest
                    {
                        Items = itens.ToArray(),
                        Metadata = metadata
                    };

                    var retornoTransacao = gerenciaNet.CreateTransaction(transacao);

                    #endregion

                    #region Associando Forma de Pagamento Boleto para Transação

                    Payment payment = new Payment();

                    payment.BankingBillet = new BankingBillet
                    {
                        ExpireAt = DateTime.Now.AddDays(1).ToShortDateString(), //Vencimento para o dia seguinte
                        Customer = new Customer
                        {
                            Name = pessoa.Usuarios.FirstOrDefault()?.UsuarioNome,
                            Birth = (pessoa.PessoaJuridica.DtFundacao ?? DateTime.Now).ToString("yyyy-MM-dd"),
                            Email = pessoa.PessoaJuridica.Email,
                            PhoneNumber = $"{telefone?.DddTelComl}{telefone?.TelefoneComl}",
                            Address = new Address
                            {
                                ZipCode = endereco.Cep,
                                City = endereco.Cidade.DescCidade,
                                Number = endereco.Numero.ToString(),
                                Neighborhood = endereco.Bairro.DescBairro,
                                Complement = endereco.Complemento,
                                State = endereco.Estado.Uf,
                                Street = endereco.DescEndereco
                            },
                            JuridicalPerson = new JuridicalPerson
                            {
                                CNPJ = pessoa.PessoaJuridica.Cnpj,
                                CorporateName = pessoa.PessoaJuridica.RazaoSocial
                            }
                        },

                        //Adicionando juros por dia caso venha pagar depois do vencimento
                        Configurations = new ConfigGerenciaNet
                        {
                            Interest = 33
                        },

                        Message = "Após vencimento será cobrado multa de 2% sobre o valor boleto e 0,033% ao dia."
                    };

                    var retornoBoleto = gerenciaNet.CreateBankingBillet(retornoTransacao.ChargerId, payment);

                    #endregion

                    #region Cancelando Boleto Vencido

                    var retornoCancelBoleto = new object();
                    if (fatura.ChargerId > 0)
                    {
                        retornoCancelBoleto = gerenciaNet.CancelTransaction(fatura.ChargerId);
                    }

                    #endregion

                    #region Atualizando Dados Fatura Banco

                    if (retornoCancelBoleto.TryParseInt() == 200)
                    {
                        fatura.DtAlteracao = DateTime.Now;
                        fatura.ChargerId = retornoBoleto.ChargeId;
                        fatura.UrlBoleto = retornoBoleto.Link;
                        fatura.Status = StatusFatura.AguardandoPagamento;
                        _faturaRep.Edit(fatura);
                        _unitOfWork.Commit();

                        #region Envia Email

                        Emails emails = new Emails
                        {
                            UsuarioCriacaoId = 1,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Boleto Gerado - 2º via do boleto gerada.",
                            EmailDestinatario = pessoa.PessoaJuridica.Email,

                            CorpoEmail = _templateRep.GetSingle(28).Template.Trim()
                            .Replace("#NomeFantasia#", pessoa.PessoaJuridica.NomeFantasia)
                            .Replace("#UrlBoleto#", fatura.UrlBoleto),

                            Status = Status.NaoEnviado,
                            Origem = Origem.BoletoGerado,
                            Ativo = true
                        };

                        _emailsRep.Add(emails);
                        _unitOfWork.Commit();

                        #endregion

                    }

                    #endregion

                    response = request.CreateResponse(HttpStatusCode.OK, new { });

                    return response;
                }

                return response;
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("retornoFatura")]
        public HttpResponseMessage RetornoFatura(HttpRequestMessage request, TokenNotificationGerenciaNet retorno)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var status = new int();

                var data = _pagamentoService.ReturnStatusFatura(retorno.Notification);

                switch (data.Status.StatusAtual)
                {

                    case "waiting":
                        status = (int)StatusFatura.AguardandoPagamento;
                        break;

                    case "unpaid":
                        status = (int)StatusFatura.NaoPago;
                        break;


                    case "paid":
                        status = (int)StatusFatura.Recebido;
                        break;

                    case "settled":
                        status = (int)StatusFatura.Recebido;
                        break;

                    case "refunded":
                        status = (int)StatusFatura.Devolvido;
                        break;

                    case "contested":
                        status = (int)StatusFatura.Contestado;
                        break;

                    case "canceled":
                        status = (int)StatusFatura.Cancelado;
                        break;
                }

                var fatura = _faturaRep.GetSingle(data.CustomId);

                if (status > 0)
                {
                    if (status != (int)StatusFatura.Cancelado && status != (int)StatusFatura.Recebido)
                    {
                        fatura.Status = (StatusFatura)status;
                        fatura.Token = retorno.Notification;
                        _faturaRep.Edit(fatura);
                        _unitOfWork.Commit();

                    }
                    else if (status == (int)StatusFatura.Recebido)
                    {
                        fatura.Status = (StatusFatura)status;
                        fatura.Token = retorno.Notification;
                        fatura.DtRecebimento = DateTime.Now;
                        _faturaRep.Edit(fatura);
                        _unitOfWork.Commit();
                    }
                }
                //var faturaVM = Mapper.Map<Fatura, FaturaViewModel>(fatura);

                response = request.CreateResponse(HttpStatusCode.OK, new { });
                return response;
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("retornoMensalidade")]
        public HttpResponseMessage RetornoMensalidade(HttpRequestMessage request, TokenNotificationGerenciaNet retorno)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var status = new int();

                var data = _pagamentoService.ReturnStatusFatura(retorno.Notification);

                switch (data.Status.StatusAtual)
                {
                    case "new":
                        status = (int)StatusMensalidade.Gerado; // Assinatura criada, porém nenhuma cobrança foi paga. O termo "new" equivale a "nova".
                        break;

                    case "active":
                        status = (int)StatusMensalidade.Recebido; // Assinatura ativa. Todas as cobranças estão sendo geradas. O termo "active" equivale a "ativa".
                        break;

                    case "paid":
                        status = (int)StatusMensalidade.Recebido;
                        break;

                    case "waiting":
                        status = (int)StatusMensalidade.AguardandoPagamento;
                        break;

                    case "canceled":
                        status = (int)StatusMensalidade.Cancelado; // Assinatura foi cancelada pelo vendedor ou pelo pagador. O termo "canceled" equivale a "cancelada".
                        break;

                    case "expired":
                        status = (int)StatusMensalidade.AssinaturaExpirada; // Assinatura expirada. Todas as cobranças configuradas para a assinatura já foram emitidas. 
                                                                            // O termo "expired" equivale a "expirada".
                        break;

                    case "unpaid":
                        status = (int)StatusMensalidade.NaoPago;
                        break;

                    case "settled":
                        status = (int)StatusMensalidade.Recebido;
                        break;

                    case "refunded":
                        status = (int)StatusMensalidade.Devolvido;
                        break;

                    case "contested":
                        status = (int)StatusMensalidade.Contestado;
                        break;
                }

                var mensalidade = _mensalidadeRep.GetSingle(data.CustomId);

                if (status == (int)StatusMensalidade.Recebido)
                {
                    mensalidade.DtRecebimento = DateTime.Now;
                    mensalidade.Status = (StatusMensalidade)status;
                    mensalidade.Token = retorno.Notification;
                }
                else if (mensalidade.Status != StatusMensalidade.Recebido)
                {
                    mensalidade.Status = (StatusMensalidade)status;
                    mensalidade.Token = retorno.Notification;
                }

                _mensalidadeRep.Edit(mensalidade);
                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.OK, new { });
                return response;
            });
        }


        [HttpGet]
        [Route("mensalidade/{tipo}")]
        public HttpResponseMessage GetMensalidade(HttpRequestMessage request, string tipo)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.All.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var mensalidades = _mensalidadeRep.All.Where(x => x.MembroId == membro.Id);
                var listaMensalidades = new List<MensalidadeViewModel>();

                switch (tipo)
                {
                    case "pendente":
                        mensalidades = mensalidades.Where(x => (x.Status == StatusMensalidade.Gerado || x.Status == StatusMensalidade.AguardandoPagamento) && x.Ativo);

                        break;
                    case "historico":
                        mensalidades = mensalidades.Where(x => x.Status != StatusMensalidade.Gerado &&
                                                               x.Status != StatusMensalidade.AguardandoPagamento);
                        break;
                    default:
                        break;
                }

                mensalidades.ToList().ForEach(x =>
                {
                    var mensal = new MensalidadeViewModel();

                    mensal.Id = x.Id;
                    mensal.Descricao = x.Descricao;

                    mensal.DtVencimento = tipo.Equals("pendente") ? x.DtVencimento :
                                                                    x.DtRecebimento != null ?
                                                                    x.DtRecebimento.Value.AddMonths(x.PlanoMensalidade.QtdMeses) :
                                                                    x.DtVencimento.AddMonths(x.PlanoMensalidade.QtdMeses);

                    mensal.Status = x.Status;
                    mensal.TipoPlano = x.PlanoMensalidade.Descricao;
                    mensal.TipoPagamentoId = (int)x.Detalhes.FirstOrDefault().Tipo;
                    mensal.Tipo = x.Detalhes.FirstOrDefault().Tipo == TipoMovimentacao.Credito ? "Crédito" :
                                      x.Detalhes.FirstOrDefault().Tipo == TipoMovimentacao.Debito ? "Débito" : "Boleto";
                    mensal.Total = x.Detalhes.Sum(s => s.Valor);
                    mensal.UrlBoleto = x.UrlPdf;
                    mensal.Ativo = x.Ativo;

                    listaMensalidades.Add(mensal);
                });

                response = request.CreateResponse(HttpStatusCode.OK, listaMensalidades);

                return response;
            });
        }


        [HttpGet]
        [Route("getPlanosMensalidade")]
        public HttpResponseMessage GetPlanosMensalidade(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.All.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var segmentoMembroId = membro.MembroCategorias.SelectMany(x => x.Categoria.SegmentosCategoria).FirstOrDefault().SegmentoId;
                var listaPlanosMensalidadesId = _planoSegmentoRep.FindBy(x => x.SegmentoId == segmentoMembroId && x.Ativo).Select(x => x.PlanoMensalidadeId).ToList();
                var planosMensalidade = _planosMensalidadeRep.FindBy(x => listaPlanosMensalidadesId.Contains(x.Id) && x.Ativo).ToList();

                var planoMensalidadeVM = new List<PlanoMensalidadeViewModel>();

                planosMensalidade.ForEach(x =>
                {
                    planoMensalidadeVM.Add(new PlanoMensalidadeViewModel
                    {
                        Id = x.Id,
                        Descricao = x.Descricao,
                        QtdMeses = x.QtdMeses,
                        Valor = x.Valor,
                        Ativo = x.Ativo

                    });
                });

                response = request.CreateResponse(HttpStatusCode.OK, planoMensalidadeVM);

                return response;
            });
        }

        [HttpPost]
        [Route("atualizarPlano/{IdPano:int=0}/{TipoPagamentoId:int=0}/{TrocarPlanoMembro:bool=false}")]
        public HttpResponseMessage AtualizarPlano(HttpRequestMessage request, int IdPano, bool TrocarPlanoMembro, int TipoPagamentoId, CartaoCreditoViewModel cartaoCreditoViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response;

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    var dtTrocaPlano = new DateTime();
                    var mensalidadeCriada = false;
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    Membro membro = _membroRep.FindBy(m => m.Pessoa.Id == usuario.Pessoa.Id).FirstOrDefault();
                    var _cartao = _cartaoCreditoRep.FirstOrDefault(x => x.MembroId == membro.Id);

                    var dtVencimento = new DateTime();
                    var dataHoje = DateTime.Now;
                    var dataFimGratuidade = membro.DataFimPeriodoGratuito ?? membro.DtCriacao;
                    var tipoPagamento = (TipoMovimentacao)TipoPagamentoId;

                    if (tipoPagamento == TipoMovimentacao.Boleto)
                    {
                        membro.PlanoMensalidadeId = IdPano;
                        dtVencimento = dataHoje.AddDays(1);
                    }
                    else
                    {
                        membro.PlanoMensalidadeId = IdPano;
                        _cartao.TokenCartaoGerenciaNet = cartaoCreditoViewModel.TokenCartaoGerenciaNet;
                        dtVencimento = DateTime.Now;

                        _cartaoCreditoRep.Edit(_cartao);
                    }

                    _membroRep.Edit(membro);

                    _unitOfWork.Commit();

                    if (!TrocarPlanoMembro)
                    {
                        mensalidadeCriada = _pagamentoService.GerarMensalidadeMembro(usuario, membro, dtVencimento, tipoPagamento);
                    }
                    else
                    {
                        dtTrocaPlano = _pagamentoService.TrocaPlano(usuario, membro, dtVencimento, tipoPagamento);
                        mensalidadeCriada = true;
                    }


                    response = request.CreateResponse(HttpStatusCode.OK, new { criouMensalidade = mensalidadeCriada, dataTrocaPlano = dtTrocaPlano.ToString("dd/MM/yyyy") });
                }

                return response;
            });
        }

        [HttpPost]
        [Route("cancelarPlanoMembro/{mensalidadeId:int=0}")]
        public HttpResponseMessage CancelarPlanoMembro(HttpRequestMessage request, int? mensalidadeId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response;
                bool cancelouPlano = false;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                Membro membro = _membroRep.FindBy(m => m.Pessoa.Id == usuario.Pessoa.Id).FirstOrDefault();


                if (mensalidadeId != null && mensalidadeId.Value > 0)
                    cancelouPlano = _pagamentoService.CancelarPlano(mensalidadeId.Value);

                if (mensalidadeId.Value == 0)
                {
                    var mensalidade = _mensalidadeRep.FindBy(x => x.MembroId == membro.Id && x.Status == StatusMensalidade.Recebido)
                                                            .OrderByDescending(o => o.Id).FirstOrDefault();

                    cancelouPlano = _pagamentoService.CancelarPlano(mensalidade.Id);
                }

                response = request.CreateResponse(HttpStatusCode.OK, cancelouPlano);


                return response;
            });
        }

    }
}