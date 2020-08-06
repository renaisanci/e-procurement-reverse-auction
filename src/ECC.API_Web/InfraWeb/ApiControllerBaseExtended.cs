using ECC.Dados.Infra;
using ECC.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ECC.Dados.Repositorio;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;


namespace ECC.API_Web.InfraWeb
{
    public class ApiControllerBaseExtended : ApiController
    {
        protected List<Type> _requiredRepositories;

        protected readonly IDataRepositoryFactory _dataRepositoryFactory;
        protected IEntidadeBaseRep<Erro> _errosRepository;
        protected  IEntidadeBaseRep<Usuario> _usuarioRepository;

        protected  IEntidadeBaseRep<Membro> _membroRepository;

        protected  IEntidadeBaseRep<Pessoa> _pessoaRepository;

        protected  IEntidadeBaseRep<PessoaJuridica> _pessoaJuridicaRepository;
     
        protected IUnitOfWork _unitOfWork;

        private HttpRequestMessage RequestMessage;

        public ApiControllerBaseExtended( IDataRepositoryFactory dataRepositoryFactory, IUnitOfWork unitOfWork)
        {
            _dataRepositoryFactory = dataRepositoryFactory;
            _unitOfWork = unitOfWork;
          

        }

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, List<Type> repos, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;

            try
            {
                RequestMessage = request;
                InitRepositories(repos);
                response = function.Invoke();
            }
            catch (DbUpdateException ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                LogError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        private void InitRepositories(List<Type> entities)
        {
            _errosRepository = _dataRepositoryFactory.GetDataRepository<Erro>(RequestMessage);

            if (entities.Any(e => e.FullName == typeof(Membro).FullName))
            {
                _membroRepository = _dataRepositoryFactory.GetDataRepository<Membro>(RequestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(Pessoa).FullName))
            {
                _pessoaRepository = _dataRepositoryFactory.GetDataRepository<Pessoa>(RequestMessage);
            }

            if (entities.Any(e => e.FullName == typeof(PessoaJuridica).FullName))
            {
                _pessoaJuridicaRepository = _dataRepositoryFactory.GetDataRepository<PessoaJuridica>(RequestMessage);
            }

        
           
        }

        private void LogError(Exception ex)
        {
            try
            {
                Erro _erro = new Erro()
                {
                    Mensagem = ex.Message,
            
                    DtCriacao = DateTime.Now,
                    UsuarioCriacao = _usuarioRepository.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId())),
                    Ativo = true

                };

                _errosRepository.Add(_erro);
                _unitOfWork.Commit();
            }
            catch { }
        }
    }
}