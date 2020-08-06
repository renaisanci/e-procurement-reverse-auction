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
using ECC.EntidadeProduto;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro")]
    [RoutePrefix("api/fabricante")]
    public class FabricanteController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Fabricante> _Fabricante;

        public FabricanteController(IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Fabricante> fabricante, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
             
            _usuarioRep = usuarioRep;
            _Fabricante = fabricante;
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
                List<Fabricante> fabricante = null;
                int totalFabricante = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    fabricante = _Fabricante.FindBy(c => c.DescFabricante.ToLower().Contains(filter) )
                        .OrderBy(c => c.DescFabricante)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalFabricante = _Fabricante
                        .GetAll()
                        .Count(c => c.DescFabricante.ToLower().Contains(filter));
                }
                else
                {
                    fabricante = _Fabricante.GetAll()
                        .OrderBy(c => c.DescFabricante)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalFabricante = _Fabricante.GetAll().Count();
                }

                IEnumerable<FabricanteViewModel> fabricanteVM = Mapper.Map<IEnumerable<Fabricante>, IEnumerable<FabricanteViewModel>>(fabricante);

                PaginacaoConfig<FabricanteViewModel> pagSet = new PaginacaoConfig<FabricanteViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalFabricante,
                    TotalPages = (int)Math.Ceiling((decimal)totalFabricante / currentPageSize),
                    Items = fabricanteVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, FabricanteViewModel fabricanteVM)
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

                    Fabricante novoFabricante = new Fabricante()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        DescFabricante = fabricanteVM.DescFabricante,
                        Ativo = fabricanteVM.Ativo
                    };
                    _Fabricante.Add(novoFabricante);

                    _unitOfWork.Commit();

                    // Update view model
                    fabricanteVM = Mapper.Map<Fabricante, FabricanteViewModel>(novoFabricante);
                    response = request.CreateResponse(HttpStatusCode.Created, fabricanteVM);


                }

                return response;
            });

        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, FabricanteViewModel fabricanteVM)
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
                    Fabricante novoFabricante = _Fabricante.GetSingle(fabricanteVM.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    if (novoFabricante.Produtos.Count > 0 && fabricanteVM.Ativo == false)
                        return request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Não é possível ser desativado");
                    else
                        novoFabricante.AtualizarFabricante(fabricanteVM, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    fabricanteVM = Mapper.Map<Fabricante, FabricanteViewModel>(novoFabricante);
                    response = request.CreateResponse(HttpStatusCode.OK, fabricanteVM);

                }

                return response;
            });
        }

    }
}