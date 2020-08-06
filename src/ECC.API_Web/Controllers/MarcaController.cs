using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeProduto;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro")]
    [RoutePrefix("api/marca")]
    public class MarcaController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Marca> _marcaRep;

        public MarcaController(IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Marca> marcaRep, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
            _marcaRep = marcaRep;
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
                List<Marca> marca = null;
                int totalMarca = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    marca = _marcaRep.FindBy(c => c.DescMarca.ToLower().Contains(filter))
                        .OrderBy(c => c.DescMarca)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalMarca = _marcaRep
                        .GetAll()
                        .Count(c => c.DescMarca.ToLower().Contains(filter));
                }
                else
                {
                    marca = _marcaRep.GetAll()
                        .OrderBy(c => c.DescMarca)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalMarca = _marcaRep.GetAll().Count();
                }

                IEnumerable<MarcaViewModel> marcaVM = Mapper.Map<IEnumerable<Marca>, IEnumerable<MarcaViewModel>>(marca);

                PaginacaoConfig<MarcaViewModel> pagSet = new PaginacaoConfig<MarcaViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMarca,
                    TotalPages = (int)Math.Ceiling((decimal)totalMarca / currentPageSize),
                    Items = marcaVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, MarcaViewModel marcaVM)
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


                    if (_marcaRep.FindBy(x => x.DescMarca == marcaVM.DescMarca).Any())
                    {

                        ModelState.AddModelError("Marca Existente", "Marca: " + marcaVM.DescMarca + " já existe .");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                            ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                .Select(m => m.ErrorMessage).ToArray());
                    }
                    else
                    {



                        Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                        Marca novaMarca = new Marca()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            DescMarca = marcaVM.DescMarca,
                            Ativo = marcaVM.Ativo
                        };
                        _marcaRep.Add(novaMarca);

                        _unitOfWork.Commit();

                        // Update view model
                        marcaVM = Mapper.Map<Marca, MarcaViewModel>(novaMarca);
                        response = request.CreateResponse(HttpStatusCode.Created, marcaVM);
                    }

                }

                return response;
            });

        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, MarcaViewModel marcaVM)
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
                    Marca novaMarca= _marcaRep.GetSingle(marcaVM.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    if (novaMarca.Produtos.Count > 0 && marcaVM.Ativo == false)
                        return request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Não é possível ser desativado");
                    else
                        novaMarca.AtualizarMarca(marcaVM, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    marcaVM = Mapper.Map<Marca, MarcaViewModel>(novaMarca);
                    response = request.CreateResponse(HttpStatusCode.OK, marcaVM);

                }

                return response;
            });
        }

    }
}
