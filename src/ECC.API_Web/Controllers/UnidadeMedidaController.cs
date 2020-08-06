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
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/unidadeMedida")]
    public class UnidadeMedidaController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;

        private readonly IEntidadeBaseRep<UnidadeMedida> _unidadeMedidaRep;
        
        public UnidadeMedidaController(IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<UnidadeMedida> unidadeMedidaRep, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
           
            _unidadeMedidaRep = unidadeMedidaRep;
            _usuarioRep = usuarioRep;


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
                List<UnidadeMedida> unidadeMedidas = null;
                int totalUnidadeMedida = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    unidadeMedidas = _unidadeMedidaRep.FindBy(c => c.DescUnidadeMedida.ToLower().Contains(filter))
                        .OrderBy(c => c.DescUnidadeMedida)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalUnidadeMedida = _unidadeMedidaRep
                        .GetAll()
                        .Count(c => c.DescUnidadeMedida.ToLower().Contains(filter));
                }
                else
                {
                    unidadeMedidas = _unidadeMedidaRep.GetAll()
                        .OrderBy(c => c.DescUnidadeMedida)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalUnidadeMedida = _unidadeMedidaRep.GetAll().Count();
                }

                IEnumerable<UnidadeMedidaViewModel> unidadeMedidasVM = Mapper.Map<IEnumerable<UnidadeMedida>, IEnumerable<UnidadeMedidaViewModel>>(unidadeMedidas);

                PaginacaoConfig<UnidadeMedidaViewModel> pagSet = new PaginacaoConfig<UnidadeMedidaViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalUnidadeMedida,
                    TotalPages = (int)Math.Ceiling((decimal)totalUnidadeMedida / currentPageSize),
                    Items = unidadeMedidasVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpGet]
        [Route("consultar/{filter?}")]
        public HttpResponseMessage Consultar(HttpRequestMessage request, string filter = null)
        {


            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<UnidadeMedida> unidadeMedidas = null;
                int totalUnidadeMedida = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    unidadeMedidas = _unidadeMedidaRep.FindBy(c => c.DescUnidadeMedida.ToLower().Contains(filter))
                        .OrderBy(c => c.DescUnidadeMedida)
                        .ToList();

                    totalUnidadeMedida = _unidadeMedidaRep
                        .GetAll()
                        .Count(c => c.DescUnidadeMedida.ToLower().Contains(filter));
                }
                else
                {
                    unidadeMedidas = _unidadeMedidaRep.GetAll()
                        .OrderBy(c => c.DescUnidadeMedida)
                    .ToList();

                    totalUnidadeMedida = _unidadeMedidaRep.GetAll().Count();
                }

                IEnumerable<UnidadeMedidaViewModel> unidadeMedidasVM = Mapper.Map<IEnumerable<UnidadeMedida>, IEnumerable<UnidadeMedidaViewModel>>(unidadeMedidas);
                
                response = request.CreateResponse(HttpStatusCode.OK, unidadeMedidasVM);

                return response;
            });
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, UnidadeMedidaViewModel unidadeMedidaViewModel)
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

                      
                       

                        UnidadeMedida novoUnidadeMedida = new UnidadeMedida()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                          DescUnidadeMedida= unidadeMedidaViewModel.DescUnidadeMedida,
                            Ativo = unidadeMedidaViewModel.Ativo

                        };
                        _unidadeMedidaRep.Add(novoUnidadeMedida);

                        _unitOfWork.Commit();

                        // Update view model
                        unidadeMedidaViewModel = Mapper.Map<UnidadeMedida, UnidadeMedidaViewModel>(novoUnidadeMedida);
                        response = request.CreateResponse(HttpStatusCode.Created, unidadeMedidaViewModel);

                    
                }

                return response;
            });
        }


        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, UnidadeMedidaViewModel unidadeMedidaViewModel)
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
                    UnidadeMedida _unidadeMedida = _unidadeMedidaRep.GetSingle(unidadeMedidaViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    _unidadeMedida.AtualizarUnidadeMedida(unidadeMedidaViewModel, usuario);

                    _unitOfWork.Commit();


 
                

                    // Update view model
                    unidadeMedidaViewModel = Mapper.Map<UnidadeMedida, UnidadeMedidaViewModel>(_unidadeMedida);
                    response = request.CreateResponse(HttpStatusCode.OK, unidadeMedidaViewModel);

                }

                return response;
            });
        }
    }
}