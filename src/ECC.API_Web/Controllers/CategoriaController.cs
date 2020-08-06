using System;
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
using ECC.Entidades;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using ECC.EntidadeProduto;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro, Fornecedor, Franquia")]
    [RoutePrefix("api/categoria")]
    public class CategoriaController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Categoria> _categoriaRep;
        private readonly IEntidadeBaseRep<FornecedorCategoria> _fornecedorCategoriaRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<SegmentoCategoria> _segmentoCategoriaRep;

        public CategoriaController(
                IEntidadeBaseRep<Categoria> categoriaRep,
                IEntidadeBaseRep<FornecedorCategoria> fornecedorCategoriaRep,
                IEntidadeBaseRep<MembroCategoria> membroCategoriaRep,
                IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Erro> _errosRepository,
                IEntidadeBaseRep<SegmentoCategoria> segmentoCategoriaRep,
                IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _categoriaRep = categoriaRep;
            _segmentoCategoriaRep = segmentoCategoriaRep;
            _fornecedorCategoriaRep = fornecedorCategoriaRep;
            _membroCategoriaRep = membroCategoriaRep;
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, CategoriaViewModel CategoriaViewModel)
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

                    var descCategoria =
                        _categoriaRep.GetAll()
                            .Where(c => c.DescCategoria.ToLower() == CategoriaViewModel.DescCategoria.ToLower()).
                            Select(c => c.DescCategoria).FirstOrDefault();

                    if (descCategoria == null)
                    {

                        Categoria novaCategoria = new Categoria()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Ativo = CategoriaViewModel.Ativo,
                            DescCategoria = CategoriaViewModel.DescCategoria
                        };

                        _categoriaRep.Add(novaCategoria);

                        _unitOfWork.Commit();
                        // Update view model
                        CategoriaViewModel = Mapper.Map<Categoria, CategoriaViewModel>(novaCategoria);

                        response = request.CreateResponse(HttpStatusCode.Created, CategoriaViewModel);
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.NotModified);

                    }
                }

                return response;
            });

        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, CategoriaViewModel categoriaViewModel)
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


                    Categoria novaCategoria = _categoriaRep.GetSingle(categoriaViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


                    var fornecedorCategoria = _fornecedorCategoriaRep.FindBy(x => x.CategoriaId == categoriaViewModel.Id).Count();
                    var membroCategoria = _membroCategoriaRep.FindBy(m => m.CategoriaId == categoriaViewModel.Id).Count();

                    if (fornecedorCategoria == 0 && membroCategoria == 0)
                    {
                        novaCategoria.AtualizarCategoria(categoriaViewModel, usuario);
                        _unitOfWork.Commit();

                        // Update view model
                        categoriaViewModel = Mapper.Map<Categoria, CategoriaViewModel>(novaCategoria);
                        response = request.CreateResponse(HttpStatusCode.OK, categoriaViewModel);
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, categoriaViewModel);

                    }


                }
                return response;
            });
        }


        [HttpGet]
        [Route("categoriaSegmento")]
        public HttpResponseMessage GetCategoriaSegmento(HttpRequestMessage request, int CategoriaId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var categoriaSegmentos = _segmentoCategoriaRep.GetAll().Where(x => x.Categoria.Id == CategoriaId).ToList();

                IEnumerable<SegmentoViewModel> catSegmentosVM = Mapper.Map<IEnumerable<SegmentoCategoria>, IEnumerable<SegmentoViewModel>>(categoriaSegmentos);

                response = request.CreateResponse(HttpStatusCode.OK, catSegmentosVM);

                return response;
            });
        }



        [HttpPost]
        [Route("inserirCategoriaSegmento/{categoriaId:int}")]
        public HttpResponseMessage InserirCategoriaSegmento(HttpRequestMessage request, int categoriaId, int[] listSegmento)
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
                    //Fornecedor _forencedor = _fornecedorRep.GetSingle(fornecedorId);
                    Categoria _categoria = _categoriaRep.GetSingle(categoriaId);

                    var categoriaSegmentos =
                        _segmentoCategoriaRep.GetAll().Where(x => x.CategoriaId == categoriaId);

                    if (categoriaSegmentos.Any())
                        _segmentoCategoriaRep.DeleteAll(categoriaSegmentos);

                    foreach (var item in listSegmento)
                    {
                        SegmentoCategoria SegmentoCategoria = new SegmentoCategoria()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            SegmentoId = item,
                            CategoriaId = categoriaId,
                            Ativo = true
                        };
                        _categoria.SegmentosCategoria.Add(SegmentoCategoria);
                    }

                    _categoriaRep.Edit(_categoria);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                }

                return response;
            });
        }


    }
}
