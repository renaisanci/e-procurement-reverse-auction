using System;
using System.Web.Http;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.Dados.Infra;
using System.Net.Http;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;

namespace ECC.API_Web.InfraWeb
{
    public class ApiControllerBase : ApiController
    {
        protected readonly IEntidadeBaseRep<Erro> _errosRepository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IEntidadeBaseRep<Usuario> _usuarioRepository;

        public ApiControllerBase(IEntidadeBaseRep<Usuario> usuarioRepository, IEntidadeBaseRep<Erro> errosRepository, IUnitOfWork unitOfWork)
        {
            _errosRepository = errosRepository;
            _unitOfWork = unitOfWork;
            _usuarioRepository = usuarioRepository;
        }

        public ApiControllerBase(IDataRepositoryFactory dataRepositoryFactory, IEntidadeBaseRep<Erro> errosRepository, IUnitOfWork unitOfWork)
        {
            _errosRepository = errosRepository;
            _unitOfWork = unitOfWork;
        }

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;

            try
            {
                response = function.Invoke();
            }
            catch (DbUpdateException ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            catch (DbEntityValidationException dbEx)
            {
                var strErro = dbEx.EntityValidationErrors.Aggregate(string.Empty, (current1, validationErrors) => validationErrors.ValidationErrors.Aggregate(current1, (current, validationError) => current + validationError.ErrorMessage + "\n"));
                response = request.CreateResponse(HttpStatusCode.InternalServerError, strErro);
            }
            catch (Exception ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        private void LogError(Exception ex)
        {
            try
            {
                var erro = new Erro()
                {
                    Mensagem = ex.Message,
                    StackTrace = ex.StackTrace.Replace('\\', ' ').Replace(':', ' ').Replace('/', ' ').Replace('\r', ' ').Replace('\n', ' '),
                    DtCriacao = DateTime.Now,
                    UsuarioCriacao = _usuarioRepository.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId())),
                    Ativo = true

                };

                _errosRepository.Add(erro);
                _unitOfWork.Commit();
            }
            catch
            {

            }
        }
    }
}