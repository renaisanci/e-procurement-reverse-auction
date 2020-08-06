using ECC.API_Web.InfraWeb;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeAvisos;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;

namespace ECC.API_Web.Controllers
{


    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro, Fornecedor, Franquia")]
    [RoutePrefix("api/avisos")]
    public class AvisosController : ApiControllerBase
    {

        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;


        public AvisosController(
          IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Erro> _errosRepository,
           IEntidadeBaseRep<Avisos> avisosRep,
          IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _avisosRep = avisosRep;

        }

        [HttpGet]
        [Route("avisosUsuarioEmpresa")]
        public HttpResponseMessage GetAvisosUsuarioEmpresa(HttpRequestMessage request, int perfilModulo)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));



                var avisos = _avisosRep.GetAll()
                    .Where(x => x.UsuarioNotificadoId == usuario.Id && x.ModuloId == perfilModulo)
                    .GroupBy(x => new {x.TipoAvisosId, x.TipoAviso.Nome, x.TipoAviso.FontIcon, x.TipoAviso.Id})
                    .Select(g => new {g.Key.Nome, g.Key.FontIcon, qtd = g.Count(), g.Key.Id});

                int totalAvisos = 0;

                if (avisos.Any())
                {

                     totalAvisos = avisos.Sum(x => x.qtd);
                }
                else
                {
                    avisos = null;
                }

                response = request.CreateResponse(HttpStatusCode.OK, new { avisos, totalAvisos });

                return response;
            });
        }


        [HttpGet]
        [Route("avisosUsuarioEmpresaTp")]
        public HttpResponseMessage GetAvisosUsuarioEmpresa(HttpRequestMessage request, int perfilModulo, int TipoAvisosId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                 Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                 var avisos = _avisosRep.GetAll().Where(x => x.UsuarioNotificadoId == usuario.Id && x.ModuloId == perfilModulo && x.TipoAviso.Id == TipoAvisosId)
                    .Select(g => new { g.DescricaoAviso, g.URLPaginaDestino, g.ToolTip, g.TipoAvisosId, g.IdReferencia}).Distinct();


                response = request.CreateResponse(HttpStatusCode.OK, new { avisos });

                return response;
            });
        }

        [HttpGet]
        [Route("limpaAvisoPorReferenciaTipo")]
        public HttpResponseMessage LimpaAvisoPorReferenciaTipo(HttpRequestMessage request, int ReferenciaId, int TipoAvisosId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var aviso = _avisosRep.GetAll().FirstOrDefault(x => x.UsuarioNotificadoId == usuario.Id && x.TipoAviso.Id == TipoAvisosId && x.IdReferencia == ReferenciaId);
                _avisosRep.Delete(aviso);
                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.OK);

                return response;
            });
        }
    }
}
