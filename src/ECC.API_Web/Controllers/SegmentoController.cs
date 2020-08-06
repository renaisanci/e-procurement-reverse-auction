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
    [RoutePrefix("api/segmento")]
    public class segmentoController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Segmento> _segmentoRep;

        public segmentoController(IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Segmento> segmentoRep, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {

            _usuarioRep = usuarioRep;
            _segmentoRep = segmentoRep;
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
                List<Segmento> segmento = null;
                int totalsegmento = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    segmento = _segmentoRep.FindBy(c => c.DescSegmento.ToLower().Contains(filter))
                        .OrderBy(c => c.DescSegmento)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalsegmento = _segmentoRep
                        .GetAll()
                        .Count(c => c.DescSegmento.ToLower().Contains(filter));
                }
                else
                {
                    segmento = _segmentoRep.GetAll()
                        .OrderBy(c => c.DescSegmento)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalsegmento = _segmentoRep.GetAll().Count();
                }

                IEnumerable<SegmentoViewModel> segmentoVM = Mapper.Map<IEnumerable<Segmento>, IEnumerable<SegmentoViewModel>>(segmento);

                PaginacaoConfig<SegmentoViewModel> pagSet = new PaginacaoConfig<SegmentoViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalsegmento,
                    TotalPages = (int)Math.Ceiling((decimal)totalsegmento / currentPageSize),
                    Items = segmentoVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, SegmentoViewModel segmentoVM)
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

                    Segmento novasegmento = new Segmento()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        DescSegmento = segmentoVM.DescSegmento,
                        Ativo = segmentoVM.Ativo
                    };
                    _segmentoRep.Add(novasegmento);

                    _unitOfWork.Commit();

                    // Update view model
                    segmentoVM = Mapper.Map<Segmento, SegmentoViewModel>(novasegmento);
                    response = request.CreateResponse(HttpStatusCode.Created, segmentoVM);


                }

                return response;
            });

        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, SegmentoViewModel segmentoVM)
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
                    Segmento novasegmento = _segmentoRep.GetSingle(segmentoVM.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    //if (novasegmento.Produtos.Count > 0 && segmentoVM.Ativo == false)
                    //   return request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Não é possível ser desativado");
                    //else
                    novasegmento.AtualizarSegmento(segmentoVM, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    segmentoVM = Mapper.Map<Segmento, SegmentoViewModel>(novasegmento);
                    response = request.CreateResponse(HttpStatusCode.OK, segmentoVM);

                }

                return response;
            });
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("loadsegmentos")]
        public HttpResponseMessage LoadSegmentos(HttpRequestMessage request)
        {

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Segmento> segmento = null;

                segmento = _segmentoRep.GetAll()
                    .OrderBy(c => c.DescSegmento)
                .ToList();

                IEnumerable<SegmentoViewModel> segmentoVM = Mapper.Map<IEnumerable<Segmento>, IEnumerable<SegmentoViewModel>>(segmento);

                response = request.CreateResponse(HttpStatusCode.OK, segmentoVM);

                return response;
            });
        }

    }
}
