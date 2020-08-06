using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
using ECC.EntidadePessoa;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeUsuario;
using Ecc.Web.Infrastructure.Core;
using ECC.EntidadeProduto;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;
using System.Data.Entity.SqlServer;

namespace ECC.API_Web.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro, Fornecedor, Franquia")]
    [RoutePrefix("api/produto")]
    public class ProdutoController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Categoria> _categoriaRep;
        private readonly IEntidadeBaseRep<SegmentoCategoria> _segmentoCategoriaRep;
        private readonly IEntidadeBaseRep<SubCategoria> _subcategoriaRep;
        private readonly IEntidadeBaseRep<Produto> _produtosRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<UnidadeMedida> _unidadeMedRep;
        private readonly IEntidadeBaseRep<Fabricante> _fabricanteRep;
        private readonly IEntidadeBaseRep<Marca> _marcaRep;
        private readonly IEntidadeBaseRep<Imagem> _imagemRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<Ranking> _rankingRep;
        private readonly IEntidadeBaseRep<IndisponibilidadeProduto> _disponibilidadeProdutoRep;
        private readonly IEntidadeBaseRep<Franquia> _franquiaRep;
        private readonly IEntidadeBaseRep<FornecedorProduto> _fornecedorProdutoRep;
        private readonly IEntidadeBaseRep<FornecedorProdutoQuantidade> _fornecedorProdutoQuantidadeRep;

        public ProdutoController(IEntidadeBaseRep<Ranking> rankingRep, IEntidadeBaseRep<Marca> marcaRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<MembroCategoria> membroCategoriaRep,
            IEntidadeBaseRep<SubCategoria> subcategoriaRep,
            IEntidadeBaseRep<Categoria> categoriaRep,
            IEntidadeBaseRep<Produto> prod,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<SegmentoCategoria> segmentoCategoriaRep,
            IEntidadeBaseRep<UnidadeMedida> unidadeMedRep,
            IEntidadeBaseRep<Fabricante> fabricanteRep,
            IEntidadeBaseRep<Imagem> imagemRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<IndisponibilidadeProduto> disponibilidadeProdutoRep,
            IEntidadeBaseRep<Franquia> franquiaRep,
            IEntidadeBaseRep<FornecedorProduto> fornecedorProdutoRep,
            IEntidadeBaseRep<FornecedorProdutoQuantidade> fornecedorProdutoQuantidadeRep,
        IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _subcategoriaRep = subcategoriaRep;
            _categoriaRep = categoriaRep;
            _produtosRep = prod;
            _membroCategoriaRep = membroCategoriaRep;
            _membroRep = membroRep;
            _unidadeMedRep = unidadeMedRep;
            _fabricanteRep = fabricanteRep;
            _imagemRep = imagemRep;
            _fornecedorRep = fornecedorRep;
            _marcaRep = marcaRep;
            _rankingRep = rankingRep;
            _segmentoCategoriaRep = segmentoCategoriaRep;
            _disponibilidadeProdutoRep = disponibilidadeProdutoRep;
            _franquiaRep = franquiaRep;
            _fornecedorProdutoRep = fornecedorProdutoRep;
            _fornecedorProdutoQuantidadeRep = fornecedorProdutoQuantidadeRep;
        }


        [HttpGet]
        [Route("pesquisar/{page:int=0}/{pageSize=4}/{categoria:int=0}/{subcategoria:int=0}/{filter?}")]
        public HttpResponseMessage GetProdutos(HttpRequestMessage request, int? page, int? pageSize, int? categoria, int? subcategoria, string filter = null)
        {

            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Produto> produtos = null;
                int totalProdutos = new int();
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId && x.FranquiaId != null);

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();


                    var strinFilter = '%' + filter.Replace(' ', '%') + '%';

                    if (membro != null)
                    {
                        produtos = _produtosRep.FindBy(c =>
                        (SqlFunctions.PatIndex(strinFilter, c.DescProduto.ToLower()) > 0 ||
                        SqlFunctions.PatIndex(strinFilter, c.SubCategoria.Categoria.DescCategoria.ToLower()) > 0 ||
                         SqlFunctions.PatIndex(strinFilter, c.Marca.DescMarca.ToLower()) > 0) &&
                         c.ProdutoPromocionalId == null &&
                         c.Franquias.Any(f => f.FranquiaId == membro.FranquiaId) && c.Ativo)
                        .OrderBy(c => c.DescProduto)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                        totalProdutos = _produtosRep.FindBy
                        (c => (SqlFunctions.PatIndex(strinFilter, c.DescProduto.ToLower()) > 0 ||
                           SqlFunctions.PatIndex(strinFilter, c.SubCategoria.Categoria.DescCategoria.ToLower()) > 0 ||
                          SqlFunctions.PatIndex(strinFilter, c.Marca.DescMarca.ToLower()) > 0) &&
                         c.ProdutoPromocionalId == null &&
                         c.Franquias.Any(f => f.FranquiaId == membro.FranquiaId) && c.Ativo).Count();
                    }
                    else
                    {

                        produtos = _produtosRep.FindBy(c =>
                            (SqlFunctions.PatIndex(strinFilter, c.DescProduto.ToLower()) > 0 ||
                            SqlFunctions.PatIndex(strinFilter, c.SubCategoria.Categoria.DescCategoria.ToLower()) > 0 ||
                            SqlFunctions.PatIndex(strinFilter, c.Marca.DescMarca.ToLower()) > 0)
                              && c.ProdutoPromocionalId == null && c.Ativo)
                            .OrderBy(c => c.DescProduto)
                            .Skip(currentPage * currentPageSize)
                            .Take(currentPageSize)
                            .ToList();

                        totalProdutos = _produtosRep.FindBy
                            (c => (SqlFunctions.PatIndex(strinFilter, c.DescProduto.ToLower()) > 0 ||
                               SqlFunctions.PatIndex(strinFilter, c.SubCategoria.Categoria.DescCategoria.ToLower()) > 0 ||
                              SqlFunctions.PatIndex(strinFilter, c.Marca.DescMarca.ToLower()) > 0) &&
                            c.ProdutoPromocionalId == null && c.Ativo).Count();
                    }

                }
                else
                {

                    if (membro != null)
                    {
                        produtos = _produtosRep.FindBy(c => c.SubCategoriaId == subcategoria &&
                         c.ProdutoPromocionalId == null &&
                         c.Franquias.Any(f => f.FranquiaId == membro.FranquiaId) && c.Ativo)
                        .OrderBy(c => c.DescProduto)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                        totalProdutos = _produtosRep.FindBy(c => c.SubCategoriaId == subcategoria &&
                        c.ProdutoPromocionalId == null &&
                        c.Franquias.Any(f => f.FranquiaId == membro.FranquiaId) && c.Ativo).Count();
                    }
                    else
                    {
                        produtos = _produtosRep.FindBy(c => c.SubCategoriaId == subcategoria &&
                       c.ProdutoPromocionalId == null && c.Ativo)
                       .OrderBy(c => c.DescProduto)
                       .Skip(currentPage * currentPageSize)
                       .Take(currentPageSize)
                       .ToList();

                        totalProdutos = _produtosRep.FindBy(
                        c => c.SubCategoriaId == subcategoria &&
                            c.ProdutoPromocionalId == null && c.Ativo).Count();
                    }
                }


                IEnumerable<ProdutoViewModel> produtosVM =
                    Mapper.Map<IEnumerable<Produto>, IEnumerable<ProdutoViewModel>>(produtos);


                PaginacaoConfig<ProdutoViewModel> pagSet = new PaginacaoConfig<ProdutoViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalProdutos,
                    TotalPages = (int)Math.Ceiling((decimal)totalProdutos / currentPageSize),
                    Items = produtosVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("categoriaporsegmento")]
        public HttpResponseMessage categoriaporsegmento(HttpRequestMessage request, int segmentoId)
        {

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var categoriaSegmentos = _segmentoCategoriaRep.GetAll().Where(x => x.SegmentoId == segmentoId).ToList();

                IEnumerable<CategoriaViewModel> categoriasVM = Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(categoriaSegmentos.Select(x => x.Categoria));

                response = request.CreateResponse(HttpStatusCode.OK, categoriasVM);

                return response;
            });

        }


        [AllowAnonymous]
        [HttpGet]
        [Route("categoria")]
        public HttpResponseMessage GetCategoria(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var categorias = _categoriaRep.GetAll().ToList();

                IEnumerable<CategoriaViewModel> categoriasVM =
                    Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(categorias);

                response = request.CreateResponse(HttpStatusCode.OK, categoriasVM);

                return response;
            });
        }


        [HttpGet]
        [Route("subcategorias/{filter?}")]
        public HttpResponseMessage GetSubCategoriaProduto(HttpRequestMessage request, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                List<SubCategoria> subCategoriaProduto = null;

                subCategoriaProduto = _subcategoriaRep.GetAll().Where(m => m.CategoriaId.ToString() == filter).ToList();

                IEnumerable<SubCategoriaViewModel> subCategoriaVM =
                    Mapper.Map<IEnumerable<SubCategoria>, IEnumerable<SubCategoriaViewModel>>(subCategoriaProduto);

                response = request.CreateResponse(HttpStatusCode.OK, subCategoriaVM);

                return response;
            });
        }


        [HttpGet]
        [Route("unidademedida")]
        public HttpResponseMessage GetUnidadeMedida(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var unidadeMedidaProduto = _unidadeMedRep.GetAll().Where(m => m.Ativo == true);

                IEnumerable<UnidadeMedidaViewModel> unidadeMedidaVM =
                    Mapper.Map<IEnumerable<UnidadeMedida>, IEnumerable<UnidadeMedidaViewModel>>(unidadeMedidaProduto);

                response = request.CreateResponse(HttpStatusCode.OK, unidadeMedidaVM);

                return response;
            });
        }



        [HttpGet]
        [Route("fabricante")]
        public HttpResponseMessage GetFabricante(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var fabricanteProduto = _fabricanteRep.GetAll().Where(m => m.Ativo == true);

                IEnumerable<FabricanteViewModel> fabricanteVM =
                    Mapper.Map<IEnumerable<Fabricante>, IEnumerable<FabricanteViewModel>>(fabricanteProduto);

                response = request.CreateResponse(HttpStatusCode.OK, fabricanteVM);

                return response;
            });
        }


        [HttpGet]
        [Route("marca/{filter?}")]
        public HttpResponseMessage GetMarca(HttpRequestMessage request, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Marca> marca = null;


                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    marca =
                        _marcaRep.FindBy(c => c.DescMarca.ToLower().Contains(filter)).OrderBy(c => c.DescMarca).ToList();

                }

                IEnumerable<MarcaViewModel> marcaVM = Mapper.Map<IEnumerable<Marca>, IEnumerable<MarcaViewModel>>(marca);


                response = request.CreateResponse(HttpStatusCode.OK, marcaVM);

                return response;
            });
        }


        [HttpGet]
        [Route("produto/{filter?}")]
        public HttpResponseMessage GetProduto(HttpRequestMessage request, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Produto> produto = null;
                var produtoVM = new List<object>();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    var strinFilter = '%' + filter.Replace(' ', '%') + '%';

                    produto = _produtosRep.FindBy(c => (SqlFunctions.PatIndex(strinFilter, c.DescProduto.ToLower()) > 0)).OrderBy(c => c.DescProduto).Take(15).ToList();
                }

                produto.ForEach(x =>
                {

                    produtoVM.Add(new Produto { DescProduto = x.DescProduto });

                });

                response = request.CreateResponse(HttpStatusCode.OK, produtoVM);

                return response;
            });
        }


        [HttpGet]
        [Route("categorias/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetCategoriaPaginacao(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Categoria> categorias = null;
                int totalCategorias = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    categorias = _categoriaRep.FindBy(c => c.DescCategoria.ToLower().Contains(filter) ||
                            c.Ativo.ToString() == filter ||
                            c.Id.ToString() == filter)
                           .OrderBy(c => c.DescCategoria)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalCategorias = _produtosRep
                        .GetAll()
                        .Count(c => c.DescProduto.ToLower().Contains(filter) ||
                            c.SubCategoria.Categoria.DescCategoria.ToLower().Contains(filter));
                }
                else
                {
                    categorias = _categoriaRep.GetAll()
                        .OrderBy(c => c.DescCategoria)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalCategorias = _categoriaRep.GetAll().Count();
                }

                IEnumerable<CategoriaViewModel> categoriasVM = Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(categorias);

                PaginacaoConfig<CategoriaViewModel> pagSet = new PaginacaoConfig<CategoriaViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalCategorias,
                    TotalPages = (int)Math.Ceiling((decimal)totalCategorias / currentPageSize),
                    Items = categoriasVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }



        [HttpGet]
        [Route("membroCategoriasMenu")]
        public HttpResponseMessage GetMembroCategoriaMenu(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {


                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.FindBy(x => x.PessoaId == usuario.PessoaId).FirstOrDefault();
                //var membroCategorias = new List<MembroCategoria>();

                if (membro.FranquiaId != null)
                {

                    var produtos = membro.Franquia.Produtos.Where(p => p.Produto.ProdutoPromocionalId == null)
                    .Select(x => x.Produto.SubCategoria.CategoriaId)
                    .Distinct()
                    .ToList();

                    var membroCategorias = _membroCategoriaRep.FindBy(x => x.MembroId == membro.Id && produtos.Contains(x.CategoriaId)).Select(m => new { Id = m.CategoriaId, m.Categoria.DescCategoria, PrimeiraLetra = m.Categoria.DescCategoria.Substring(0, 1), MenuSubCategorias = m.Categoria.SubCategorias.Where(x => x.Produtos.Count > 0).Select(s => new { s.Id, s.DescSubCategoria }).ToList() }).ToList();
                
                    var response = request.CreateResponse(HttpStatusCode.OK, membroCategorias);

                    return response;
                }
                else
                {


                   var membroCategorias = _membroCategoriaRep.FindBy(x => x.MembroId == membro.Id).Select(m => new {Id= m.CategoriaId, m.Categoria.DescCategoria, PrimeiraLetra= m.Categoria.DescCategoria.Substring(0,1), MenuSubCategorias = m.Categoria.SubCategorias.Where(x=>x.Produtos.Count > 0).Select(s=> new {s.Id, s.DescSubCategoria }).ToList() }).ToList();
 
                    var response = request.CreateResponse(HttpStatusCode.OK, membroCategorias);

                    return response;
                }
 

            });
        }


   

        [HttpGet]
        [Route("membroCategorias")]
        public HttpResponseMessage GeUsuariotMembroCategoria(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


                var membro = _membroRep.GetAll().FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var membroCategorias = _membroCategoriaRep.GetAll().Where(x => x.Membro.Id == membro.Id).ToList();

                var membroCategoriasVM = Mapper.Map<IEnumerable<MembroCategoria>, IEnumerable<CategoriaSubCatMenuViewModel>>(membroCategorias);

                var response = request.CreateResponse(HttpStatusCode.OK, membroCategoriasVM);

                return response;
            });
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, ProdutoViewModel ProdutoViewModel)
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

                    var descProduto =
                       _produtosRep.GetAll().Where(p => p.DescProduto.ToLower() == ProdutoViewModel.DescProduto.ToLower()).Select(d => d.DescProduto).FirstOrDefault();

                    if (string.IsNullOrEmpty(descProduto))
                    {
                        Produto novoproduto = new Produto()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            SubCategoriaId = ProdutoViewModel.SubCategoriaId,
                            UnidadeMedidaId = ProdutoViewModel.UnidadeMedidaId,
                            FabricanteId = null,
                            MarcaId = ProdutoViewModel.MarcaId,
                            DescProduto = ProdutoViewModel.DescProduto,
                            Especificacao = ProdutoViewModel.Especificacao,
                            PrecoMedio = ProdutoViewModel.PrecoMedio.TryParseDecimal(),
                            CodigoCNP = ProdutoViewModel.CodigoCNP,
                            Ativo = ProdutoViewModel.Ativo
                        };

                        _produtosRep.Add(novoproduto);

                        novoproduto.Rankings.Add(new Ranking
                        {
                            Nota = 0,
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Ativo = true
                        });

                        _unitOfWork.Commit();

                        // Update view model
                        ProdutoViewModel vmProduto = Mapper.Map<Produto, ProdutoViewModel>(novoproduto);
                        vmProduto.CategoriaId = ProdutoViewModel.CategoriaId;

                        response = request.CreateResponse(HttpStatusCode.Created, vmProduto);

                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Nome do produto já existe, utilize outro nome");
                    }

                }

                return response;
            });

        }


        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, ProdutoViewModel ProdutoViewModel)
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

                    var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");

                    string strArqAnt = String.Empty;

                    bool booSubCategoriaAlterada = false;

                    Produto novoProduto = _produtosRep.GetSingle(ProdutoViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    DirectoryInfo diretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb + "_CamImagens"] +
                                         novoProduto.SubCategoria.Categoria.Id.ToString() + @"\" + novoProduto.SubCategoria.Id.ToString());

                    if (novoProduto.SubCategoria.Id != ProdutoViewModel.SubCategoriaId)
                        booSubCategoriaAlterada = true;

                    if (novoProduto.Imagens.Count > 0)
                    {
                        strArqAnt = diretorio + @"\" + novoProduto.Imagens.FirstOrDefault().CaminhoImagem;
                    }

                    novoProduto.AtualizarProduto(ProdutoViewModel, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    ProdutoViewModel = Mapper.Map<Produto, ProdutoViewModel>(novoProduto);

                    if (booSubCategoriaAlterada)
                    {
                        DirectoryInfo NovoDiretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb + "_CamImagens"] +
                                         novoProduto.SubCategoria.Categoria.Id.ToString() + @"\" + novoProduto.SubCategoria.Id.ToString());

                        string strArqNovo = NovoDiretorio + @"\" + novoProduto.Imagens.FirstOrDefault().CaminhoImagem;

                        File.Move(strArqAnt, strArqNovo);
                    }

                    //var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");
                    //ProdutoViewModel.Imagem.CaminhoImagem = ConfigurationManager.AppSettings[amb + "_CamImagens"] +
                    //                                        novoProduto.SubCategoria.Categoria.DescCategoria + @"\" +
                    //                                        RemoverAcentos(
                    //                                            novoProduto.SubCategoria.DescSubCategoria.Replace(" ",
                    //                                                "_")) + ProdutoViewModel.Imagem.CaminhoImagem;


                    response = request.CreateResponse(HttpStatusCode.OK, ProdutoViewModel);

                }

                return response;
            });
        }


        [HttpGet]
        [Route("imagens/{filter?}")]
        public HttpResponseMessage GetImagem(HttpRequestMessage request, string filter = null)
        {
            HttpResponseMessage response = null;
            Imagem imagemProduto = null;

            return CreateHttpResponse(request, () =>
            {

                if (!string.IsNullOrEmpty(filter))
                {
                    var produto = _produtosRep.GetSingle(filter.TryParseInt());
                    filter = filter.Trim().ToLower();

                    imagemProduto = _imagemRep.GetAll().FirstOrDefault(c => c.ProdutoId.ToString() == filter);

                    if (imagemProduto != null)
                    {



                        var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");

                        //var amb = "Dev";

                        // Update view model
                        var imagemVM = Mapper.Map<Imagem, ImagemViewModel>(imagemProduto);

                        /*
                        imagemVM.CaminhoImagem = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                 Util.RemoverAcentos(produto.SubCategoria.Categoria.DescCategoria.Replace(
                                                     " ", "_")) + @"/" +
                                                 Util.RemoverAcentos(
                                                     produto.SubCategoria.DescSubCategoria.Replace(" ",
                                                         "_")) + @"/" + imagemVM.CaminhoImagem;
                        */



                        imagemVM.CaminhoImagem = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                                         produto.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                                         produto.SubCategoria.Id.ToString() + @"/" + imagemVM.CaminhoImagem;



                        imagemVM.CaminhoImagemGrande = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                                         produto.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                                         produto.SubCategoria.Id.ToString() + @"/" + imagemVM.CaminhoImagemGrande;



                        response = request.CreateResponse(HttpStatusCode.OK, imagemVM);
                    }
                    else
                    {

                        ImagemViewModel imagVM = new ImagemViewModel();
                        {

                            imagVM.CaminhoImagem = "../../../Content/images/unknown.jpg";
                            imagVM.CaminhoImagemGrande = "../../../Content/images/unknown.jpg";

                        }



                        response = request.CreateResponse(HttpStatusCode.OK, imagVM);
                    }

                }


                return response;
            });
        }


        [MimeMultipart]
        [Route("images/upload")]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request, int produtoId, bool imageGr)
        {



            HttpResponseMessage response = null;

            var produto = _produtosRep.GetSingle(produtoId);


            string strArqAnt = string.Empty;
            string strArqAntGrande = string.Empty;
            int imgId = 0;
            string imageGrande = "";



            var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");

            //var amb = "Dev";

            //DirectoryInfo diretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb + "_CamImagens"] +
            //                     Util.RemoverAcentos(produto.SubCategoria.Categoria.DescCategoria.Replace(" ", "_")) + @"\" + Util.RemoverAcentos(produto.SubCategoria.DescSubCategoria.Replace(" ", "_")));


            DirectoryInfo diretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb + "_CamImagens"] +
                                 produto.SubCategoria.Categoria.Id.ToString() + @"\" + produto.SubCategoria.Id.ToString());



            if (produto.Imagens.Count > 0)
            {
                strArqAnt = diretorio + @"\" + produto.Imagens.FirstOrDefault().CaminhoImagem;

                strArqAntGrande = diretorio + @"\" + produto.Imagens.FirstOrDefault().CaminhoImagemGrande;

                imgId = produto.Imagens.FirstOrDefault().Id;

            }


            Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

            //Só entra se no controller eu passar a variável 'imageGr' como true
            #region Cadastro da Imagem Grande


            if (imageGr)
            {

                imageGrande = "_G";

                var multipartFormDataStreamProviderGrande = new UploadMultipartFormProvider(diretorio.ToString(), produtoId.ToString(), imageGrande);


                // Read the MIME multipart asynchronously 
                await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProviderGrande);


                string _localFileNameGrande = multipartFormDataStreamProviderGrande.FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();



                Imagem imagem = _imagemRep.FindBy(i => i.ProdutoId == produtoId).FirstOrDefault();


                ImagemViewModel novaImagemGrande = new ImagemViewModel
                {
                    IdProduto = produtoId,
                    ImagemId = imagem.Id,
                    CaminhoImagemGrande = Path.GetFileName(_localFileNameGrande),
                    CaminhoImagem = imagem.CaminhoImagem
                };

                if (diretorio.Exists)
                {


                    string imagemG = imagem.CaminhoImagemGrande;


                    if (!string.IsNullOrEmpty(imagemG))
                    {
                        File.Delete(strArqAntGrande);
                    }


                    imagem.AtualizarImagem(novaImagemGrande, usuario);

                    _unitOfWork.Commit();


                    // Update view model
                    var imagemVM = Mapper.Map<Imagem, ImagemViewModel>(imagem);


                    imagemVM.CaminhoImagemGrande = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                   produto.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                   produto.SubCategoria.Id.ToString() + @"/" + imagemVM.CaminhoImagemGrande;


                    imagemVM.CaminhoImagem = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                   produto.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                   produto.SubCategoria.Id.ToString() + @"/" + imagemVM.CaminhoImagem;


                    response = request.CreateResponse(HttpStatusCode.OK, imagemVM);

                }


            }

            #endregion



            //Só entra se no controller eu passar a variável 'imageGr' como false
            #region Cadastro da Imagem Pequena
            if (imageGr == false)
            {


                var multipartFormDataStreamProvider = new UploadMultipartFormProvider(diretorio.ToString(), produtoId.ToString(), imageGrande);


                // Read the MIME multipart asynchronously 
                await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);


                string _localFileName = multipartFormDataStreamProvider.FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();

                string imgGrande =
                    _imagemRep.FindBy(i => i.Id == imgId).Select(o => o.CaminhoImagemGrande).FirstOrDefault();

                Imagem novaImagem = new Imagem
                {
                    UsuarioCriacao = usuario,
                    DtCriacao = DateTime.Now,
                    Produto = produto,
                    CaminhoImagem = Path.GetFileName(_localFileName),
                    CaminhoImagemGrande = imgGrande,
                    Ativo = produto.Ativo
                };


                //verifica se diretório existe, caso exista ele não cria a pasta.
                if (diretorio.Exists)
                {

                    _imagemRep.Add(novaImagem);

                    _unitOfWork.Commit();

                    // Update view model
                    var imagemVM = Mapper.Map<Imagem, ImagemViewModel>(novaImagem);


                    //imagemVM.CaminhoImagem = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                    //                                Util.RemoverAcentos(produto.SubCategoria.Categoria.DescCategoria.Replace(" ", "_")) + @"/" +
                    //                               Util.RemoverAcentos(
                    //                                   produto.SubCategoria.DescSubCategoria.Replace(" ",
                    //                                      "_")) + @"/" + imagemVM.CaminhoImagem;


                    imagemVM.CaminhoImagem = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                   produto.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                   produto.SubCategoria.Id.ToString() + @"/" + imagemVM.CaminhoImagem;


                    imagemVM.CaminhoImagemGrande = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                produto.SubCategoria.Categoria.Id.ToString() + @"/" +
                                                produto.SubCategoria.Id.ToString() + @"/" + imagemVM.CaminhoImagemGrande;


                    response = request.CreateResponse(HttpStatusCode.OK, imagemVM);

                }


                //Este código serve para delete a imagem anterior depois q ele insere a nova caso ja exista uma
                if (imgId > 0)
                {
                    File.Delete(strArqAnt);
                    var imgant = _imagemRep.GetSingle(imgId);
                    _imagemRep.Delete(imgant);
                    _unitOfWork.Commit();
                }

            }

            #endregion

            return response;
        }


        [HttpPost]
        [Route("inserirNota")]
        public HttpResponseMessage InserirNota(HttpRequestMessage request, ProdutoViewModel ProdutoViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (ProdutoViewModel.Ranking > 0)
                {

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    Ranking novoRanking = new Ranking()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        ProdutoId = ProdutoViewModel.Id,
                        Nota = ProdutoViewModel.Ranking,
                        Ativo = ProdutoViewModel.Ativo

                    };

                    _rankingRep.Add(novoRanking);
                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });

        }

        [HttpGet]
        [Route("indisponibilidadeProduto")]
        public HttpResponseMessage GetDisponibilidadeProduto(HttpRequestMessage request)
        {

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var disponibilidadeProduto = _disponibilidadeProdutoRep.GetAll().Where(x => x.FornecedorId == fornecedor.Id).ToList();

                IEnumerable<IndisponibilidadeProdutoViewModel> disponibilidadeProdutoVM = Mapper.Map<IEnumerable<IndisponibilidadeProduto>, IEnumerable<IndisponibilidadeProdutoViewModel>>(disponibilidadeProduto);

                response = request.CreateResponse(HttpStatusCode.OK, disponibilidadeProdutoVM);

                return response;
            });

        }

        [HttpPost]
        [Route("inserirDeletarIndisponibilidadeProduto/{selecionado:bool}")]
        public HttpResponseMessage InserirDeletarDisponibilidadeProduto(HttpRequestMessage request, bool selecionado, IndisponibilidadeProdutoViewModel disponibilidadeProdutoViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                IndisponibilidadeProduto disponibilidadeProduto = new IndisponibilidadeProduto();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);


                if (selecionado)
                {

                    if (disponibilidadeProdutoViewModel.FimIndisponibilidade.Ticks > 0)
                    {

                        disponibilidadeProduto.UsuarioCriacao = usuario;
                        disponibilidadeProduto.DtCriacao = DateTime.Now;
                        disponibilidadeProduto.ProdutoId = disponibilidadeProdutoViewModel.ProdutoId;
                        disponibilidadeProduto.FornecedorId = fornecedor.Id;
                        disponibilidadeProduto.InicioIndisponibilidade = disponibilidadeProdutoViewModel.InicioIndisponibilidade;
                        disponibilidadeProduto.FimIndisponibilidade = disponibilidadeProdutoViewModel.FimIndisponibilidade;
                        disponibilidadeProduto.IndisponivelPermanente = disponibilidadeProdutoViewModel.IndisponivelPermanente;
                        disponibilidadeProduto.Ativo = true;

                        _disponibilidadeProdutoRep.Add(disponibilidadeProduto);
                        _unitOfWork.Commit();
                    }
                    else
                    {
                        disponibilidadeProduto.UsuarioCriacao = usuario;
                        disponibilidadeProduto.DtCriacao = DateTime.Now;
                        disponibilidadeProduto.ProdutoId = disponibilidadeProdutoViewModel.ProdutoId;
                        disponibilidadeProduto.FornecedorId = fornecedor.Id;
                        disponibilidadeProduto.IndisponivelPermanente = disponibilidadeProdutoViewModel.IndisponivelPermanente;
                        disponibilidadeProduto.Ativo = true;

                        _disponibilidadeProdutoRep.Add(disponibilidadeProduto);
                        _unitOfWork.Commit();
                    }

                    //var disponibilidadeProdutoVM = Mapper.Map<IndisponibilidadeProduto, IndisponibilidadeProdutoViewModel>(disponibilidadeProduto);


                    response = request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
                    var produtoDisponivel = _disponibilidadeProdutoRep.FindBy(x => x.ProdutoId == disponibilidadeProdutoViewModel.ProdutoId &&
                                                                                  x.FornecedorId == fornecedor.Id).FirstOrDefault();

                    if (produtoDisponivel != null)
                    {
                        _disponibilidadeProdutoRep.Delete(produtoDisponivel);
                        _unitOfWork.Commit();

                        response = request.CreateResponse(HttpStatusCode.OK);
                    }

                }

                return response;
            });

        }

        [HttpGet]
        [Route("deletarIndisponibilidadeForaValidade")]
        public HttpResponseMessage DeletarIndisponibilidadeForaValidade(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                Fornecedor fornecedor = _fornecedorRep.FindBy(x => x.PessoaId == usuario.PessoaId).FirstOrDefault();

                _disponibilidadeProdutoRep.ExecuteWithStoreProcedure("stp_upd_indisponibilidade_produto @FornecedorId",
                                                     new SqlParameter("@FornecedorId", fornecedor.Id)
                                                    );

                response = request.CreateResponse(HttpStatusCode.OK);

                return response;
            });
        }

        private static List<Produto> Produtos(List<Produto> produtos, Membro membro)
        {
            var produtosFranquia = produtos
                .Where(x => x.Franquias.Any(f => f.FranquiaId == membro.FranquiaId))
                .ToList();

            produtos = produtosFranquia;

            return produtos;
        }

        #region FornecedorProduto

        [HttpGet]
        [Route("getfornecedorproduto")]
        public HttpResponseMessage GetFornecedorProduto(HttpRequestMessage request)
        {

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var fornecedorProduto = _fornecedorProdutoRep.GetAll().Where(x => x.FornecedorId == fornecedor.Id);

                IEnumerable<FornecedorProdutoViewModel> fpVM = Mapper.Map<IEnumerable<FornecedorProduto>, IEnumerable<FornecedorProdutoViewModel>>(fornecedorProduto);

                response = request.CreateResponse(HttpStatusCode.OK, fpVM);

                return response;
            });

        }


        [HttpGet]
        [Route("getfornecedorprodutoqtd/{produtoId:int}")]
        public HttpResponseMessage GetFornecedorProdutoQtd(HttpRequestMessage request, int produtoId)
        {

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var fornecedorProduto = _fornecedorProdutoRep.GetAll().FirstOrDefault(x => x.FornecedorId == fornecedor.Id &&
                                                    x.ProdutoId == produtoId);
                var fpQuantidade = _fornecedorProdutoQuantidadeRep.GetAll().Where(w => w.FornecedorProdutoId == fornecedorProduto.Id).ToList();

                IEnumerable<FornecedorProdutoQuantidadeViewModel> fpQuantidadeVM = Mapper.Map<IEnumerable<FornecedorProdutoQuantidade>, IEnumerable<FornecedorProdutoQuantidadeViewModel>>(fpQuantidade);

                response = request.CreateResponse(HttpStatusCode.OK, fpQuantidadeVM);

                return response;
            });

        }


        [HttpPost]
        [Route("salvarProdutoQtd")]
        public HttpResponseMessage SalvarProdutoQtd(HttpRequestMessage request, FornecedorProdutoSalvaPostViewModel fornProdVM)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                decimal percMin = 0;
                decimal percMax = 0;

                //verifica se existe ProdutoFornecedor Cadastrado
                FornecedorProduto fornecedorProduto = new FornecedorProduto();
                if (_fornecedorProdutoRep.GetAll().Where(w => w.FornecedorId == fornecedor.Id
                                                           && w.ProdutoId == fornProdVM.ProdutoId).Any())
                {
                    //Atualiza
                    fornecedorProduto = _fornecedorProdutoRep.GetAll().FirstOrDefault(w => w.FornecedorId == fornecedor.Id
                                                                                     && w.ProdutoId == fornProdVM.ProdutoId);

                    fornecedorProduto.Valor = fornProdVM.Valor.TryParseDecimal();
                    fornecedorProduto.CodigoProdutoFornecedor = fornProdVM.CodigoProdutoFornecedor;
                    fornecedorProduto.UsuarioAlteracaoId = usuario.Id;
                    fornecedorProduto.DtAlteracao = DateTime.Now;


                    _fornecedorProdutoRep.Edit(fornecedorProduto);
                    _unitOfWork.Commit();
                }
                else
                {
                    //Insere novo
                    fornecedorProduto.FornecedorId = fornecedor.Id;
                    fornecedorProduto.ProdutoId = fornProdVM.ProdutoId;
                    fornecedorProduto.Valor = fornProdVM.Valor.TryParseDecimal();
                    fornecedorProduto.CodigoProdutoFornecedor = fornProdVM.CodigoProdutoFornecedor;
                    fornecedorProduto.DtCriacao = DateTime.Now;
                    fornecedorProduto.UsuarioCriacaoId = usuario.Id;
                    fornecedorProduto.Ativo = true;
                    _fornecedorProdutoRep.Add(fornecedorProduto);
                    _unitOfWork.Commit();
                }


                // Deleta os Percentuais e Quantidades
                var fpQtd = _fornecedorProdutoQuantidadeRep.GetAll().Where(w => w.FornecedorProdutoId == fornecedorProduto.Id);
                _fornecedorProdutoQuantidadeRep.DeleteAll(fpQtd);

                // Insere os novos Percentuais e Quantidades
                FornecedorProdutoQuantidade novoFpQtd = new FornecedorProdutoQuantidade();
                foreach (var item in fornProdVM.ListaQuantidadeDesconto)
                {
                    novoFpQtd = new FornecedorProdutoQuantidade();
                    novoFpQtd.UsuarioCriacaoId = usuario.Id;
                    novoFpQtd.DtCriacao = DateTime.Now;

                    novoFpQtd.FornecedorProdutoId = fornecedorProduto.Id;
                    novoFpQtd.QuantidadeMinima = item.QuantidadeMinima;
                    novoFpQtd.PercentualDesconto = item.PercentualDesconto.TryParseDecimal();
                    novoFpQtd.ValidadeQtdDesconto = item.ValidadeQtdDesconto;



                    _fornecedorProdutoQuantidadeRep.Add(novoFpQtd);
                }
                _unitOfWork.Commit();

                if (fornProdVM.ListaQuantidadeDesconto.Count > 0)
                {
                    percMin = fornProdVM.ListaQuantidadeDesconto.Min(m => m.PercentualDesconto.TryParseDecimal());
                    percMax = fornProdVM.ListaQuantidadeDesconto.Max(m => m.PercentualDesconto.TryParseDecimal());
                }

                var response = request.CreateResponse(HttpStatusCode.OK, new { success = true, PercentMin = percMin, PercentMax = percMax });

                return response;
            });
        }

        #endregion

    }
}
