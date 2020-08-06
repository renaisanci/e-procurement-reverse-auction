using System;
using System.Collections.Generic;
using System.Configuration;
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
using ECC.API_Web.Models;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.Entidades;
using ECC.EntidadePessoa;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeUsuario;
using Ecc.Web.Infrastructure.Core;
using Microsoft.AspNet.Identity;
using System.Drawing;
using ECC.API_Web.Hubs;
using ECC.EntidadeFormaPagto;
using ECC.EntidadeProduto;
using ECC.Servicos;
using Microsoft.AspNet.SignalR;
using ECC.EntidadeEmail;
using ECC.EntidadeSms;

namespace ECC.API_Web.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.Authorize(Roles = "Admin, Membro, Fornecedor,Franquia")]
    [RoutePrefix("api/produtopromocional")]
    public class ProdutoPromocionalController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Categoria> _categoriaRep;
        private readonly IEntidadeBaseRep<SubCategoria> _subcategoriaRep;
        private readonly IEntidadeBaseRep<Produto> _produtosRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<UnidadeMedida> _unidadeMedRep;
        private readonly IEntidadeBaseRep<Marca> _marcaRep;
        private readonly IEntidadeBaseRep<Imagem> _imagemRep;
        private readonly IEntidadeBaseRep<FornecedorCategoria> _fornecedorCategoriaRep;
        private readonly IEntidadeBaseRep<ProdutoPromocional> _produtoPromocionalRep;
        private readonly IEntidadeBaseRep<HistoricoPromocao> _historicoPromocaoRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<PromocaoFormaPagto> _promocaoFormaPagtoRep;
        private readonly IEntidadeBaseRep<Franquia> _franquiaRep;
        private readonly IEntidadeBaseRep<FranquiaProduto> _franquiaProdutoRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IEntidadeBaseRep<Sms> _smsRep;


        public ProdutoPromocionalController(
           IEntidadeBaseRep<TemplateEmail> templateEmailRep,
            IEntidadeBaseRep<Emails> emailsRep,
            IEntidadeBaseRep<Sms> smsRep,
            IEntidadeBaseRep<Marca> marcaRep,
            IEntidadeBaseRep<SubCategoria> subcategoriaRep,
            IEntidadeBaseRep<Categoria> categoriaRep,
            IEntidadeBaseRep<Produto> prod,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<UnidadeMedida> unidadeMedRep,
            IEntidadeBaseRep<FornecedorCategoria> fornecedorCategoriaRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<Imagem> imagemRep,
            IEntidadeBaseRep<ProdutoPromocional> produtoPromocionalRep,
            IEntidadeBaseRep<HistoricoPromocao> historicoPromocaoRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<MembroCategoria> membroCategoriaRep,
            IEntidadeBaseRep<PromocaoFormaPagto> promocaoFormaPagtoRep,
            IEntidadeBaseRep<Franquia> franquiaRep,
            IEntidadeBaseRep<FranquiaProduto> franquiaProdutoRep,
            IEntidadeBaseRep<Erro> _errosRepository,
            IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _subcategoriaRep = subcategoriaRep;
            _categoriaRep = categoriaRep;
            _produtosRep = prod;
            _unidadeMedRep = unidadeMedRep;
            _imagemRep = imagemRep;
            _marcaRep = marcaRep;
            _fornecedorCategoriaRep = fornecedorCategoriaRep;
            _fornecedorRep = fornecedorRep;
            _produtoPromocionalRep = produtoPromocionalRep;
            _historicoPromocaoRep = historicoPromocaoRep;
            _membroRep = membroRep;
            _membroCategoriaRep = membroCategoriaRep;
            _promocaoFormaPagtoRep = promocaoFormaPagtoRep;
            _franquiaRep = franquiaRep;
            _franquiaProdutoRep = franquiaProdutoRep;
            _templateEmailRep = templateEmailRep;
            _smsRep = smsRep;
            _emailsRep = emailsRep;
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

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                Fornecedor fornecedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.PessoaId).FirstOrDefault();


                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    produtos = _produtosRep.FindBy(c => c.DescProduto.ToLower().Contains(filter) ||
                                                        c.SubCategoria.Categoria.DescCategoria.ToLower()
                                                            .Contains(filter) &&
                                                            c.SubCategoriaId == subcategoria &&
                                                            c.SubCategoria.CategoriaId == categoria &&
                                                            c.ProdutoPromocional.FornecedorId == fornecedor.Id)
                                                        .OrderBy(c => c.SubCategoria.Categoria.DescCategoria)
                                                        .Skip(currentPage * currentPageSize)
                                                        .Take(currentPageSize)
                                                        .ToList();

                    totalProdutos = _produtosRep
                        .GetAll()
                        .Count(c => c.DescProduto.ToLower().Contains(filter) ||
                                    c.SubCategoria.Categoria.DescCategoria.ToLower().Contains(filter) &&
                                    c.SubCategoriaId == subcategoria &&
                                    c.SubCategoria.CategoriaId == categoria &&
                                    c.ProdutoPromocional.FornecedorId == fornecedor.Id);
                }
                else
                {

                    if (categoria > 0)
                    {

                        produtos = _produtosRep.GetAll()
                            .Where(c => c.SubCategoriaId == subcategoria &&
                                        c.SubCategoria.CategoriaId == categoria &&
                                        c.ProdutoPromocional.FornecedorId == fornecedor.Id)
                            .OrderBy(c => c.DescProduto)
                            .Skip(currentPage * currentPageSize)
                            .Take(currentPageSize)
                            .ToList();

                        totalProdutos = _produtosRep.GetAll().Count(c => c.SubCategoriaId == subcategoria &&
                            c.SubCategoria.CategoriaId == categoria &&
                            c.ProdutoPromocional.FornecedorId == fornecedor.Id);
                    }
                    else
                    {
                        produtos = _produtosRep.GetAll()
                      .Where(c => c.ProdutoPromocional.FornecedorId == fornecedor.Id)
                      .OrderBy(c => c.DescProduto)
                      .Skip(currentPage * currentPageSize)
                      .Take(currentPageSize)
                      .ToList();

                        totalProdutos = _produtosRep.GetAll().Count(c => c.ProdutoPromocional.FornecedorId == fornecedor.Id);

                    }


                }


                var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");

                IEnumerable<ProdutoPromocionalViewModel> produtosVM =
                    Mapper.Map<IEnumerable<Produto>, IEnumerable<ProdutoPromocionalViewModel>>(produtos);


                PaginacaoConfig<ProdutoPromocionalViewModel> pagSet = new PaginacaoConfig<ProdutoPromocionalViewModel>()
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


        [HttpGet]
        [Route("pesquisaradm/{page:int=0}/{pageSize=4}/{categoria:int=0}/{subcategoria:int=0}/{fornecedor:int=0}/{filter?}")]
        public HttpResponseMessage GetProdutosAdm(HttpRequestMessage request, int? page, int? pageSize, int? categoria, int? subcategoria, int? fornecedor, string filter = null)
        {

            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Produto> produtos = null;
                int totalProdutos = new int();


                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    produtos = _produtosRep.FindBy(c => c.DescProduto.ToLower().Contains(filter) ||
                                                            c.SubCategoria.Categoria.DescCategoria.ToLower()
                                                            .Contains(filter) &&
                                                            c.SubCategoriaId == subcategoria &&
                                                            c.SubCategoria.CategoriaId == categoria &&
                                                            c.ProdutoPromocional.Ativo &&
                                                            c.ProdutoPromocional.Aprovado ||
                                                            !c.ProdutoPromocional.Aprovado)
                                                        .OrderBy(c => c.SubCategoria.Categoria.DescCategoria)
                                                        .Skip(currentPage * currentPageSize)
                                                        .Take(currentPageSize)
                                                        .ToList();

                    totalProdutos = _produtosRep
                        .GetAll()
                        .Count(c => c.DescProduto.ToLower().Contains(filter) ||
                                    c.SubCategoria.Categoria.DescCategoria.ToLower().Contains(filter) &&
                                    c.SubCategoriaId == subcategoria &&
                                    c.SubCategoria.CategoriaId == categoria &&
                                    c.ProdutoPromocional.Ativo &&
                                    c.ProdutoPromocional.Aprovado ||
                                    !c.ProdutoPromocional.Aprovado);
                }
                else
                {

                    if (subcategoria > 0)
                    {
                        produtos = _produtosRep.GetAll()
                       .Where(c => c.SubCategoriaId == subcategoria &&
                                    c.SubCategoria.CategoriaId == categoria &&
                                    c.ProdutoPromocional.Ativo &&
                                    c.ProdutoPromocional.Aprovado ||
                                    !c.ProdutoPromocional.Aprovado)
                       .OrderBy(c => c.DescProduto)
                       .Skip(currentPage * currentPageSize)
                       .Take(currentPageSize)
                       .ToList();

                        totalProdutos = _produtosRep.GetAll().Count(c => c.SubCategoriaId == subcategoria &&
                             c.SubCategoria.CategoriaId == categoria &&
                             c.ProdutoPromocional.Ativo &&
                             c.ProdutoPromocional.Aprovado ||
                             !c.ProdutoPromocional.Aprovado);
                    }
                    //else if (categoria > 0)
                    //{
                    //    produtos = _produtosRep.GetAll()
                    //   .Where(c => c.SubCategoria.CategoriaId == categoria &&
                    //                c.ProdutoPromocional.Ativo &&
                    //                c.ProdutoPromocional.Aprovado ||
                    //                !c.ProdutoPromocional.Aprovado)
                    //   .OrderBy(c => c.DescProduto)
                    //   .Skip(currentPage * currentPageSize)
                    //   .Take(currentPageSize)
                    //   .ToList();

                    //    totalProdutos = _produtosRep.GetAll().Count(c => 
                    //         c.SubCategoria.CategoriaId == categoria &&
                    //         c.ProdutoPromocional.Ativo &&
                    //         c.ProdutoPromocional.Aprovado ||
                    //         !c.ProdutoPromocional.Aprovado);
                    //}
                    else if (fornecedor > 0)
                    {
                        produtos = _produtosRep.GetAll()
                       .Where(c => c.ProdutoPromocional.FornecedorId == fornecedor &&
                                    c.ProdutoPromocional.Ativo &&
                                    c.ProdutoPromocional.Aprovado ||
                                    !c.ProdutoPromocional.Aprovado)
                       .OrderBy(c => c.DescProduto)
                       .Skip(currentPage * currentPageSize)
                       .Take(currentPageSize)
                       .ToList();

                        totalProdutos = _produtosRep.GetAll().Count(c => c.ProdutoPromocional.FornecedorId == fornecedor &&
                             c.ProdutoPromocional.Ativo &&
                             c.ProdutoPromocional.Aprovado ||
                             !c.ProdutoPromocional.Aprovado);
                    }
                    else
                    {
                        produtos = _produtosRep.GetAll()
                    .Where(c => c.ProdutoPromocional.Ativo &&
                                 c.ProdutoPromocional.Aprovado ||
                                !c.ProdutoPromocional.Aprovado)
                    .OrderBy(c => c.DescProduto)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                        totalProdutos = _produtosRep.GetAll().Count(c => c.ProdutoPromocional.Ativo &&
                             c.ProdutoPromocional.Aprovado ||
                            !c.ProdutoPromocional.Aprovado);

                    }
                }


                var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");

                IEnumerable<ProdutoPromocionalViewModel> produtosVM =
                    Mapper.Map<IEnumerable<Produto>, IEnumerable<ProdutoPromocionalViewModel>>(produtos);


                PaginacaoConfig<ProdutoPromocionalViewModel> pagSet = new PaginacaoConfig<ProdutoPromocionalViewModel>()
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

        [HttpGet]
        [Route("pesquisarprodfranquia/{page:int=0}/{pageSize=4}/{categoria:int=0}/{subcategoria:int=0}/{filter?}")]
        public HttpResponseMessage GetProdutosFranquia(HttpRequestMessage request, int? page, int? pageSize, int? categoria, int? subcategoria, string filter = null)
        {

            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Produto> produtos = null;
                int totalProdutos = new int();
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var idFornecedores = franquia.Fornecedores.Select(f => f.FornecedorId).ToList();

                var produtosFranquia = franquia.Produtos.
                                        Where(c => c.Produto.ProdutoPromocionalId != null &&
                                        c.Produto.ProdutoPromocional.FimPromocao > DateTime.Now &&
                                        c.Produto.ProdutoPromocional.Ativo &&
                                        c.Produto.ProdutoPromocional.Aprovado)
                                        .Select(x => x.ProdutoId).ToList();

                DeletaProdutoFranquia(franquia);

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    produtos = _produtosRep.FindBy(c => c.DescProduto.ToLower().Contains(filter) ||
                                                            c.SubCategoria.Categoria.DescCategoria.ToLower()
                                                            .Contains(filter) &&
                                                            c.SubCategoriaId == subcategoria &&
                                                            c.SubCategoria.CategoriaId == categoria &&
                                                            c.ProdutoPromocional.Ativo &&
                                                            c.ProdutoPromocional.Aprovado &&
                                                            idFornecedores.Contains(c.ProdutoPromocional.FornecedorId) &&
                                                            c.ProdutoPromocional.FimPromocao > DateTime.Now &&
                                                            c.ProdutoPromocionalId != null)
                                                        .OrderBy(c => c.SubCategoria.Categoria.DescCategoria)
                                                        .Skip(currentPage * currentPageSize)
                                                        .Take(currentPageSize)
                                                        .ToList();

                    totalProdutos = _produtosRep.GetAll()
                        .Count(c => c.DescProduto.ToLower().Contains(filter) ||
                                    c.SubCategoria.Categoria.DescCategoria.ToLower().Contains(filter) &&
                                    c.SubCategoriaId == subcategoria &&
                                    c.SubCategoria.CategoriaId == categoria &&
                                    c.ProdutoPromocional.Ativo &&
                                    c.ProdutoPromocional.Aprovado &&
                                    idFornecedores.Contains(c.ProdutoPromocional.FornecedorId) &&
                                    c.ProdutoPromocional.FimPromocao > DateTime.Now &&
                                    c.ProdutoPromocionalId != null);
                }
                else
                {

                    if (categoria > 0)
                    {
                        produtos = _produtosRep.GetAll()
                       .Where(c => c.SubCategoriaId == subcategoria &&
                                    c.SubCategoria.CategoriaId == categoria &&
                                    c.ProdutoPromocional.Ativo &&
                                    c.ProdutoPromocional.Aprovado &&
                                    idFornecedores.Contains(c.ProdutoPromocional.FornecedorId) &&
                                    c.ProdutoPromocional.FimPromocao > DateTime.Now &&
                                    c.ProdutoPromocionalId != null)
                       .OrderBy(c => c.DescProduto)
                       .Skip(currentPage * currentPageSize)
                       .Take(currentPageSize)
                       .ToList();

                        totalProdutos = _produtosRep.GetAll()
                        .Count(c => c.SubCategoriaId == subcategoria &&
                             c.SubCategoria.CategoriaId == categoria &&
                             c.ProdutoPromocional.Ativo &&
                             c.ProdutoPromocional.Aprovado &&
                             idFornecedores.Contains(c.ProdutoPromocional.FornecedorId) &&
                             c.ProdutoPromocional.FimPromocao > DateTime.Now &&
                             c.ProdutoPromocionalId != null);
                    }
                    else
                    {
                        produtos = _produtosRep.GetAll()
                        .Where(c => c.ProdutoPromocional.Ativo &&
                                 c.ProdutoPromocional.Aprovado &&
                                 idFornecedores.Contains(c.ProdutoPromocional.FornecedorId) &&
                                 c.ProdutoPromocional.FimPromocao > DateTime.Now &&
                                 c.ProdutoPromocionalId != null)
                        .OrderBy(c => c.DescProduto)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                        totalProdutos = _produtosRep.GetAll()
                        .Count(c => c.ProdutoPromocional.Ativo &&
                             c.ProdutoPromocional.Aprovado &&
                             idFornecedores.Contains(c.ProdutoPromocional.FornecedorId) &&
                             c.ProdutoPromocional.FimPromocao > DateTime.Now &&
                             c.ProdutoPromocionalId != null);
                    }
                }

                IEnumerable<ProdutoPromocionalViewModel> produtosVM =
                    Mapper.Map<IEnumerable<Produto>, IEnumerable<ProdutoPromocionalViewModel>>(produtos);


                PaginacaoConfig<ProdutoPromocionalViewModel> pagSet = new PaginacaoConfig<ProdutoPromocionalViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalProdutos,
                    TotalPages = (int)Math.Ceiling((decimal)totalProdutos / currentPageSize),
                    Items = produtosVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, new { Produtos = pagSet, ProdutosFranquiaId = produtosFranquia });

                return response;
            });
        }

        [HttpGet]
        [Route("pesquisarmembro/{page:int=0}/{pageSize=4}/{categoria:int=0}/{subcategoria:int=0}/{filter?}")]
        public HttpResponseMessage GetProdutosMembro(HttpRequestMessage request, int? page, int? pageSize, int? categoria, int? subcategoria, string filter = null)
        {

            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Produto> produtos = null;
                List<Produto> listaProdutos = new List<Produto>();
                int totalProdutos = new int();
                var dataHoje = DateTime.Now;
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                Membro membro = _membroRep.FirstOrDefault(f => f.PessoaId == usuario.PessoaId);
                var franquia = membro.FranquiaId;
                var fornecedoresMembro = membro.MembroFornecedores.Select(x => x.FornecedorId).ToList();
                var membrocategorias = membro.MembroCategorias;
                var categoriasMembro = membrocategorias.Select(x => x.CategoriaId).ToList();

                var produtosFranquiaId = membro.Franquia?.Produtos
                          .Where(x => x.Produto.ProdutoPromocionalId != null &&
                          x.Produto.ProdutoPromocional.Ativo &&
                          x.Produto.ProdutoPromocional.Aprovado &&
                          x.Produto.ProdutoPromocional.FimPromocao >= dataHoje &&
                          x.Produto.ProdutoPromocional.QuantidadeProduto > 0 &&
                         x.Produto.ProdutoPromocional.QuantidadeProduto >= x.Produto.ProdutoPromocional.QtdMinVenda)
                .Select(x => x.ProdutoId).ToList();

                if (franquia != null)
                {
                    #region Entra somente quando Franquia

                    if (fornecedoresMembro.Any() && produtosFranquiaId.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            filter = filter.Trim().ToLower();

                            produtos = _produtosRep.FindBy(c => c.DescProduto.ToLower().Contains(filter) ||
                                                                    c.SubCategoria.Categoria.DescCategoria.ToLower().Contains(filter) &&
                                                                    categoriasMembro.Contains(c.SubCategoria.CategoriaId) &&
                                                                    fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId) &&
                                                                    produtosFranquiaId.Contains(c.Id))
                                                                .OrderBy(c => c.SubCategoria.Categoria.DescCategoria)
                                                                .Skip(currentPage * currentPageSize)
                                                                .Take(currentPageSize)
                                                                .ToList();

                            if (produtos.Count > 0)
                            {
                                listaProdutos.AddRange(produtos);
                            }


                            totalProdutos = _produtosRep
                                .GetAll()
                                .Count(c => c.DescProduto.ToLower().Contains(filter) ||
                                            c.SubCategoria.Categoria.DescCategoria.ToLower().Contains(filter) &&
                                            c.SubCategoriaId == subcategoria &&
                                            c.SubCategoria.CategoriaId == categoria &&
                                            fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId) &&
                                            produtosFranquiaId.Contains(c.Id));
                        }
                        else
                        {
                            if (categoria > 0)
                            {

                                produtos = _produtosRep.GetAll()
                               .Where(c => categoriasMembro.Contains(categoria.Value) &&
                                           fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId) &&
                                           produtosFranquiaId.Contains(c.Id))
                               .OrderBy(c => c.DescProduto)
                               .Skip(currentPage * currentPageSize)
                               .Take(currentPageSize)
                               .ToList();

                                if (produtos.Count > 0)
                                {
                                    listaProdutos.AddRange(produtos);
                                }

                                totalProdutos = _produtosRep.GetAll().Count(c =>
                                     c.SubCategoriaId == subcategoria &&
                                     c.SubCategoria.CategoriaId == categoria &&
                                     fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId) &&
                                     produtosFranquiaId.Contains(c.Id));
                            }
                            else if (membrocategorias.Count > 0)
                            {

                                produtos = _produtosRep.GetAll()
                                .Where(c => categoriasMembro.Contains(c.SubCategoria.CategoriaId) &&
                                     fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId) &&
                                     produtosFranquiaId.Contains(c.Id))
                                .OrderBy(c => c.DescProduto)
                                .Skip(currentPage * currentPageSize)
                                .Take(currentPageSize)
                                .ToList();

                                if (produtos.Count > 0)
                                {
                                    listaProdutos.AddRange(produtos);
                                }

                                totalProdutos += _produtosRep.GetAll().Count(c =>
                                    categoriasMembro.Contains(c.SubCategoria.CategoriaId) &&
                                     fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId) &&
                                     produtosFranquiaId.Contains(c.Id));
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region Entra quando não é Franquia

                    if (!string.IsNullOrEmpty(filter))
                    {
                        filter = filter.Trim().ToLower();

                        produtos = _produtosRep.FindBy(c => c.DescProduto.ToLower().Contains(filter) ||
                                                                c.SubCategoria.Categoria.DescCategoria.ToLower()
                                                                .Contains(filter) &&
                                                                c.SubCategoriaId == subcategoria &&
                                                                c.SubCategoria.CategoriaId == categoria &&
                                                                c.ProdutoPromocional.Ativo &&
                                                                c.ProdutoPromocional.Aprovado &&
                                                                c.ProdutoPromocional.FimPromocao >= dataHoje &&
                                                                c.ProdutoPromocional.QuantidadeProduto > 0 &&
                                                                c.ProdutoPromocional.QuantidadeProduto >= c.ProdutoPromocional.QtdMinVenda &&
                                                                fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId))
                                                            .OrderBy(c => c.SubCategoria.Categoria.DescCategoria)
                                                            .Skip(currentPage * currentPageSize)
                                                            .Take(currentPageSize)
                                                            .ToList();

                        if (produtos.Count > 0)
                        {
                            listaProdutos.AddRange(produtos);
                        }


                        totalProdutos = _produtosRep
                            .GetAll()
                            .Count(c => c.DescProduto.ToLower().Contains(filter) ||
                                        c.SubCategoria.Categoria.DescCategoria.ToLower().Contains(filter) &&
                                        c.SubCategoriaId == subcategoria &&
                                        c.SubCategoria.CategoriaId == categoria &&
                                        c.ProdutoPromocional.Ativo &&
                                        c.ProdutoPromocional.Aprovado &&
                                        c.ProdutoPromocional.FimPromocao >= dataHoje &&
                                        c.ProdutoPromocional.QuantidadeProduto > 0 &&
                                        c.ProdutoPromocional.QuantidadeProduto >= c.ProdutoPromocional.QtdMinVenda &&
                                        fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId));
                    }
                    else
                    {

                        if (categoria > 0)
                        {
                            produtos = _produtosRep.GetAll()
                           .Where(c => c.SubCategoriaId == subcategoria &&
                                        c.SubCategoria.CategoriaId == categoria &&
                                        c.ProdutoPromocional.Ativo &&
                                        c.ProdutoPromocional.Aprovado &&
                                        c.ProdutoPromocional.FimPromocao >= dataHoje &&
                                        c.ProdutoPromocional.QuantidadeProduto > 0 &&
                                        c.ProdutoPromocional.QuantidadeProduto >= c.ProdutoPromocional.QtdMinVenda &&
                                        fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId))
                           .OrderBy(c => c.DescProduto)
                           .Skip(currentPage * currentPageSize)
                           .Take(currentPageSize)
                           .ToList();

                            if (produtos.Count > 0)
                            {
                                listaProdutos.AddRange(produtos);
                            }

                            totalProdutos = _produtosRep.GetAll().Count(c =>
                                 c.SubCategoriaId == subcategoria &&
                                 c.SubCategoria.CategoriaId == categoria &&
                                 c.ProdutoPromocional.Ativo &&
                                 c.ProdutoPromocional.Aprovado &&
                                 c.ProdutoPromocional.FimPromocao >= dataHoje &&
                                 c.ProdutoPromocional.QuantidadeProduto > 0 &&
                                 c.ProdutoPromocional.QuantidadeProduto >= c.ProdutoPromocional.QtdMinVenda &&
                                 fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId));
                        }
                        else if (membrocategorias.Count > 0)
                        {
                            var categoriasId = membrocategorias.Select(x => x.CategoriaId).ToList();

                            produtos = _produtosRep.GetAll()
                            .Where(c => categoriasId.Contains(c.SubCategoria.CategoriaId) &&
                                 c.ProdutoPromocional.Ativo &&
                                 c.ProdutoPromocional.Aprovado &&
                                 c.ProdutoPromocional.FimPromocao >= dataHoje &&
                                 c.ProdutoPromocional.QuantidadeProduto > 0 &&
                                 c.ProdutoPromocional.QuantidadeProduto >= c.ProdutoPromocional.QtdMinVenda &&
                                 fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId))
                            .OrderBy(c => c.DescProduto)
                            .Skip(currentPage * currentPageSize)
                            .Take(currentPageSize)
                            .ToList();

                            if (produtos.Count > 0)
                            {
                                listaProdutos.AddRange(produtos);
                            }

                            totalProdutos += _produtosRep.GetAll().Count(c =>
                                 categoriasId.Contains(c.SubCategoria.CategoriaId) &&
                                 c.ProdutoPromocional.Ativo &&
                                 c.ProdutoPromocional.Aprovado &&
                                 c.ProdutoPromocional.FimPromocao >= dataHoje &&
                                 c.ProdutoPromocional.QuantidadeProduto > 0 &&
                                 c.ProdutoPromocional.QuantidadeProduto >= c.ProdutoPromocional.QtdMinVenda &&
                                 fornecedoresMembro.Contains(c.ProdutoPromocional.FornecedorId));
                        }
                    }

                    #endregion
                }

                var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");

                IEnumerable<ProdutoPromocionalViewModel> produtosVM =
                    Mapper.Map<IEnumerable<Produto>, IEnumerable<ProdutoPromocionalViewModel>>(listaProdutos);


                #region Prazo de Entrega Semanal/Dias

                foreach (var vm in produtosVM)
                {
                    var regiaoDias = vm.Fornecedor.FornecedorRegiao
                    .FirstOrDefault(x => membro.Pessoa.Enderecos.Select(c => c.CidadeId).Contains(x.CidadeId));

                    var regiaoSemanal = vm.Fornecedor.FornecedorPrazoSemanal
                        .Where(x => membro.Pessoa.Enderecos.Select(c => c.CidadeId).Contains(x.CidadeId)).ToList();

                    if (regiaoDias != null)
                    {
                        vm.Fornecedor.PrazoEntrega = regiaoDias.Prazo;
                        vm.Fornecedor.VlPedidoMin = "";
                        vm.Fornecedor.FornecedorPrazoSemanal.Clear();
                    }
                    if (regiaoSemanal.Any())
                    {
                        vm.Fornecedor.PrazoEntrega = 0;
                        vm.Fornecedor.FornecedorPrazoSemanal.Clear();
                        vm.Fornecedor.FornecedorPrazoSemanal = regiaoSemanal;
                        vm.Fornecedor.VlPedidoMin = "";

                    }
                }

                #endregion


                PaginacaoConfig<ProdutoPromocionalViewModel> pagSet = new PaginacaoConfig<ProdutoPromocionalViewModel>()
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

        [HttpGet]
        [Route("fornecedorcategoria")]
        public HttpResponseMessage GetCategoriaFornecedor(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                Fornecedor fornecedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.PessoaId).FirstOrDefault();

                var idsCategoriasFornecedor =
                    _fornecedorCategoriaRep.GetAll().Where(c => c.FornecedorId == fornecedor.Id).Select(c => c.CategoriaId).ToList();

                List<Categoria> ListaCategorias = new List<Categoria>();

                foreach (var cat in idsCategoriasFornecedor)
                {
                    var categorias = _categoriaRep.FindBy(c => c.Id == cat).FirstOrDefault();

                    ListaCategorias.Add(categorias);
                }


                IEnumerable<CategoriaViewModel> categoriasVM =
                    Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(ListaCategorias);

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

                var categoriasPromocao = _produtosRep.GetAll().Where(c => c.Ativo &&
                    c.ProdutoPromocional.Ativo &&
                    !c.ProdutoPromocional.Aprovado ||
                     c.ProdutoPromocional.Aprovado)
                    .Select(c => c.SubCategoria.Categoria)
                    .ToList().Distinct();


                IEnumerable<CategoriaViewModel> categoriasVM =
                    Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(categoriasPromocao);

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
        [Route("formaspagamentofornecedor")]
        public HttpResponseMessage GetFormasPagamentoFornecedor(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                Fornecedor fornecedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.PessoaId).FirstOrDefault();

                var formasPagFornecedor = fornecedor.FornecedorFormaPagtos.Where(p => p.Ativo).Select(f => new { f.FormaPagto, f.Desconto }).ToList();

                var result = formasPagFornecedor.Select(x => new FormaPagtoViewModel
                {
                    Id = x.FormaPagto.Id,
                    Avista = x.FormaPagto.Avista,
                    DescFormaPagto = x.FormaPagto.DescFormaPagto,
                    Desconto = x.Desconto
                });

                response = request.CreateResponse(HttpStatusCode.OK, result);

                return response;
            });
        }

        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, ProdutoPromocionalViewModel produtoPromocionalViewModel)
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

                    Fornecedor fornecedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.PessoaId).FirstOrDefault();

                    var descProduto =
                       _produtosRep.GetAll()
                       .Where(p => p.DescProduto.ToLower() == produtoPromocionalViewModel.DescProduto.ToLower())
                       .Select(d => d.DescProduto).FirstOrDefault();

                    //if (string.IsNullOrEmpty(descProduto))
                    //{
                    //Inseri Dados da Promoção
                    DateTime dataFimPromo = new DateTime(produtoPromocionalViewModel.FimPromocao.Year,
                                                                produtoPromocionalViewModel.FimPromocao.Month,
                                                                produtoPromocionalViewModel.FimPromocao.Day,
                                                                23, 59, 59);
                    DateTime dataInicioPromo = new DateTime(produtoPromocionalViewModel.InicioPromocao.Year,
                                                                produtoPromocionalViewModel.InicioPromocao.Month,
                                                                produtoPromocionalViewModel.InicioPromocao.Day,
                                                                0, 0, 0);


                    ProdutoPromocional novaPromocao = new ProdutoPromocional
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        FornecedorId = fornecedor.Id,
                        MotivoPromocao = produtoPromocionalViewModel.MotivoPromocao,
                        QuantidadeProduto = produtoPromocionalViewModel.QtdProdutos,
                        PrecoPromocao = produtoPromocionalViewModel.PrecoPromocao.TryParseDecimal(),
                        QtdMinVenda = produtoPromocionalViewModel.QtdMinVenda,
                        ValidadeProduto = produtoPromocionalViewModel.ValidadeProd,
                        InicioPromocao = dataInicioPromo,
                        FimPromocao = dataFimPromo,
                        Ativo = true,
                        Aprovado = false,

                        FreteGratis = produtoPromocionalViewModel.FreteGratis,
                        ObsFrete = produtoPromocionalViewModel.ObsFrete

                    };

                    _produtoPromocionalRep.Add(novaPromocao);
                    _unitOfWork.Commit();


                    //Inseri o produto
                    Produto novoproduto = new Produto()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        SubCategoriaId = produtoPromocionalViewModel.SubCategoriaId,
                        UnidadeMedidaId = produtoPromocionalViewModel.UnidadeMedidaId,
                        FabricanteId = null,
                        MarcaId = produtoPromocionalViewModel.MarcaId,
                        DescProduto = produtoPromocionalViewModel.DescProduto,
                        Especificacao = produtoPromocionalViewModel.Especificacao,
                        PrecoMedio = produtoPromocionalViewModel.PrecoMedio.TryParseDecimal(),
                        ProdutoPromocionalId = novaPromocao.Id,
                        Ativo = produtoPromocionalViewModel.Ativo
                    };

                    _produtosRep.Add(novoproduto);
                    _unitOfWork.Commit();

                    //Enviar notificação para o topo do ADM através do SignalRHub do total de produtos a serem aprovados
                    var totalAprovaPromocao = _produtosRep.GetAll().Count(x => x.Ativo && !x.ProdutoPromocional.Aprovado);
                    var _context = GlobalHost.ConnectionManager.GetHubContext<NotificacoesHub>();
                    _context.Clients.All.aprovarPromocao(totalAprovaPromocao);


                    //Inseri a forma de pagamento para a Promoção
                    if (produtoPromocionalViewModel.PromocaoFormaPagto.Count > 0)
                    {
                        foreach (var formapagto in produtoPromocionalViewModel.PromocaoFormaPagto)
                        {
                            PromocaoFormaPagto formaPagtoObj = new PromocaoFormaPagto()
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                ProdutoPromocionalId = novaPromocao.Id,
                                FormaPagtoId = formapagto.FormaPagtoId,
                                Ativo = true
                            };
                            _promocaoFormaPagtoRep.Add(formaPagtoObj);
                            _unitOfWork.Commit();
                        }
                    }

                    // Update view model
                    ProdutoPromocionalViewModel vmProduto = Mapper.Map<Produto, ProdutoPromocionalViewModel>(novoproduto);

                    vmProduto.CategoriaId = produtoPromocionalViewModel.CategoriaId;

                    response = request.CreateResponse(HttpStatusCode.Created, vmProduto);
                    //}
                    //else
                    //{
                    //    response = request.CreateResponse(HttpStatusCode.BadRequest, "Nome do produto já existe, utilize outro nome");
                    //}

                }

                return response;
            });

        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, ProdutoPromocionalViewModel produtoPromocionalViewModel)
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

                    // var amb = Environment.GetEnvironmentVariable("Amb_EconomizaJa");


                    var amb = "HOM";
                    string strArqAnt1 = String.Empty;
                    string strArqAnt2 = String.Empty;

                    bool booSubCategoriaAlterada = false;

                    Produto novoProduto = _produtosRep.GetSingle(produtoPromocionalViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    DirectoryInfo diretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb + "_CamImagens"] +
                                         novoProduto.SubCategoria.Categoria.Id + @"\" + novoProduto.SubCategoria.Id);

                    if (novoProduto.SubCategoria.Id != produtoPromocionalViewModel.SubCategoriaId)
                        booSubCategoriaAlterada = true;

                    if (novoProduto.Imagens.Count > 0)
                    {
                        strArqAnt1 = diretorio + @"\" + novoProduto.Imagens.FirstOrDefault().CaminhoImagem;
                        strArqAnt2 = diretorio + @"\" + novoProduto.Imagens.FirstOrDefault().CaminhoImagemGrande;
                    }

                    DateTime dataFimPromo = new DateTime(produtoPromocionalViewModel.FimPromocao.Year,
                                                                   produtoPromocionalViewModel.FimPromocao.Month,
                                                                   produtoPromocionalViewModel.FimPromocao.Day,
                                                                   23, 59, 59);
                    DateTime dataInicioPromo = new DateTime(produtoPromocionalViewModel.InicioPromocao.Year,
                                                                produtoPromocionalViewModel.InicioPromocao.Month,
                                                                produtoPromocionalViewModel.InicioPromocao.Day,
                                                                0, 0, 0);


                    novoProduto.UsuarioAlteracao = usuario;
                    novoProduto.DtAlteracao = DateTime.Now;
                    novoProduto.Ativo = produtoPromocionalViewModel.Ativo;
                    novoProduto.SubCategoriaId = produtoPromocionalViewModel.SubCategoriaId;
                    novoProduto.UnidadeMedidaId = produtoPromocionalViewModel.UnidadeMedidaId;
                    novoProduto.FabricanteId = null;
                    novoProduto.DescProduto = produtoPromocionalViewModel.DescProduto;
                    novoProduto.Especificacao = produtoPromocionalViewModel.Especificacao;
                    novoProduto.PrecoMedio = produtoPromocionalViewModel.PrecoMedio.TryParseDecimal();
                    novoProduto.ProdutoPromocional.PrecoPromocao = produtoPromocionalViewModel.PrecoPromocao.TryParseDecimal();
                    novoProduto.MarcaId = produtoPromocionalViewModel.MarcaId;
                    novoProduto.ProdutoPromocional.FornecedorId = produtoPromocionalViewModel.FornecedorId;
                    novoProduto.ProdutoPromocional.QuantidadeProduto = produtoPromocionalViewModel.QtdProdutos;
                    novoProduto.ProdutoPromocional.QtdMinVenda = produtoPromocionalViewModel.QtdMinVenda;
                    novoProduto.ProdutoPromocional.MotivoPromocao = produtoPromocionalViewModel.MotivoPromocao;
                    novoProduto.ProdutoPromocional.ValidadeProduto = produtoPromocionalViewModel.ValidadeProd;
                    novoProduto.ProdutoPromocional.InicioPromocao = dataInicioPromo;
                    novoProduto.ProdutoPromocional.FimPromocao = dataFimPromo;
                    novoProduto.ProdutoPromocional.Aprovado = false;
                    novoProduto.ProdutoPromocional.Ativo = true;

                    novoProduto.ProdutoPromocional.FreteGratis = produtoPromocionalViewModel.FreteGratis;
                    novoProduto.ProdutoPromocional.ObsFrete = produtoPromocionalViewModel.ObsFrete;

                    _produtosRep.Edit(novoProduto);
                    _unitOfWork.Commit();


                    #region Deletando e inserindo as formas de pagamento

                    if (produtoPromocionalViewModel.PromocaoFormaPagto.Count > 0)
                    {
                        var promocaoPagamento =
                            _promocaoFormaPagtoRep.GetAll()
                                .Where(p => p.ProdutoPromocionalId == novoProduto.ProdutoPromocionalId)
                                .ToList();

                        _promocaoFormaPagtoRep.DeleteAll(promocaoPagamento);
                        _unitOfWork.Commit();

                        foreach (var pag in produtoPromocionalViewModel.PromocaoFormaPagto)
                        {
                            PromocaoFormaPagto newPromocaoFormaPagto = new PromocaoFormaPagto()
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                ProdutoPromocionalId = (int)novoProduto.ProdutoPromocionalId,
                                FormaPagtoId = pag.FormaPagtoId,
                                Ativo = true
                            };

                            _promocaoFormaPagtoRep.Add(newPromocaoFormaPagto);
                            _unitOfWork.Commit();
                        }


                    }

                    #endregion


                    // Update view model
                    produtoPromocionalViewModel = Mapper.Map<Produto, ProdutoPromocionalViewModel>(novoProduto);

                    if (booSubCategoriaAlterada)
                    {
                        DirectoryInfo NovoDiretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb + "_CamImagens"] +
                                         novoProduto.SubCategoria.Categoria.Id + @"\" + novoProduto.SubCategoria.Id);

                        string strArqNovo1 = NovoDiretorio + @"\" + novoProduto.Imagens.FirstOrDefault().CaminhoImagem;
                        string strArqNovo2 = NovoDiretorio + @"\" + novoProduto.Imagens.FirstOrDefault().CaminhoImagem;

                        File.Move(strArqAnt1, strArqNovo1);
                        File.Move(strArqAnt2, strArqNovo2);
                    }

                    #region Quando o produto é alterado é removido da tabela de FranquiaProduto

                    var produtosPromocaoFranquias = _franquiaProdutoRep.GetAll()
                        .Where(x => x.ProdutoId == novoProduto.Id).ToList();
                    if (produtosPromocaoFranquias.Any())
                    {
                        _franquiaProdutoRep.DeleteAll(produtosPromocaoFranquias);
                        _unitOfWork.Commit();
                    }

                    #endregion

                    response = request.CreateResponse(HttpStatusCode.OK, produtoPromocionalViewModel);


                    //Enviar notificação para o topo do ADM através do SignalRHub do total de produtos a serem aprovados depois de atualizado
                    var totalAprovaPromocao = _produtosRep.GetAll().Count(x => x.Ativo && !x.ProdutoPromocional.Aprovado);
                    var _context = GlobalHost.ConnectionManager.GetHubContext<NotificacoesHub>();
                    _context.Clients.All.aprovarPromocao(totalAprovaPromocao);

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

            DirectoryInfo diretorio = new DirectoryInfo(ConfigurationManager.AppSettings[amb + "_CamImagens"] +
                                 produto.SubCategoria.Categoria.Id + @"\" + produto.SubCategoria.Id);


            if (produto.Imagens.Count > 0)
            {

                imgId = produto.Imagens.FirstOrDefault().Id;

                strArqAnt = diretorio + @"\" + produto.Imagens.FirstOrDefault().CaminhoImagem;

                strArqAntGrande = diretorio + @"\" + produto.Imagens.FirstOrDefault().CaminhoImagemGrande;

                //Este código serve para delete a imagem anterior depois q ele insere a nova caso ja exista uma
                if (imgId > 0 && imgId != null)
                {
                    //Deleta as imagens antigas da pasta
                    File.Delete(strArqAnt);
                    File.Delete(strArqAntGrande);

                    //Deleta do banco as imagens antigas
                    var imgant = _imagemRep.GetSingle(imgId);
                    _imagemRep.Delete(imgant);
                    _unitOfWork.Commit();
                }

            }


            Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

            //Só entra se no controller eu passar a variável 'imageGr' como false
            #region Cadastro da Imagem

            if (imageGr == false)
            {

                var multipartFormDataStreamProvider = new UploadMultipartFormProvider(diretorio.ToString(), produtoId.ToString(), imageGrande);


                // Read the MIME multipart asynchronously 
                await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);


                string _localFileName = multipartFormDataStreamProvider.FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();

                //Redimensionando imagem para 400x400
                var _imagemGrande = await ResizeImageAndSave(_localFileName, 400, 400, "ProGr");

                //Redimensionando imagem para 200x200
                var _imagemPequena = await ResizeImageAndSave(_localFileName, 200, 200, "ProPeq");


                //Apaga imagem original
                File.Delete(_localFileName);


                Imagem novaImagem = new Imagem
                {
                    UsuarioCriacao = usuario,
                    DtCriacao = DateTime.Now,
                    Produto = produto,
                    CaminhoImagem = Path.GetFileName(_imagemPequena),
                    CaminhoImagemGrande = Path.GetFileName(_imagemGrande),
                    Ativo = produto.Ativo
                };


                //verifica se diretório existe, caso exista ele não cria a pasta.
                if (diretorio.Exists)
                {

                    _imagemRep.Add(novaImagem);

                    _unitOfWork.Commit();

                    // Update view model
                    var imagemVM = Mapper.Map<Imagem, ImagemViewModel>(novaImagem);


                    imagemVM.CaminhoImagem = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                         produto.SubCategoria.Categoria.Id + @"/" +
                                                         produto.SubCategoria.Id + @"/" + imagemVM.CaminhoImagem;


                    imagemVM.CaminhoImagemGrande = ConfigurationManager.AppSettings[amb + "_CamImagensExibi"] +
                                                produto.SubCategoria.Categoria.Id + @"/" +
                                                produto.SubCategoria.Id + @"/" + imagemVM.CaminhoImagemGrande;


                    response = request.CreateResponse(HttpStatusCode.OK, imagemVM);

                }

            }

            #endregion

            return response;
        }

        //Método que redimensiona as imagens para o tamanho necessário
        public async Task<string> ResizeImageAndSave(string imagePath, int largura, int altura, string prefixo)
        {

            Image fullSizeImg = Image.FromFile(imagePath);
            var thumbnailImg = new Bitmap(largura, altura);
            var thumbGraph = Graphics.FromImage(thumbnailImg);
            thumbGraph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            thumbGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            thumbGraph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            var imageRectangle = new Rectangle(0, 0, largura, altura);
            thumbGraph.DrawImage(fullSizeImg, imageRectangle);
            string targetPath = imagePath.Replace(Path.GetFileNameWithoutExtension(imagePath), Path.GetFileNameWithoutExtension(imagePath) + prefixo);
            thumbnailImg.Save(targetPath);
            thumbnailImg.Dispose();
            thumbGraph.Dispose();
            fullSizeImg.Dispose();
            return targetPath;
        }

        [HttpPost]
        [Route("salvarAprovacaoPromocao/{aprovacao:bool}")]
        public HttpResponseMessage SalvarAprovacaoPromocao(HttpRequestMessage request, ProdutoPromocionalViewModel produtoPromocionalViewModel, bool aprovacao)
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
                    var novoProduto = _produtosRep.GetSingle(produtoPromocionalViewModel.Id);
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    if (aprovacao)
                    {
                        novoProduto.UsuarioAlteracao = usuario;
                        novoProduto.DtAlteracao = DateTime.Now;
                        novoProduto.ProdutoPromocional.Ativo = true;
                        novoProduto.ProdutoPromocional.Aprovado = true;

                        //grava todas as ativações da promoção na tabela de HistoricoPromocao
                        HistoricoPromocao historicoPromocao = new HistoricoPromocao()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            ProdutoId = novoProduto.Id,
                            FornecedorId = produtoPromocionalViewModel.FornecedorId,
                            QuantidadeProduto = produtoPromocionalViewModel.QtdProdutos,
                            Preco = produtoPromocionalViewModel.PrecoPromocao.TryParseDecimal(),
                            MotivoPromocao = produtoPromocionalViewModel.MotivoPromocao,
                            InicioPromocao = produtoPromocionalViewModel.InicioPromocao,
                            FimPromocao = produtoPromocionalViewModel.FimPromocao,
                            Ativo = true
                        };

                        _produtosRep.Edit(novoProduto);
                        _historicoPromocaoRep.Add(historicoPromocao);

                        _unitOfWork.Commit();


                        var fornecedorPromo = _fornecedorRep.GetSingle(produtoPromocionalViewModel.FornecedorId);

                        var usuarioFornecedorPromocao = fornecedorPromo.Pessoa.Usuarios.FirstOrDefault();

                        //Envia e-mail para fornecedor cadastro do produto promociona foi aprovado
                        var corpoEmail = _templateEmailRep.FindBy(e => e.Id == 38).Select(e => e.Template).FirstOrDefault();

                        Emails email = new Emails()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Economiza Já - Cadastro de produto Promocional Aprovado.",
                            EmailDestinatario = usuarioFornecedorPromocao.UsuarioEmail,
                            CorpoEmail = corpoEmail,
                            Status = Status.NaoEnviado,
                            Origem = Origem.AprovaCadastroProdutoPromocional,
                            Ativo = true

                        };
                        _emailsRep.Add(email);

                        Sms sms = new Sms()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,

                            Numero = usuarioFornecedorPromocao.Telefones.Where(x => x.UsuarioTelId == usuarioFornecedorPromocao.Id).Select(t => t.DddCel).FirstOrDefault() +
                            usuarioFornecedorPromocao.Telefones.Where(x => x.UsuarioTelId == usuarioFornecedorPromocao.Id).Select(t => t.Celular).FirstOrDefault(),

                            Mensagem = "Economiza Já - Cadastro do produto promocional Aprovado.",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.AprovaCadastroProdutoPromocional,
                            Ativo = true

                        };

                        _smsRep.Add(sms);
                        _unitOfWork.Commit();
                    }
                    else
                    {
                        novoProduto.UsuarioAlteracao = usuario;
                        novoProduto.DtAlteracao = DateTime.Now;
                        novoProduto.ProdutoPromocional.Ativo = false;
                        novoProduto.ProdutoPromocional.Aprovado = false;
                        novoProduto.ProdutoPromocional.DescMotivoCancelamento = produtoPromocionalViewModel.DescMotivoCancelamento;

                        _produtosRep.Edit(novoProduto);
                        _unitOfWork.Commit();


                        var fornecedorPromo = _fornecedorRep.GetSingle(produtoPromocionalViewModel.FornecedorId);

                        var usuarioFornecedorPromocao = fornecedorPromo.Pessoa.Usuarios.FirstOrDefault();

                        //Envia e-mail para fornecedor cadastro do produto promociona foi cancelado
                        var corpoEmail = _templateEmailRep.FindBy(e => e.Id == 39).Select(e => e.Template).FirstOrDefault();

                        Emails email = new Emails()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Economiza Já - Cadastro do produto promocinal Reprovado, acesse a plataforma e veja o motivo",
                            EmailDestinatario = usuarioFornecedorPromocao.UsuarioEmail,
                            CorpoEmail = corpoEmail,
                            Status = Status.NaoEnviado,
                            Origem = Origem.AprovaCadastroProdutoPromocional,
                            Ativo = true

                        };
                        _emailsRep.Add(email);

                        Sms sms = new Sms()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,

                            Numero = usuarioFornecedorPromocao.Telefones.Where(x => x.UsuarioTelId == usuarioFornecedorPromocao.Id).Select(t => t.DddCel).FirstOrDefault() +
                            usuarioFornecedorPromocao.Telefones.Where(x => x.UsuarioTelId == usuarioFornecedorPromocao.Id).Select(t => t.Celular).FirstOrDefault(),

                            Mensagem = "Economiza Já - Cadastro do produto promocinal Reprovado, acesse a plataforma e veja o motivo.",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.AprovaCadastroProdutoPromocional,
                            Ativo = true

                        };

                        _smsRep.Add(sms);
                        _unitOfWork.Commit();

                    }

                 


                 


                    // Update view model
                    produtoPromocionalViewModel = Mapper.Map<Produto, ProdutoPromocionalViewModel>(novoProduto);

                    response = request.CreateResponse(HttpStatusCode.OK, produtoPromocionalViewModel);
                }

                return response;
            });
        }

        [HttpPost]
        [Route("aprovaCancelaPromocaoFranquia/{aprovacao:bool}")]
        public HttpResponseMessage AprovaCancelaPromocaoFranquia(HttpRequestMessage request, ProdutoPromocionalViewModel produtoPromocionalViewModel, bool aprovacao)
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
                    var franquiaProd = new FranquiaProduto();
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                    if (aprovacao)
                    {
                        franquiaProd.UsuarioCriacao = usuario;
                        franquiaProd.DtCriacao = DateTime.Now;
                        franquiaProd.ProdutoId = produtoPromocionalViewModel.Id;
                        franquiaProd.FranquiaId = franquia.Id;
                        franquiaProd.Ativo = true;

                        _franquiaProdutoRep.Add(franquiaProd);
                        _unitOfWork.Commit();

                        // Update view model
                        produtoPromocionalViewModel = Mapper.Map<Produto, ProdutoPromocionalViewModel>(franquiaProd?.Produto);

                        response = request.CreateResponse(HttpStatusCode.OK, produtoPromocionalViewModel);
                    }
                    else
                    {
                        franquiaProd = franquia.Produtos
                        .FirstOrDefault(x => x.FranquiaId == franquia.Id && x.ProdutoId == produtoPromocionalViewModel.Id);

                        if (franquiaProd != null)
                        {
                            _franquiaProdutoRep.Delete(franquiaProd);
                            _unitOfWork.Commit();

                            response = request.CreateResponse(HttpStatusCode.OK);
                        }
                    }
                }

                return response;
            });
        }

        [HttpGet]
        [Route("totalProdutoPromocao")]
        public HttpResponseMessage GetTotalProdutoPromocao(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var totalprod = _produtosRep.GetAll().Count(x => x.Ativo && !x.ProdutoPromocional.Aprovado);

                response = request.CreateResponse(HttpStatusCode.OK, totalprod);

                return response;
            });
        }

        private void DeletaProdutoFranquia(Franquia franquia)
        {
            var produtosFranquia = franquia.Produtos.
                Where(c => c.Produto.ProdutoPromocionalId != null &&
                           c.Produto.ProdutoPromocional.FimPromocao < DateTime.Now &&
                           c.Produto.ProdutoPromocional.Ativo &&
                           c.Produto.ProdutoPromocional.Aprovado).ToList();

            if (produtosFranquia.Count > 0)
            {
                _franquiaProdutoRep.DeleteAll(produtosFranquia);
                _unitOfWork.Commit();
            }

        }
    }
}
