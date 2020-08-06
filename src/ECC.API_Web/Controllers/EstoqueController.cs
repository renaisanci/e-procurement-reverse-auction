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
using ECC.Entidades;
using ECC.EntidadeUsuario;
using ECC.EntidadeEndereco;
using ECC.EntidadeEstoque;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.Servicos.Abstrato;
using Microsoft.AspNet.Identity;
using ECC.Entidades.EntidadeProduto;

namespace ECC.API_Web.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.Authorize(Roles = "Admin, Fornecedor, Membro")]
    [RoutePrefix("api/estoque")]
    public class EstoqueController : ApiControllerBase
    {

        #region Variaveis
        private readonly IEntidadeBaseRep<Estoque> _estoqueRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IUtilService _ultilService;
        private readonly IEntidadeBaseRep<Endereco> _enderecoRep;
        private readonly IEntidadeBaseRep<Produto> _produtoRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<SubCategoria> _subcategoriaRep;

        #endregion

        public EstoqueController(IEntidadeBaseRep<Estoque> estoqueRep, 
                                 IEntidadeBaseRep<Endereco> enderecoRep, 
                                 IEntidadeBaseRep<Usuario> usuarioRep, 
                                 IEntidadeBaseRep<Produto> produtoRep,
                                 IEntidadeBaseRep<Membro> membroRep,
                                 IEntidadeBaseRep<MembroCategoria> membroCategoriaRep,
                                 IEntidadeBaseRep<SubCategoria> subcategoriaRep,
                                 IUtilService ultilService, 
                                 IEntidadeBaseRep<Erro> _errosRepository, 
                                 IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _estoqueRep = estoqueRep;
            _usuarioRep = usuarioRep;
            _enderecoRep = enderecoRep;
            _produtoRep = produtoRep;
            _membroRep = membroRep;
            _membroCategoriaRep = membroCategoriaRep;
            _subcategoriaRep = subcategoriaRep;
            _ultilService = ultilService;
            
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
                List<Estoque> estoque = null;
                int totalEstoque = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    estoque = _estoqueRep.FindBy(c => c.Endereco.DescEndereco.Contains(filter))
                        .OrderBy(c => c.Produto.DescProduto)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalEstoque = _estoqueRep
                        .GetAll()
                        .Count(c => c.Endereco.DescEndereco.Contains(filter));
                }
                else
                {
                    estoque = _estoqueRep.GetAll()
                        .OrderBy(c => c.Produto.DescProduto)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalEstoque = _estoqueRep.GetAll().Count();
                }

                IEnumerable<EstoqueViewModel> estoqueVM = Mapper.Map<IEnumerable<Estoque>, IEnumerable<EstoqueViewModel>>(estoque);

                PaginacaoConfig<EstoqueViewModel> pagSet = new PaginacaoConfig<EstoqueViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalEstoque,
                    TotalPages = (int)Math.Ceiling((decimal)totalEstoque / currentPageSize),
                    Items = estoqueVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }



        [HttpGet]
        [Route("enderecos")]
        public HttpResponseMessage GetEnderecos(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var enderecos = _enderecoRep.GetAll().Where(x => x.Pessoa.Id == usuario.PessoaId); ;

                IEnumerable<EnderecoViewModel> enderecosVM = Mapper.Map<IEnumerable<Endereco>, IEnumerable<EnderecoViewModel>>(enderecos);

                response = request.CreateResponse(HttpStatusCode.OK, enderecosVM);

                return response;
            });
        }



        [HttpGet]
        [Route("membro")]
        public HttpResponseMessage GetMembro(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                Membro membro = _membroRep.FindBy(m => m.PessoaId == usuario.Pessoa.Id).FirstOrDefault();

                MembroViewModel estoqueVM = Mapper.Map<Membro, MembroViewModel>(membro);
                response = request.CreateResponse(HttpStatusCode.OK, estoqueVM);

                return response;
            });
        }



        [HttpGet]
        [Route("membrocategoria")]
        public HttpResponseMessage GetMembroCategoria(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                Membro membro = _membroRep.FindBy(m => m.PessoaId == usuario.Pessoa.Id).FirstOrDefault();

                var membroCategoria = membro.MembroCategorias;
                IEnumerable<CategoriaViewModel> membroCategoriasVM = Mapper.Map<IEnumerable<MembroCategoria>, IEnumerable<CategoriaViewModel>>(membroCategoria);
                
                response = request.CreateResponse(HttpStatusCode.OK, membroCategoriasVM);

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

                IEnumerable<SubCategoriaViewModel> subCategoriaVM = Mapper.Map<IEnumerable<SubCategoria>, IEnumerable<SubCategoriaViewModel>>(subCategoriaProduto);

                response = request.CreateResponse(HttpStatusCode.OK, subCategoriaVM);

                return response;
            });
        }



        [HttpGet]
        [Route("produtos/{filter?}")]
        public HttpResponseMessage GetProdutos(HttpRequestMessage request, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Produto> produtos = null;

                produtos = _produtoRep.GetAll().Where(m => m.SubCategoriaId.ToString() == filter).ToList();

                IEnumerable<ProdutoViewModel> produtosVM = Mapper.Map<IEnumerable<Produto>, IEnumerable<ProdutoViewModel>>(produtos);

                response = request.CreateResponse(HttpStatusCode.OK, produtosVM);

                return response;
            });
        }



        [HttpGet]
        [Route("produtoSelecionado/{filter?}")]
        public HttpResponseMessage GetProdutoSelecionado(HttpRequestMessage request, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var produto = _produtoRep.GetAll().FirstOrDefault(m => m.Id.ToString() == filter);

                ProdutoViewModel produtosVM = Mapper.Map<Produto, ProdutoViewModel>(produto);

                response = request.CreateResponse(HttpStatusCode.OK, produtosVM);

                return response;
            });
        }



        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, EstoqueViewModel estoqueViewModel)
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


                    Estoque novoItemEstoque = new Estoque()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        ProdutoId = estoqueViewModel.ProdutoId,
                        MembroId = estoqueViewModel.MembroId,
                        EnderecoId = estoqueViewModel.EnderecoId,
                        MinimoEstoque = estoqueViewModel.MinimoEstoque,
                        MaximoEstoque = estoqueViewModel.MaximoEstoque,
                        QtdEstoque = estoqueViewModel.QtdEstoque,
                        QtdEstoqueReceber = estoqueViewModel.QtdEstoqueReceber,
                        Ativo = estoqueViewModel.Ativo
                    };

                    _estoqueRep.Add(novoItemEstoque);

                    _unitOfWork.Commit();

                    // Update view model
                    EstoqueViewModel estoqueVM = Mapper.Map<Estoque, EstoqueViewModel>(novoItemEstoque);

                    response = request.CreateResponse(HttpStatusCode.Created, estoqueVM);
                }

                return response;
            });

        }



        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, EstoqueViewModel estoqueViewModel)
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
                    Estoque novoItemEstoque = _estoqueRep.GetSingle(estoqueViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    novoItemEstoque.AtualizarEstoque(estoqueViewModel, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    estoqueViewModel = Mapper.Map<Estoque, EstoqueViewModel>(novoItemEstoque);

                    response = request.CreateResponse(HttpStatusCode.OK, estoqueViewModel);
                }

                return response;
            });
        }
    }
}