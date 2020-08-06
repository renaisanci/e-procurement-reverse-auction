using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using ECC.EntidadeParametroSistema;
using ECC.EntidadePessoa;
using ECC.EntidadeRecebimento;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using Microsoft.AspNet.Identity;

namespace ECC.API_Web.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "Admin, Membro")]
    [RoutePrefix("api/cartaocredito")]
    public class CartaoCreditoController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IEntidadeBaseRep<CartaoBandeira> _cartaoBandeiraRep;
        private readonly IEntidadeBaseRep<CartaoCredito> _cartaoCreditoRep;
        private readonly IEncryptionService _encryptionService;


        public CartaoCreditoController(
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
            IEntidadeBaseRep<CartaoBandeira> cartaoBandeiraRep,
            IEntidadeBaseRep<CartaoCredito> cartaoCreditoRep,
            IEncryptionService encryptionService,
            IEntidadeBaseRep<Erro> _errosRepository,
            IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _membroRep = membroRep;
            _parametroSistemaRep = parametroSistemaRep;
            _cartaoBandeiraRep = cartaoBandeiraRep;
            _cartaoCreditoRep = cartaoCreditoRep;
            _encryptionService = encryptionService;
        }

        [HttpGet]
        [Route("getCartaoCredito/{cartaoId:int}")]
        public HttpResponseMessage GetCartaoCredito(HttpRequestMessage request, int cartaoId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var cartao = new List<CartaoCredito>();
                var cartaoCreditoVM = new List<CartaoCreditoViewModel>();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var membro = this._membroRep.GetAll().FirstOrDefault(x => x.PessoaId.Equals(usuario.PessoaId));

                if (cartaoId > 0)
                    cartao = _cartaoCreditoRep.FindBy(x => x.Id == cartaoId && x.MembroId == membro.Id && x.Ativo).ToList();
                else
                    cartao = _cartaoCreditoRep.FindBy(x => x.MembroId == membro.Id && x.Ativo).ToList();

                cartao.ForEach(x =>
                {
                    cartaoCreditoVM.Add(new CartaoCreditoViewModel
                    {
                        Id = x.Id,
                        Nome = x.Nome,
                        Numero = x.Numero,
                        DataVencimento = x.DataVencimento.ToString("MM/yyyy"),
                        DescricaoCartaoBandeira = x.CartaoBandeira.Descricao,
                        //Cvv = x.Cvv,
                        Padrao = x.Padrao,
                        Ativo = x.Ativo
                    });
                });

                response = request.CreateResponse(HttpStatusCode.OK, cartaoCreditoVM);

                return response;
            });
        }

        [HttpPost]
        [Route("inserirAlterarCartaoCredito")]
        public HttpResponseMessage InserirCartaoCredito(HttpRequestMessage request, CartaoCreditoViewModel cartaoCreditoViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var cartao = new CartaoCredito();

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                            .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {

                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    var membro = this._membroRep.GetAll().FirstOrDefault(x => x.PessoaId.Equals(usuario.PessoaId));


                    if (cartaoCreditoViewModel.Padrao)
                    {
                        var _param = new[]
                         {
                               new SqlParameter { ParameterName = "@PADRAO", SqlDbType = System.Data.SqlDbType.Bit, Value = 0},
                               new SqlParameter { ParameterName = "@PADRAO1", SqlDbType = System.Data.SqlDbType.Bit, Value = 1 }
                         };

                        var sql = "UPDATE CartaoCredito set Padrao = @PADRAO WHERE Padrao = @PADRAO1";
                        _cartaoCreditoRep.ExecuteWithStoreProcedure(sql, _param);
                        _unitOfWork.Commit();
                    }

                    if (cartaoCreditoViewModel.Id != null && cartaoCreditoViewModel.Id > 0)
                    {

                        cartao = _cartaoCreditoRep.FirstOrDefault(x => x.Id == cartaoCreditoViewModel.Id);


                        cartao.Nome = cartaoCreditoViewModel.Nome;
                        cartao.Numero = cartaoCreditoViewModel.Numero;
                        cartao.DataVencimento = Convert.ToDateTime(cartaoCreditoViewModel.DataVencimento);
                        cartao.Cvc = $"{usuario.Chave}|{_encryptionService.EncryptCvv(cartaoCreditoViewModel.Cvc)}";
                        cartao.TokenCartaoGerenciaNet = string.Empty;
                        cartao.CartaoBandeiraId = cartaoCreditoViewModel.CartaoBandeiraId;
                        cartao.Padrao = cartaoCreditoViewModel.Padrao;
                        cartao.MembroId = membro.Id;
                        cartao.Ativo = true;
                        cartao.UsuarioAlteracaoId = usuario.Id;
                        cartao.UsuarioAlteracao = usuario;
                        cartao.DtAlteracao = DateTime.Now;


                        _cartaoCreditoRep.Edit(cartao);
                        _unitOfWork.Commit();

                    }
                    else
                    {

                        cartao = new CartaoCredito
                        {
                            Nome = cartaoCreditoViewModel.Nome,
                            Numero = cartaoCreditoViewModel.Numero,
                            DataVencimento = Convert.ToDateTime(cartaoCreditoViewModel.DataVencimento),
                            CartaoBandeiraId = cartaoCreditoViewModel.CartaoBandeiraId,
                            Cvc = $"{usuario.Chave}|{_encryptionService.EncryptCvv(cartaoCreditoViewModel.Cvc)}",
                            TokenCartaoGerenciaNet = string.Empty,
                            Padrao = true,
                            MembroId = membro.Id,
                            Ativo = true,
                            UsuarioCriacaoId = usuario.Id,
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                        };

                        _cartaoCreditoRep.Add(cartao);
                        _unitOfWork.Commit();

                        cartaoCreditoViewModel.Id = cartao.Id;
                    }

                    response = request.CreateResponse(HttpStatusCode.Created, cartaoCreditoViewModel);

                }

                return response;
            });
        }

        [HttpPost]
        [Route("removerCartaoCredito/{cartaoId:int}")]
        public HttpResponseMessage RemoverCartaoCredito(HttpRequestMessage request, int cartaoId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = this._membroRep.GetAll().FirstOrDefault(x => x.PessoaId.Equals(usuario.PessoaId));

                if (cartaoId > 0)
                {
                    var cartaoCredito = _cartaoCreditoRep.FirstOrDefault(x => x.Id == cartaoId && x.MembroId == membro.Id);

                    _cartaoCreditoRep.Delete(cartaoCredito);
                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, new { });
                }

                return response;
            });
        }

        [HttpGet]
        [Route("getCartaoBandeira")]
        public HttpResponseMessage GetCartaoBandeira(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var cartaoBandeiraVM = new List<CartaoBandeiraViewModel>();

                var cartaoBandeira = _cartaoBandeiraRep.FindBy(x => x.Ativo).ToList();

                cartaoBandeira.ForEach(x =>
                {
                    cartaoBandeiraVM.Add(new CartaoBandeiraViewModel
                    {
                        Id = x.Id,
                        Descricao = x.Descricao,
                        Ativo = x.Ativo

                    });
                });

                response = request.CreateResponse(HttpStatusCode.OK, cartaoBandeiraVM);

                return response;
            });
        }

        [HttpGet]
        [Route("existeCartaoCadastradoMembro")]
        public HttpResponseMessage ExisteCartaoCadastradoMembro(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var cartaoBandeiraVM = new List<CartaoBandeiraViewModel>();
                var cartaoCredito = new CartaoCreditoViewModel();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var membro = _membroRep.GetAll().FirstOrDefault(x => x.PessoaId.Equals(usuario.PessoaId));

                var _cartao = _cartaoCreditoRep.FirstOrDefault(x => x.MembroId == membro.Id && x.Padrao && x.Ativo);

                if (_cartao != null)
                {
                    cartaoCredito = new CartaoCreditoViewModel
                    {
                        Id = _cartao.Id,
                        Nome = _cartao.Nome,
                        Numero = _cartao.Numero,
                        CartaoBandeiraId = _cartao.CartaoBandeiraId,
                        DataVencimento = _cartao.DataVencimento.ToString("MM/yyyy"),
                        Padrao = _cartao.Padrao,
                        Ativo = _cartao.Ativo
                    };
                }
                else
                    cartaoCredito = null;


                Object planoIdFormaPagtoId = new
                {
                    IdPano = membro.PlanoMensalidadeId,
                    TipoPagamentoId = membro.Mensalidades != null && membro.Mensalidades.Count > 0 ?
                                      membro.Mensalidades.OrderByDescending(x => x.Id).FirstOrDefault().Detalhes.FirstOrDefault().Tipo : 0,
                    MensalidadePaga = membro.Mensalidades != null && membro.Mensalidades.Count > 0 ?
                                      membro.Mensalidades.OrderByDescending(x => x.Id)
                                                        .FirstOrDefault(x => x.Status == StatusMensalidade.Recebido ||
                                                                             x.Status == StatusMensalidade.AguardandoPagamento) != null : false
                };

                response = request.CreateResponse(HttpStatusCode.OK, new { cartao = cartaoCredito, planoMembro = planoIdFormaPagtoId });

                return response;
            });
        }
    }
}