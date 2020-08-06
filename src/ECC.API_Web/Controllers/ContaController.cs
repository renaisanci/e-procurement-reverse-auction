using ECC.API_Web.InfraWeb;
using ECC.Servicos.Abstrato;
using System.Web.Http;
using ECC.Entidades;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using System.Net.Http;
using ECC.Servicos.Util;
using System.Net;
using System;
using System.Web;
using ECC.API_Web.Models;
using ECC.EntidadeUsuario;
using System.Linq;
using ECC.EntidadeEmail;
using Microsoft.AspNet.Identity;
using ECC.EntidadeParametroSistema;

namespace ECC.API_Web.Controllers
{
    [Authorize(Roles = "Admin, Membro, Fornecedor, Franquia")]
    [RoutePrefix("api/conta")]
    public class ContaController : ApiControllerBase
    {
        private readonly IMembershipService _membershipService;
        private readonly IEmailService _emailService;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<UsuarioGrupo> _usuarioGrupoRep;
        private readonly IEntidadeBaseRep<PermissaoGrupo> _grupoPermissaoRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistema;
      
        public ContaController(IEntidadeBaseRep<PermissaoGrupo> grupoPermissaoRep,
                               IEntidadeBaseRep<UsuarioGrupo> usuarioGrupoRep,
                               IEntidadeBaseRep<TemplateEmail> templateEmailRep,
                               IEmailService emailService,
                               IEntidadeBaseRep<Usuario> usuarioRep,
                               IMembershipService membershipService,
                               IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
                               IEntidadeBaseRep<Erro> _errosRepository,
                               IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _membershipService = membershipService;
            _emailService = emailService;
            _templateEmailRep = templateEmailRep;
            _usuarioRep = usuarioRep;
            _usuarioGrupoRep = usuarioGrupoRep;
            _grupoPermissaoRep = grupoPermissaoRep;
            _parametroSistema = parametroSistemaRep;
        }
       
