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
    [Authorize(Roles = "Admin, Membro")]
    [RoutePrefix("api/workflowstatus")]
    public class WorkflowStatusController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<WorkflowStatus> _workflowStatus;

        public WorkflowStatusController(IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<WorkflowStatus> workflowStatus, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
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
                List<WorkflowStatus> workflowStatus = null;
                int totalworkflowStatus = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    workflowStatus = _workflowStatus.FindBy(c => c.DescWorkslowStatus.ToLower().Contains(filter) )
                        .OrderBy(c => c.DescWorkslowStatus)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalworkflowStatus = _workflowStatus
                        .GetAll()
                        .Count(c => c.DescWorkslowStatus.ToLower().Contains(filter));
                }
                else
                {
                    workflowStatus = _workflowStatus.GetAll()
                        .OrderBy(c => c.DescWorkslowStatus)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalworkflowStatus = _workflowStatus.GetAll().Count();
                }

                IEnumerable<workflowStatusViewModel> workflowVM = Mapper.Map<IEnumerable<WorkflowStatus>, IEnumerable<workflowStatusViewModel>>(workflowStatus);

                PaginacaoConfig<workflowStatusViewModel> pagSet = new PaginacaoConfig<workflowStatusViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalworkflowStatus,
                    TotalPages = (int)Math.Ceiling((decimal)totalworkflowStatus / currentPageSize),
                    Items = workflowVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }



        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, workflowStatusViewModel workflowStatusViewModel)
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

                    WorkflowStatus novoStatus = new WorkflowStatus()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        DescWorkslowStatus = workflowStatusViewModel.DescWorkslowStatus,
                        Ativo = workflowStatusViewModel.Ativo
                        
                    };
                    _workflowStatus.Add(novoStatus);

                    _unitOfWork.Commit();

                    // Update view model
                    workflowStatusViewModel = Mapper.Map<WorkflowStatus, workflowStatusViewModel>(novoStatus);
                    response = request.CreateResponse(HttpStatusCode.Created, workflowStatusViewModel);


                }

                return response;
            });

        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, workflowStatusViewModel workflowStatusViewModel)
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
                    WorkflowStatus novoStatus = _workflowStatus.GetSingle(workflowStatusViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    if (novoStatus.StatusSistemas.Count > 0 && workflowStatusViewModel.Ativo == false)
                        return request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Workflow utilizado em algum status, não é possível ser desativado");
                    else
                        novoStatus.AtualizarWorkflowStatus(workflowStatusViewModel, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    workflowStatusViewModel = Mapper.Map<WorkflowStatus, workflowStatusViewModel>(novoStatus);
                    response = request.CreateResponse(HttpStatusCode.OK, workflowStatusViewModel);

                }

                return response;
            });
        }

    }
}
