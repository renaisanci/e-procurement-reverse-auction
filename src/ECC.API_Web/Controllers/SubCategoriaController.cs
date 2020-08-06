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
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Configuration;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.EntidadeProduto;

namespace ECC.API_Web.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro, Franquia")]
    [RoutePrefix("api/subcategoria")]
    public class SubCategoriaController : ApiControllerBase
    {

        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<SubCategoria> _subCategoriaRep;
        private readonly IEntidadeBaseRep<Categoria> _categoriaRep;

        public SubCategoriaController(IEntidadeBaseRep<SubCategoria> subCategoriaRep, IEntidadeBaseRep<Categoria> categoriaRep, IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _subCategoriaRep = subCategoriaRep;
            _categoriaRep = categoriaRep;
        }
        
        [HttpGet]
        [Route("pesquisar/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetSubCategoriaPaginacao(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<SubCategoria> subCategorias = null;
                int totalSubCategorias = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    subCategorias = _subCategoriaRep.FindBy(c => c.DescSubCategoria.ToLower().Contains(filter) ||
                            c.Ativo.ToString() == filter || c.Categoria.DescCategoria.ToLower().Contains(filter) || c.Id.ToString() == filter)
                           .OrderBy(c => c.Categoria.DescCategoria)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalSubCategorias = _subCategoriaRep
                        .GetAll()
                        .Count(c => c.DescSubCategoria.ToLower().Contains(filter));
                }
                else
                {
                    subCategorias = _subCategoriaRep.GetAll()
                        .OrderBy(c => c.DescSubCategoria)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalSubCategorias = _subCategoriaRep.GetAll().Count();
                }

                IEnumerable<SubCategoriaViewModel> subCategoriasVM = Mapper.Map<IEnumerable<SubCategoria>, IEnumerable<SubCategoriaViewModel>>(subCategorias);

                PaginacaoConfig<SubCategoriaViewModel> pagSet = new PaginacaoConfig<SubCategoriaViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalSubCategorias,
                    TotalPages = (int)Math.Ceiling((decimal)totalSubCategorias / currentPageSize),
                    Items = subCategoriasVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }
        
        
        [HttpGet]
        [Route("consultar/{categoriaId:int=0}")]
        public HttpResponseMessage GetSubCategoriaPorCategoria(HttpRequestMessage request, int? categoriaId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var subCategorias = _subCategoriaRep.GetAll().Where(c => c.CategoriaId == categoriaId);

                var categoriasVM = Mapper.Map<IEnumerable<SubCategoria>, IEnumerable<SubCategoriaViewModel>>(subCategorias);

                response = request.CreateResponse(HttpStatusCode.OK, categoriasVM);

                return response;
            });
        }

        [HttpGet]
        [Route("categoria")]
        public HttpResponseMessage GetCategoria(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var categorias = _categoriaRep.GetAll().Where(c => c.Ativo).ToList();

                IEnumerable<CategoriaViewModel> categoriasVM = Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(categorias);

                response = request.CreateResponse(HttpStatusCode.OK, categoriasVM);

                return response;
            });
        }
        

        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, SubCategoriaViewModel SubCategoriaViewModel)
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

                    SubCategoria novaSubCategoria = new SubCategoria()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        Ativo = SubCategoriaViewModel.Ativo,
                        DescSubCategoria = SubCategoriaViewModel.DescSubCategoria,
                        CategoriaId = SubCategoriaViewModel.CategoriaId

                    };
                    
                    _subCategoriaRep.Add(novaSubCategoria);

                    _unitOfWork.Commit();

                    //Cria Diretório quando se cadastra um nova subcategoria
                    CriarDiretorioImagens(SubCategoriaViewModel, novaSubCategoria);


                    // Update view model
                    SubCategoriaViewModel = Mapper.Map<SubCategoria, SubCategoriaViewModel>(novaSubCategoria);
                    response = request.CreateResponse(HttpStatusCode.Created, SubCategoriaViewModel);


                }

                return response;
            });

        }
        

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, SubCategoriaViewModel subcategoriaViewModel)
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
                    SubCategoria novaSubCategoria = _subCategoriaRep.GetSingle(subcategoriaViewModel.Id);

                    //Cria Diretório quando se cadastra um nova subcategoria
                    CriarDiretorioImagens(subcategoriaViewModel, novaSubCategoria);

                    if (novaSubCategoria.CategoriaId != subcategoriaViewModel.CategoriaId && novaSubCategoria.Produtos.Count > 0)
                    {
                        response = request.CreateResponse(HttpStatusCode.PreconditionFailed, "Não é possível alterar a Categoria desta sub-categoria, pois já existem imagens associadas a ela.");
                    }
                    else
                    {
                        Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                        novaSubCategoria.AtualizarSubCategoria(subcategoriaViewModel, usuario);

                        _unitOfWork.Commit();

                        // Update view model
                        subcategoriaViewModel = Mapper.Map<SubCategoria, SubCategoriaViewModel>(novaSubCategoria);
                        response = request.CreateResponse(HttpStatusCode.OK, subcategoriaViewModel);
                    }
                    
                }

                return response;
            });
        }
        
       
        
        /// <summary>
        /// Método para Criar diretório Imagens do Produto
        /// </summary>
        /// <param name="SubCategoriaViewModel"></param>
        /// <param name="novaSubCategoria"></param>
        private void CriarDiretorioImagens(SubCategoriaViewModel SubCategoriaViewModel, SubCategoria novaSubCategoria)
        {
            var categoria = _categoriaRep.GetSingle(SubCategoriaViewModel.CategoriaId);


            var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");


            //usa uma variável de ambiente para saber se está executando o projeto no computador ou servidor e cria a pasta
            //DirectoryInfo diretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb+"_CamImagens"] +
            //                       Util.RemoverAcentos(categoria.DescCategoria.Replace(" ", "_")) + @"\" + Util.RemoverAcentos(novaSubCategoria.DescSubCategoria.Replace(" ", "_")));

            DirectoryInfo diretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb+"_CamImagens"] +
                                   categoria.Id.ToString() + @"\" + novaSubCategoria.Id.ToString());


           //Chama o método para verificar se diretório existe, caso exista ele não cria a pasta.
            if (VerificaDiretorioExiste(diretorio))
            {
                diretorio.Create();
            }
        }

        
        private bool VerificaDiretorioExiste(DirectoryInfo diretorio)
        {
            if (!diretorio.Exists)
            {
                return true;
            }


            return false;

        }

    }
}
