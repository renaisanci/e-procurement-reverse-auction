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
using ECC.EntidadeEntrega;
using ECC.EntidadePessoa;

namespace ECC.API_Web.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro, Fornecedor")]
    [RoutePrefix("api/entrega")]
    public class EntregaController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Entrega> _entregaRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        
        public EntregaController(
            IEntidadeBaseRep<Usuario> usuarioRep, 
            IEntidadeBaseRep<Entrega> entrega,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<Erro> _errosRepository, 
            IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
            _entregaRep = entrega;
            _fornecedorRep = fornecedorRep;
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
                List<Entrega> entrega = null;
                int totalentrega = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    entrega = _entregaRep.FindBy(c => c.DescEntrega.ToLower().Contains(filter))
                        .OrderBy(c => c.DescEntrega)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalentrega = _entregaRep
                        .GetAll()
                        .Count(c => c.DescEntrega.ToLower().Contains(filter));
                }
                else
                {
                    entrega = _entregaRep.GetAll()
                        .OrderBy(c => c.DescEntrega)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalentrega = _entregaRep.GetAll().Count();
                }

                IEnumerable<EntregaViewModel> entregaVM = Mapper.Map<IEnumerable<Entrega>, IEnumerable<EntregaViewModel>>(entrega);

                PaginacaoConfig<EntregaViewModel> pagSet = new PaginacaoConfig<EntregaViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalentrega,
                    TotalPages = (int)Math.Ceiling((decimal)totalentrega / currentPageSize),
                    Items = entregaVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, EntregaViewModel entregaViewModel)
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
                    
                    Entrega novoentrega = new Entrega()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Ativo = entregaViewModel.Ativo,
                        DescEntrega = entregaViewModel.DescEntrega
                    };
                    _entregaRep.Add(novoentrega);

                    _unitOfWork.Commit();

                    // Update view model
                    entregaViewModel = Mapper.Map<Entrega, EntregaViewModel>(novoentrega);
                    response = request.CreateResponse(HttpStatusCode.Created, entregaViewModel);
					
                }

                return response;
            });

        }


        [HttpGet]
        [Route("entregaStatus")]
        public HttpResponseMessage EntregaStatus(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var entrega = _entregaRep.GetAll().Where(m => m.Ativo);

                IEnumerable<EntregaViewModel> entregaVM = Mapper.Map<IEnumerable<Entrega>, IEnumerable<EntregaViewModel>>(entrega);

                response = request.CreateResponse(HttpStatusCode.OK, entregaVM);

                return response;
            });
        }


    }
}