        [AllowAnonymous]
        [Route("autenticar")]
        [HttpPost]
        public HttpResponseMessage Login(HttpRequestMessage request, LoginViewModel usuario)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                
                if (ModelState.IsValid)
                {
                    MembershipContext _userContext = _membershipService.ValidateUser(usuario.UsuarioEmail, usuario.Senha, usuario.perfilModulo);

                    var faturaVencida = _membershipService.PagamentosFaturasMensalidadeVencidos(usuario.perfilModulo,usuario.UsuarioEmail);
                                       
                    if (_userContext.Usuario != null)
                    {

                       response = request.CreateResponse(HttpStatusCode.OK, 
                            new { success = true, usuarioNome = _userContext.Usuario.UsuarioNome,
                                usuarioFlgTrocaSenha = _userContext.Usuario.FlgTrocaSenha,
                                usuarioChave = _userContext.Usuario.Chave,
                                usuarioId = _userContext.Usuario.Id,
                                faturaVenceu = faturaVencida,
                                tipoPessoa = _userContext.Usuario.Pessoa.TipoPessoa
                            });
                        
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = false });
                    }
                }
                else
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = false });

                return response;
            });
        }

        [Route("logout")]
        [HttpPost]
        public HttpResponseMessage Logout(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = this._usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                usuario.TokenSignalR = "";
                usuario.Logado = false;
                usuario.DtUsuarioSaiu = DateTime.Now;
                this._usuarioRep.Edit(usuario);
                this._unitOfWork.Commit();

                return request.CreateResponse(HttpStatusCode.OK, new { success = true });
            });
        }
        
        [AllowAnonymous]
        [Route("recuperarsenha")]
        [HttpPost]
        public HttpResponseMessage EnviarEmail(HttpRequestMessage request, LoginViewModel usuario)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                //remove a validacao da propriedade da senha, pois a propriedade esta sendo validada 
                //lá no fluent validator e neste caso não prcisa validar                 
                ModelState.Remove("usuario.Senha");

                if (ModelState.IsValid)
                {
                    RecuperaSenha recuperaSenha = new RecuperaSenha();

                    recuperaSenha.Chave = Guid.NewGuid();

                    Usuario userRecupera = _membershipService.GetUserPorEmail(usuario.UsuarioEmail, usuario.perfilModulo);

                    if (userRecupera != null)
                    {
                        recuperaSenha.UsuarioCriacao = userRecupera;
                        recuperaSenha.UsuarioCriacaoId = userRecupera.Id;
                        recuperaSenha.DtCriacao = DateTime.Now;
                        recuperaSenha.UsuarioId = userRecupera.Id;
                        recuperaSenha.DtExpira = DateTime.Now.AddDays(1); // tem um dia para trocar a senha
                        recuperaSenha.Usuario = userRecupera;
                        _membershipService.CreateRecuperaSenha(recuperaSenha);

                        // Update view model
                        string strAmbiente = Environment.GetEnvironmentVariable("Amb_EconomizaJa").ToUpper();
                        string localCodigo = $"URL_Mod{usuario.perfilModulo}_Amb{strAmbiente}".ToString();
                        string URL_Modulo_Ambiente = _parametroSistema.GetAll().FirstOrDefault(fd => fd.Codigo == localCodigo).Valor;

                        RecuperaSenhaViewModel recuperaSenhaVM = AutoMapper.Mapper.Map<RecuperaSenha, RecuperaSenhaViewModel>(recuperaSenha);

                        //	recuperaSenhaVM.URL = "http://localhost:1312/#/recuperasenha?Q="; //ADM local
                        recuperaSenhaVM.URL = $"{URL_Modulo_Ambiente}/#/recuperasenha?Q=";

                        //Envia email de Recuperar Senha para o Usuário
                        var template = _templateEmailRep.GetSingle(2).Template;
                        _emailService.EnviarEmailViaRobo(userRecupera, "Recuperar Senha - Economiza Já", recuperaSenhaVM.Usuarioemail,
                            _emailService.MontaEmail(recuperaSenhaVM, template), Origem.RecuperarSenha);


                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true, usuarioEmail = userRecupera.UsuarioEmail });
                    }
                    else
                    {

                        response = request.CreateResponse(HttpStatusCode.OK, new { success = false });
                    }
                }
                else
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = false });

                return response;
            });
        }
        
        [AllowAnonymous]
        [Route("getrecuperarsenha")]
        [HttpPost]
        public HttpResponseMessage loadRecuperarsenha(HttpRequestMessage request, RecuperaSenha recuperaSenha)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                RecuperaSenha localRecuperaSenha = _membershipService.getRecuperaSenha(recuperaSenha);

                if (localRecuperaSenha != null)
                {
                    //recuperaSenha.Usuario = locaRecuperaSenha.Usuario;
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true, usuarioNome = localRecuperaSenha.Usuario.UsuarioNome, usuarioId = localRecuperaSenha.Usuario.Id, usuarioChave = localRecuperaSenha.Usuario.Chave });
                }

                else
                {
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = false });
                }
                return response;
            });
        }
        
        [AllowAnonymous]
        [Route("alterarsenhausuario")]
        [HttpPost]
        public HttpResponseMessage AlterarSenhaUser(HttpRequestMessage request, Usuario user)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                _membershipService.AlterarSenhaUsuario(user);

                response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;
            });
        }
        
        [Route("permissaourl/{url}")]
        [HttpPost]
        public HttpResponseMessage PermissaoUrl(HttpRequestMessage request, string url)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                bool booTemAcesso = false;
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var UsuarioGrupos = _usuarioGrupoRep.GetAll().Where(ug => ug.UsuarioId == usuario.Id).ToList();
                var menuVMPermissao = _grupoPermissaoRep.GetAll().Where(x => x.Menu.Url != null
                                        && x.Menu.Url.ToUpper() == "#/" + url.ToUpper()
                                        && x.Menu.Ativo == true);

                var menus = from ug in UsuarioGrupos
                            join mp in menuVMPermissao
                                 on ug.GrupoId equals mp.GrupoId
                            //where mp.Menu.Url.ToUpper() == "#/" + url.ToUpper()
                            select mp;


                if (menus.Any())
                    booTemAcesso = true;

                response = request.CreateResponse(HttpStatusCode.OK, new { success = true, temacesso = booTemAcesso, novaurl = url });

                return response;
            });
        }
    }
}
