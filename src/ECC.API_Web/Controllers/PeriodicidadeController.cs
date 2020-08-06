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
using ECC.Entidades.EntidadeComum;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/periodicidade")]
    public class PeriodicidadeController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;

        private readonly IEntidadeBaseRep<Periodicidade> _PeriodicidadeRep;
        
        public PeriodicidadeController(IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Periodicidade> PeriodicidadeRep, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
           
            _PeriodicidadeRep = PeriodicidadeRep;
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
                List<Periodicidade> Periodicidades = null;
                int totalPeriodicidade = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    Periodicidades = _PeriodicidadeRep.FindBy(c => c.DescPeriodicidade.ToLower().Contains(filter))
                        .OrderBy(c => c.DescPeriodicidade)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalPeriodicidade = _PeriodicidadeRep
                        .GetAll()
                        .Count(c => c.DescPeriodicidade.ToLower().Contains(filter));
                }
                else
                {
                    Periodicidades = _PeriodicidadeRep.GetAll()
                        .OrderBy(c => c.DescPeriodicidade)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalPeriodicidade = _PeriodicidadeRep.GetAll().Count();
                }

                IEnumerable<PeriodicidadeViewModel> PeriodicidadesVM = Mapper.Map<IEnumerable<Periodicidade>, IEnumerable<PeriodicidadeViewModel>>(Periodicidades);

                PaginacaoConfig<PeriodicidadeViewModel> pagSet = new PaginacaoConfig<PeriodicidadeViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalPeriodicidade,
                    TotalPages = (int)Math.Ceiling((decimal)totalPeriodicidade / currentPageSize),
                    Items = PeriodicidadesVM
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
                List<Periodicidade> Periodicidades = null;
                int totalPeriodicidade = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    Periodicidades = _PeriodicidadeRep.FindBy(c => c.DescPeriodicidade.ToLower().Contains(filter))
                        .OrderBy(c => c.DescPeriodicidade)
                        .ToList();

                    totalPeriodicidade = _PeriodicidadeRep
                        .GetAll()
                        .Count(c => c.DescPeriodicidade.ToLower().Contains(filter));
                }
                else
                {
                    Periodicidades = _PeriodicidadeRep.GetAll()
                        .OrderBy(c => c.DescPeriodicidade)
                    .ToList();

                    totalPeriodicidade = _PeriodicidadeRep.GetAll().Count();
                }

                IEnumerable<PeriodicidadeViewModel> PeriodicidadesVM = Mapper.Map<IEnumerable<Periodicidade>, IEnumerable<PeriodicidadeViewModel>>(Periodicidades);

                response = request.CreateResponse(HttpStatusCode.OK, PeriodicidadesVM);

                return response;
            });
        }

        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, PeriodicidadeViewModel PeriodicidadeViewModel)
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

                      
                       

                        Periodicidade novoPeriodicidade = new Periodicidade()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                          DescPeriodicidade= PeriodicidadeViewModel.DescPeriodicidade,
                            Ativo = PeriodicidadeViewModel.Ativo

                        };
                        _PeriodicidadeRep.Add(novoPeriodicidade);

                        _unitOfWork.Commit();

                        // Update view model
                        PeriodicidadeViewModel = Mapper.Map<Periodicidade, PeriodicidadeViewModel>(novoPeriodicidade);
                        response = request.CreateResponse(HttpStatusCode.Created, PeriodicidadeViewModel);

                    
                }

                return response;
            });
        }


        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, PeriodicidadeViewModel PeriodicidadeViewModel)
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
                    Periodicidade _Periodicidade = _PeriodicidadeRep.GetSingle(PeriodicidadeViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    _Periodicidade.AtualizarPeriodicidade(PeriodicidadeViewModel, usuario);

                    _unitOfWork.Commit();


 
                

                    // Update view model
                    PeriodicidadeViewModel = Mapper.Map<Periodicidade, PeriodicidadeViewModel>(_Periodicidade);
                    response = request.CreateResponse(HttpStatusCode.OK, PeriodicidadeViewModel);

                }

                return response;
            });
        }
    }
}