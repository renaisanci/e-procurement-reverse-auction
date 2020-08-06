
using System.Collections.Generic;
using System.Net;
using ECC.API_Web.InfraWeb;
using System.Web.Http;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.Dados.Infra;
using System.Net.Http;
using System.Web.Http.Cors;
using ECC.API_Web.Models;
using ECC.EntidadeUsuario;


namespace ECC.API_Web.Controllers
{
    //enable cors temporario por não estar no mesmo dominio
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/painel")]
    public class PainelController : ApiControllerBase
    {

        public PainelController(IEntidadeBaseRep<Usuario> _usuarioRepository, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(_usuarioRepository, _errosRepository, _unitOfWork)
        {

        }

        [Authorize(Roles = "Admin")]
        [Route("adm")]
        public HttpResponseMessage GetAdm(HttpRequestMessage request)
        {
            //este codigo foi o teste pata consumir a api do MVC
            List<LoginViewModel> listLogin = new List<LoginViewModel>();

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste1" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste2" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste3" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste4" });

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;


                response = request.CreateResponse<IEnumerable<LoginViewModel>>(HttpStatusCode.OK, listLogin);
                return response;
            });
        }


        [Authorize(Roles = "Membro")]
        [Route("membro")]
        public HttpResponseMessage GetMembro(HttpRequestMessage request)
        {
            //este codigo foi o teste pata consumir a api do MVC
            List<LoginViewModel> listLogin = new List<LoginViewModel>();

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste1" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste2" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste3" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste4" });

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;


                response = request.CreateResponse<IEnumerable<LoginViewModel>>(HttpStatusCode.OK, listLogin);
                return response;
            });
        }


        [Authorize(Roles = "Fornecedor")]
        [Route("fornecedor")]
        public HttpResponseMessage GetForncedor(HttpRequestMessage request)
        {
 
            //este codigo foi o teste pata consumir a api do MVC
            List<LoginViewModel> listLogin = new List<LoginViewModel>();

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste1" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste2" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste3" });

            listLogin.Add(new LoginViewModel() { UsuarioEmail = "Teste4" });

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;


                response = request.CreateResponse<IEnumerable<LoginViewModel>>(HttpStatusCode.OK, listLogin);
                return response;
            });
        }

    }
}
