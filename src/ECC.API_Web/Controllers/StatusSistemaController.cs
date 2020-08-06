using System;
using System.Linq;
using System.Net;
using ECC.API_Web.InfraWeb;
using System.Web.Http;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.Dados.Infra;
using System.Net.Http;
using System.Web;
using System.Web.Http.Cors;
using AutoMapper;
using ECC.API_Web.Models;

using ECC.EntidadeUsuario;

using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.EntidadeStatus;

namespace ECC.API_Web.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro, Fornecedor")]
    [RoutePrefix("api/statusSistema")]
    public class StatusSistemaController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<StatusSistema> _statusSistema;
        private readonly IEntidadeBaseRep<WorkflowStatus> _workflowStatus;

        public StatusSistemaController(IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<StatusSistema> statusSistema,IEntidadeBaseRep<WorkflowStatus> workflowStatus , IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
            _statusSistema = statusSistema;
            _workflowStatus = workflowStatus;
        }

        [HttpGet]
        [Route("pesquisar/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {


            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<StatusSistema> statusSistema = null;
                int totalStatusSistema = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    statusSistema = _statusSistema.FindBy(c => c.DescStatus.ToLower().Contains(filter))
                        .OrderBy(c => c.DescStatus)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalStatusSistema = _statusSistema
                        .GetAll()
                        .Count(c => c.DescStatus.ToLower().Contains(filter));
                }
                else
                {
                    statusSistema = _statusSistema.GetAll()
                        .OrderBy(c => c.DescStatus)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalStatusSistema = _statusSistema.GetAll().Count();
                }

                IEnumerable<StatusSistemaViewModel> statusSistemaVM = Mapper.Map<IEnumerable<StatusSistema>, IEnumerable<StatusSistemaViewModel>>(statusSistema);

                PaginacaoConfig<StatusSistemaViewModel> pagSet = new PaginacaoConfig<StatusSistemaViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalStatusSistema,
                    TotalPages = (int)Math.Ceiling((decimal)totalStatusSistema / currentPageSize),
                    Items = statusSistemaVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, StatusSistemaViewModel statusSistemaViewModel)
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
                    
                    StatusSistema novoStatusSistema = new StatusSistema()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Ativo = statusSistemaViewModel.Ativo,
                        DescStatus = statusSistemaViewModel.DescStatus,
                        Ordem = statusSistemaViewModel.Ordem,
                        WorkflowStatusId = statusSistemaViewModel.WorkflowStatusId
                    };
                    _statusSistema.Add(novoStatusSistema);

                    _unitOfWork.Commit();

                    // Update view model
                    statusSistemaViewModel = Mapper.Map<StatusSistema, StatusSistemaViewModel>(novoStatusSistema);
                    response = request.CreateResponse(HttpStatusCode.Created, statusSistemaViewModel);


                }

                return response;
            });

        }


        [HttpGet]
        [Route("workflowstatus")]
        public HttpResponseMessage Workflowstatus(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var workflowStatusSistema = _workflowStatus.GetAll().Where(m => m.Ativo);

                IEnumerable<workflowStatusViewModel> workflowVM = Mapper.Map<IEnumerable<WorkflowStatus>, IEnumerable<workflowStatusViewModel>>(workflowStatusSistema);

                response = request.CreateResponse(HttpStatusCode.OK, workflowVM);

                return response;
            });
        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, StatusSistemaViewModel StatusSistemaVM)
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
                    StatusSistema novoStatus = _statusSistema.GetSingle(StatusSistemaVM.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    novoStatus.AtualizarStatusSistema(StatusSistemaVM, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    StatusSistemaVM = Mapper.Map<StatusSistema, StatusSistemaViewModel>(novoStatus);
                    response = request.CreateResponse(HttpStatusCode.OK, StatusSistemaVM);

                }

                return response;
            });
        }

        [HttpPost]
        [Route("deletar")]
        public HttpResponseMessage Deletar(HttpRequestMessage request, StatusSistemaViewModel StatusSistemaVM)
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
                    StatusSistema novoStatus = _statusSistema.GetSingle(StatusSistemaVM.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    _statusSistema.Delete(novoStatus);

                    _unitOfWork.Commit();

                    // Update view model
                    StatusSistemaVM = Mapper.Map<StatusSistema, StatusSistemaViewModel>(novoStatus);
                    response = request.CreateResponse(HttpStatusCode.OK, StatusSistemaVM);

                }

                return response;
            });
        }



        [HttpGet]
        [Route("statusPorWorkflow")]
        public HttpResponseMessage StatusPorWorkflow(HttpRequestMessage request, int workflowStatusId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var statusSistema =
                    _statusSistema.GetAll().Where(m => m.Ativo && 
                                                       m.WorkflowStatusId == workflowStatusId)
                                                       .ToList();

                IEnumerable<StatusSistemaViewModel> statusVM = Mapper.Map<IEnumerable<StatusSistema>, IEnumerable<StatusSistemaViewModel>>(statusSistema);

                response = request.CreateResponse(HttpStatusCode.OK, statusVM);

                return response;
            });
        }

        [HttpGet]
        [Route("statusPorWorkflowPedidoPromocao")]
        public HttpResponseMessage StatusPorWorkflowPedidoPromocao(HttpRequestMessage request, int workflowStatusId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var statusSistema =
                    _statusSistema.GetAll().Where(m => m.WorkflowStatusId == workflowStatusId)
                    .ToList();

                IEnumerable<StatusSistemaViewModel> statusVM = Mapper.Map<IEnumerable<StatusSistema>, IEnumerable<StatusSistemaViewModel>>(statusSistema);

                response = request.CreateResponse(HttpStatusCode.OK, statusVM);

                return response;
            });
        }

        [HttpGet]
		[Route("statusPorWorkflowEntrega")]
		public HttpResponseMessage statusPorWorkflowEntrega(HttpRequestMessage request, int workflowStatusId)
		{
			return CreateHttpResponse(request, () =>
			{
				HttpResponseMessage response = null;
				var statusSistema =
					_statusSistema.GetAll().Where(m => m.Ativo && m.Id == 29 || m.Id == 30).ToList();

				IEnumerable<StatusSistemaViewModel> statusVM = Mapper.Map<IEnumerable<StatusSistema>, IEnumerable<StatusSistemaViewModel>>(statusSistema);

				response = request.CreateResponse(HttpStatusCode.OK, statusVM);

				return response;
			});
		}

	}
}
