using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using ECC.API_Web.Comum.Enum;
using ECC.API_Web.Hubs;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.API_Web.Models;
using ECC.Dados.Extensions;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeAvisos;
using ECC.EntidadeEmail;
using ECC.EntidadeEndereco;
using ECC.EntidadeFormaPagto;
using ECC.EntidadeFornecedor;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.Entidades;
using ECC.Entidades.EntidadePessoa;
using ECC.EntidadeSms;
using ECC.EntidadeUsuario;
using ECC.Servicos;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace ECC.API_Web.Controllers
{
    [EnableCors("*", "*", "*")]
    [Authorize(Roles = "Admin, Fornecedor")]
    [RoutePrefix("api/fornecedor")]
    public class FornecedorController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;
        private readonly IEntidadeBaseRep<Bairro> _bairroRep;
        private readonly IEntidadeBaseRep<Cidade> _cidadeRep;
        private readonly IEntidadeBaseRep<Emails> _emailsNotificaFornecedorMembro;
        private readonly IEntidadeBaseRep<Endereco> _enderecoRep;
        private readonly IEntidadeBaseRep<FormaPagto> _formaPagtoRep;
        private readonly IEntidadeBaseRep<FornecedorCategoria> _fornecedorCategoriaRep;
        private readonly IEntidadeBaseRep<FornecedorFormaPagto> _fornecedorFormaPagtoRep;
        private readonly IEntidadeBaseRep<FornecedorPrazoSemanal> _fornecedorPrazoSemanalRep;
        private readonly IEntidadeBaseRep<FornecedorRegiao> _fornecedorRegiaoRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<Membro> _membro;
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedor;
        private readonly IEntidadeBaseRep<PessoaJuridica> _pessoaJuridicaRep;
        private readonly IEntidadeBaseRep<Pessoa> _pessoaRep;
        private readonly IEntidadeBaseRep<Sms> _sms;
        private readonly IEntidadeBaseRep<SolicitacaoMembroFornecedor> _solicitacaoMembroFornecedor;
        private readonly IEntidadeBaseRep<Telefone> _telefoneRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmail;
        private readonly IEntidadeBaseRep<FornecedorFormaPagtoMembro> _fornecedorFormaPagtoMembro;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        //private readonly NotificacoesAlertasService _verificaNotificacoes;

        public FornecedorController(IEntidadeBaseRep<FornecedorCategoria> fornecedorCategoriaRep,
            IEntidadeBaseRep<FornecedorFormaPagto> fornecedorFormaPagtoRep,
            IEntidadeBaseRep<FormaPagto> formaPagtoRep,
            IEntidadeBaseRep<Telefone> telefoneRep,
            IEntidadeBaseRep<Pessoa> pessoaRep,
            IEntidadeBaseRep<PessoaJuridica> pessoaJuridicaRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<FornecedorRegiao> fornecedorRegiapRep,
            IEntidadeBaseRep<MembroFornecedor> membroFornecedor,
            IEntidadeBaseRep<Membro> membro,
            IEntidadeBaseRep<SolicitacaoMembroFornecedor> solicitacaoMembroFornecedor,
            IEntidadeBaseRep<TemplateEmail> templateEmail,
            IEntidadeBaseRep<Emails> emailsNotificaFornecedorMembro,
            IEntidadeBaseRep<Sms> smsRep,
            IEntidadeBaseRep<Bairro> bairroRep,
            IEntidadeBaseRep<Cidade> cidadeRep,
            IEntidadeBaseRep<Endereco> enderecoRep,
            IEntidadeBaseRep<Avisos> avisosRep,
            IEntidadeBaseRep<FornecedorFormaPagtoMembro> fornecedorFormaPagtoMembro,
            IEntidadeBaseRep<FornecedorPrazoSemanal> fornecedorPrazoSemanalRep,
            IEntidadeBaseRep<Erro> _errosRepository,
            IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _pessoaRep = pessoaRep;
            _pessoaJuridicaRep = pessoaJuridicaRep;
            _fornecedorRep = fornecedorRep;
            _usuarioRep = usuarioRep;
            _telefoneRep = telefoneRep;
            _formaPagtoRep = formaPagtoRep;
            _fornecedorFormaPagtoRep = fornecedorFormaPagtoRep;
            _fornecedorCategoriaRep = fornecedorCategoriaRep;
            _fornecedorRegiaoRep = fornecedorRegiapRep;
            _membroFornecedor = membroFornecedor;
            _membro = membro;
            _solicitacaoMembroFornecedor = solicitacaoMembroFornecedor;
            _templateEmail = templateEmail;
            _emailsNotificaFornecedorMembro = emailsNotificaFornecedorMembro;
            _sms = smsRep;
            _bairroRep = bairroRep;
            _cidadeRep = cidadeRep;
            _enderecoRep = enderecoRep;
            _fornecedorFormaPagtoMembro = fornecedorFormaPagtoMembro;
            _avisosRep = avisosRep;
            _fornecedorPrazoSemanalRep = fornecedorPrazoSemanalRep;
        }


        [HttpGet]
        [Route("pesquisar/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage Get(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            var currentPage = page.Value;
            var currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Fornecedor> fornecedores = null;
                var totalFornecedores = new int();


                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    fornecedores = _fornecedorRep.FindBy(
                            c => c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter) ||
                                 c.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                                 c.Pessoa.PessoaJuridica.Cnpj.ToLower().Contains(filter))
                        .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalFornecedores = _fornecedorRep
                        .GetAll()
                        .Count(c => c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter) ||
                                    c.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                                    c.Pessoa.PessoaJuridica.Cnpj.ToLower().Contains(filter));
                }
                else
                {
                    fornecedores = _fornecedorRep.GetAll()
                        .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalFornecedores = _fornecedorRep.GetAll().Count();
                }

                var fornecedoresVM =
                    Mapper.Map<IEnumerable<Fornecedor>, IEnumerable<FornecedorViewModel>>(fornecedores);

                var pagSet = new PaginacaoConfig<FornecedorViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalFornecedores,
                    TotalPages = (int)Math.Ceiling((decimal)totalFornecedores / currentPageSize),
                    Items = fornecedoresVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpGet]
        [Route("consultar/{fornecedorId:int=0}")]
        public HttpResponseMessage Consultar(HttpRequestMessage request, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var fornecedorVm = Mapper.Map<Fornecedor, FornecedorViewModel>(_fornecedorRep.GetSingle(fornecedorId));
                response = request.CreateResponse(HttpStatusCode.OK, fornecedorVm);

                return response;
            });
        }

        [HttpGet]
        [Route("perfil")]
        public HttpResponseMessage GetPerfil(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.GetAll().FirstOrDefault(x => x.PessoaId.Equals(usuario.PessoaId));
                var fornecedorVm = Mapper.Map<Fornecedor, FornecedorViewModel>(fornecedor);
                var response = request.CreateResponse(HttpStatusCode.OK, fornecedorVm);

                return response;
            });
        }

        [HttpGet]
        [Route("formaPagto")]
        public HttpResponseMessage GetFormaPagto(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var formaPagto = _formaPagtoRep.GetAll().Where(x => x.Ativo).ToList();

                var formaPagtoAvista = formaPagto.Where(x => x.Avista).ToList();
                var formaPagtoParcelado = formaPagto.Where(x => !x.Avista).ToList();

                var formaPagtosAvistaVM =
                    Mapper.Map<IEnumerable<FormaPagto>, IEnumerable<FormaPagtoViewModel>>(formaPagtoAvista);
                var formaPagtosParceladoVM =
                    Mapper.Map<IEnumerable<FormaPagto>, IEnumerable<FormaPagtoViewModel>>(formaPagtoParcelado);

                response = request.CreateResponse(HttpStatusCode.OK, new { formaPagtosAvistaVM, formaPagtosParceladoVM });

                return response;
            });
        }



        [HttpGet]
        [Route("formaPagtoCadastradaFornecedor")]
        public HttpResponseMessage GetFormaPagtoCadastradaFornecedor(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var fornecedor = _fornecedorRep.GetAll().FirstOrDefault(x => x.PessoaId.Equals(usuario.PessoaId));

                var formaPagto = _fornecedorFormaPagtoRep.FindBy(x => x.Ativo && x.FornecedorId==fornecedor.Id).ToList();

               var formaPagtosAvistaVM = formaPagto.Where(x => x.FormaPagto.Avista).Select(x=> new { x.Id, x.FormaPagto.DescFormaPagto, x.FormaPagto.Avista, x.FormaPagto.QtdParcelas, x.Desconto }).ToList();
                var formaPagtosParceladoVM = formaPagto.Where(x => !x.FormaPagto.Avista).Select(x => new { x.Id, x.FormaPagto.DescFormaPagto, x.FormaPagto.Avista, x.FormaPagto.QtdParcelas, x.Desconto, x.ValorMinParcela, x.ValorMinParcelaPedido }).ToList();
                response = request.CreateResponse(HttpStatusCode.OK, new { formaPagtosAvistaVM, formaPagtosParceladoVM });

                return response;
            });
        }

        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, FornecedorViewModel fornecedorViewModel)
        {
            fornecedorViewModel.Cnpj = fornecedorViewModel.Cnpj.Replace('.', ' ').Replace('/', ' ').Replace('-', ' ');

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;


                if (fornecedorViewModel.Endereco.BairroDescNew != null ||
                    fornecedorViewModel.Endereco.BairroDescNew != "")
                    ModelState.Remove("fornecedorViewModel.Endereco.BairroId");


                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                            .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    if (_fornecedorRep.CnpjExistente(fornecedorViewModel.Cnpj) > 0)
                    {
                        ModelState.AddModelError("CNPJ Existente", "CNPJ:" + fornecedorViewModel.Cnpj + " já existe .");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                            ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                .Select(m => m.ErrorMessage).ToArray());
                    }
                    else
                    {
                        var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                        if (!string.IsNullOrEmpty(fornecedorViewModel.Endereco.BairroDescNew))
                        {
                            var bairroCad = new Bairro
                            {
                                DescBairro = fornecedorViewModel.Endereco.BairroDescNew,
                                CidadeId = fornecedorViewModel.Endereco.CidadeId,
                                DtCriacao = DateTime.Now,
                                UsuarioCriacao = usuario
                            };

                            _bairroRep.Add(bairroCad);
                            _unitOfWork.Commit();

                            fornecedorViewModel.Endereco.BairroId = bairroCad.Id;
                        }


                        var pessoa = new Pessoa
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            TipoPessoa = TipoPessoa.PessoaJuridica,
                            Ativo = fornecedorViewModel.Ativo
                        };


                        var pessoaJuridica = new PessoaJuridica
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Pessoa = pessoa,
                            NomeFantasia = fornecedorViewModel.NomeFantasia,
                            Cnpj = fornecedorViewModel.Cnpj,
                            RazaoSocial = fornecedorViewModel.RazaoSocial,
                            DtFundacao = fornecedorViewModel.DtFundacao,
                            Email = fornecedorViewModel.Email,
                            InscEstadual = fornecedorViewModel.InscEstadual,
                            Ativo = fornecedorViewModel.Ativo
                        };

                        _pessoaJuridicaRep.Add(pessoaJuridica);
                        _unitOfWork.Commit();


                        var enderecoForn = new Endereco
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            PessoaId = fornecedorViewModel.Endereco.PessoaId,
                            EstadoId = fornecedorViewModel.Endereco.EstadoId,
                            CidadeId = fornecedorViewModel.Endereco.CidadeId,
                            BairroId = fornecedorViewModel.Endereco.BairroId,
                            LogradouroId = fornecedorViewModel.Endereco.LogradouroId,
                            Numero = fornecedorViewModel.Endereco.NumEndereco,
                            Complemento = fornecedorViewModel.Endereco.Complemento,
                            DescEndereco = fornecedorViewModel.Endereco.Endereco,
                            Cep = fornecedorViewModel.Endereco.Cep,
                            Ativo = fornecedorViewModel.Ativo,
                            EnderecoPadrao = fornecedorViewModel.Endereco.EnderecoPadrao
                        };

                        enderecoForn.LocalizacaoGoogle();

                        pessoa.PessoaJuridica = pessoaJuridica;
                        pessoa.Enderecos.Add(enderecoForn);
                        _pessoaRep.Edit(pessoa);
                        _unitOfWork.Commit();

                        var novoFornecedor = new Fornecedor
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Pessoa = pessoa,
                            Responsavel = fornecedorViewModel.Responsavel,
                            Descricao = fornecedorViewModel.Descricao,
                            PalavrasChaves = fornecedorViewModel.PalavrasChaves,
                            Observacao = fornecedorViewModel.Observacao,
                            ObservacaoEntrega = fornecedorViewModel.ObservacaoEntrega,
                            Ativo = fornecedorViewModel.Ativo,
                            VlPedidoMin = fornecedorViewModel.VlPedidoMin.TryParseDecimal(),
                            PrimeiraAvista = fornecedorViewModel.PrimeiraAvista,
                            DddTel = fornecedorViewModel.DddTelComl,
                            Telefone = fornecedorViewModel.TelefoneComl,
                            DddCel = fornecedorViewModel.DddCel,
                            Celular = fornecedorViewModel.Celular,
                            Contato = fornecedorViewModel.Contato
                        };
                        _fornecedorRep.Add(novoFornecedor);
                        _unitOfWork.Commit();


                        var fornecedorFormaPagtos =
                            _fornecedorFormaPagtoRep.GetAll().Where(x => x.FornecedorId == novoFornecedor.Id);

                        if (fornecedorFormaPagtos.Any())
                            _fornecedorFormaPagtoRep.DeleteAll(fornecedorFormaPagtos);

                        foreach (var item in fornecedorViewModel.FormaPagtos)
                        {
                            var ffPagto = new FornecedorFormaPagto
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                FornecedorId = novoFornecedor.Id,
                                FormaPagtoId = item.FormaPagtoId,
                                Desconto = item.Desconto,
                                Ativo = fornecedorViewModel.Ativo,
                                ValorPedido = item.VlFormaPagto.TryParseDecimal()
                            };

                            novoFornecedor.FornecedorFormaPagtos.Add(ffPagto);
                        }

                        _fornecedorRep.Edit(novoFornecedor);
                        _unitOfWork.Commit();


                        // Update view model
                        var fonecedorVM = Mapper.Map<Fornecedor, FornecedorViewModel>(novoFornecedor);
                        response = request.CreateResponse(HttpStatusCode.Created, fonecedorVM);
                    }
                }
                return response;
            });
        }

        [HttpPost]
        [Route("inserirregiao")]
        public HttpResponseMessage InserirRegiao(HttpRequestMessage request, dadosInserirRegiao dadosInserirRegiao)
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
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    var _fornecedor = _fornecedorRep.GetSingle(dadosInserirRegiao.fornecedorId);

                    var listaRegioesInserir = dadosInserirRegiao.dadosRegiaoFornecedores.Where(x => x.InserirRegiao).ToList();
                    var listaRegioesDeletar = dadosInserirRegiao.dadosRegiaoFornecedores.Where(x => !x.InserirRegiao).ToList();

                    var listaRegioesPrazoNaoSelecionado = listaRegioesInserir.Where(x => x.TipoPrazo == TipoPrazoEntrega.NaoSelecionado).ToList();
                    var listaRegioesPrazoDias = listaRegioesInserir.Where(x => x.TipoPrazo == TipoPrazoEntrega.Dias).ToList();
                    var listaRegioesPrazoSemana = listaRegioesInserir.Where(x => x.TipoPrazo == TipoPrazoEntrega.DiasSemana).ToList();
                    var inserirPrazoDias = listaRegioesPrazoNaoSelecionado.Concat(listaRegioesPrazoDias).ToList();

                    var listaRegioesDeletarPrazoDias = listaRegioesDeletar.Where(x => x.TipoPrazo == TipoPrazoEntrega.Dias).Select(d => d.CidadeId).ToList();
                    var listaRegioesDeletarPrazoSemana = listaRegioesDeletar.Where(x => x.TipoPrazo == TipoPrazoEntrega.DiasSemana).Select(d => d.CidadeId).ToList();

                    #region Deletar Região de Prazo por Dias

                    if (listaRegioesDeletarPrazoDias.Any())
                    {
                        var fornecedorRegioes =
                            _fornecedorRegiaoRep.GetAll().Where(p => p.FornecedorId == dadosInserirRegiao.fornecedorId &&
                                                                     listaRegioesDeletarPrazoDias.Contains(p.CidadeId));

                        _fornecedorRegiaoRep.DeleteAll(fornecedorRegioes);
                        _unitOfWork.Commit();

                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                    }

                    #endregion

                    #region Deletar Região de Prazo por Semama

                    if (listaRegioesDeletarPrazoSemana.Any())
                    {
                        var fornecedorRegioesPrazoSemanal =
                            _fornecedorPrazoSemanalRep.GetAll().Where(p => p.FornecedorId == dadosInserirRegiao.fornecedorId &&
                                                                           listaRegioesDeletarPrazoSemana.Contains(p.CidadeId));
                        _fornecedorPrazoSemanalRep.DeleteAll(fornecedorRegioesPrazoSemanal);
                        _unitOfWork.Commit();

                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                    }

                    #endregion

                    #region Inserir Região na tabela de Prazo por dias.

                    if (inserirPrazoDias.Any())
                    {
                        var cidades = inserirPrazoDias.Select(x => x.CidadeId).ToList();

                        var fornecedorRegioesPrazoSemanal =
                            _fornecedorPrazoSemanalRep.GetAll()
                                .Where(p => p.FornecedorId == dadosInserirRegiao.fornecedorId &&
                                            cidades.Contains(p.CidadeId));

                        var fornecedorRegioesPrazoDias =
                            _fornecedorRegiaoRep.GetAll()
                                .Where(p => p.FornecedorId == dadosInserirRegiao.fornecedorId &&
                                            cidades.Contains(p.CidadeId));

                        if (fornecedorRegioesPrazoSemanal.Any())
                        {
                            _fornecedorPrazoSemanalRep.DeleteAll(fornecedorRegioesPrazoSemanal);
                        }

                        if (fornecedorRegioesPrazoDias.Any())
                        {
                            foreach (var dias in fornecedorRegioesPrazoDias)
                            {
                                foreach (var lista in inserirPrazoDias)
                                {
                                    if (dias.CidadeId == lista.CidadeId)
                                    {
                                        dias.UsuarioAlteracaoId = usuario.Id;
                                        dias.DtAlteracao = DateTime.Now;
                                        dias.VlPedMinRegiao = lista.VlPedMinRegiao.TryParseDecimal();
                                        dias.TaxaEntrega = lista.TaxaEntrega.TryParseDecimal();
                                        dias.Cif = lista.Cif;
                                        dias.Ativo = true;
                                        _fornecedorRegiaoRep.Edit(dias);
                                        inserirPrazoDias.Remove(lista);
                                        break;
                                    }
                                }
                            }
                            _unitOfWork.Commit();
                        }

                        foreach (var cidade in inserirPrazoDias)
                        {
                            var dadosRegiao =
                                dadosInserirRegiao.dadosRegiaoFornecedores.FirstOrDefault(
                                    x => x.CidadeId == cidade.CidadeId);

                            var fornecedorRegiao = new FornecedorRegiao
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                FornecedorId = dadosInserirRegiao.fornecedorId,
                                CidadeId = cidade.CidadeId,
                                VlPedMinRegiao = cidade.VlPedMinRegiao.TryParseDecimal(),
                                Cif = dadosRegiao.Cif,
                                TaxaEntrega = dadosRegiao.TaxaEntrega.TryParseDecimal(),
                                Prazo = 0,
                                Ativo = true
                            };

                            _fornecedor.FornecedorRegiao.Add(fornecedorRegiao);
                            _fornecedorRep.Edit(_fornecedor);
                        }

                        _unitOfWork.Commit();

                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                    }

                    #endregion

                    #region Inserir Região na tabela de Prazo por Semana

                    if (listaRegioesPrazoSemana.Any())
                    {
                        var cidades = listaRegioesPrazoSemana.Select(x => x.CidadeId).ToList();

                        var fornecedorRegioesPrazoDias =
                            _fornecedorRegiaoRep.GetAll()
                                .Where(p => p.FornecedorId == dadosInserirRegiao.fornecedorId &&
                                           cidades.Contains(p.CidadeId));

                        var fornecedorRegioesPrazoSemanal =
                            _fornecedorPrazoSemanalRep.GetAll()
                                .Where(p => p.FornecedorId == dadosInserirRegiao.fornecedorId &&
                                            cidades.Contains(p.CidadeId));


                        if (fornecedorRegioesPrazoDias.Any())
                        {
                            _fornecedorRegiaoRep.DeleteAll(fornecedorRegioesPrazoDias);
                        }
                        if (fornecedorRegioesPrazoSemanal.Any())
                        {
                            foreach (var semana in fornecedorRegioesPrazoSemanal)
                            {
                                foreach (var lista in listaRegioesPrazoSemana)
                                {
                                    if (semana.CidadeId == lista.CidadeId)
                                    {
                                        semana.UsuarioAlteracaoId = usuario.Id;
                                        semana.DtAlteracao = DateTime.Now;
                                        semana.VlPedMinRegiao = lista.VlPedMinRegiao.TryParseDecimal();
                                        semana.TaxaEntrega = lista.TaxaEntrega.TryParseDecimal();
                                        semana.Cif = lista.Cif;
                                        _fornecedorPrazoSemanalRep.Edit(semana);
                                        listaRegioesPrazoSemana.Remove(lista);
                                    }
                                }
                            }

                            _unitOfWork.Commit();
                        }

                        foreach (var cidade in listaRegioesPrazoSemana)
                        {
                            var fornecedorRegiao = new FornecedorPrazoSemanal
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                FornecedorId = dadosInserirRegiao.fornecedorId,
                                CidadeId = cidade.CidadeId,
                                VlPedMinRegiao = cidade.VlPedMinRegiao.TryParseDecimal(),
                                TaxaEntrega = cidade.TaxaEntrega.TryParseDecimal(),
                                Cif = cidade.Cif,
                                DiaSemana = 1,
                                Ativo = true
                            };

                            _fornecedor.FornecedorRegiaoSemanal.Add(fornecedorRegiao);
                            _fornecedorRep.Edit(_fornecedor);
                        }

                        _unitOfWork.Commit();
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                    }

                    #endregion

                }

                return response;
            });
        }

        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, FornecedorViewModel fornecedorViewModel)
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
                    var _fornecedor = _fornecedorRep.GetSingle(fornecedorViewModel.Id);

                    var _enderecoForn = _enderecoRep.GetSingle(fornecedorViewModel.Endereco.Id);

                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    _fornecedor.AtualizarForecedor(fornecedorViewModel, usuario);


                    //como fornecedor só tem um endereço eu seto aqui de acordo com ativo do cadastro do fornecedor
                    fornecedorViewModel.Endereco.Ativo = fornecedorViewModel.Ativo;

                    _enderecoForn.AtualizarEndereco(fornecedorViewModel.Endereco, usuario);


                    //if (fornecedorViewModel.TelefoneId == 0)
                    //{

                    //    Telefone telefone = new Telefone()
                    //    {
                    //        UsuarioCriacao = usuario,
                    //        DtCriacao = DateTime.Now,
                    //        DddTelComl = fornecedorViewModel.DddTelComl,
                    //        TelefoneComl = fornecedorViewModel.TelefoneComl,
                    //        DddCel = fornecedorViewModel.DddCel,
                    //        Celular = fornecedorViewModel.Celular,
                    //        Ativo = fornecedorViewModel.Ativo,
                    //        Contato = fornecedorViewModel.Contato

                    //    };

                    //    _fornecedor.Pessoa.Telefones.Add(telefone);
                    //}
                    //else
                    //{

                    //    Telefone _telefone = _telefoneRep.GetSingle(fornecedorViewModel.TelefoneId);

                    //    _telefone.AtualizarTelefoneFornecedor(fornecedorViewModel, usuario);
                    //}


                    #region Criando a desativação ou inserção das formas de pagamento

                    var fornecedorFormaPagtos =
                        _fornecedorFormaPagtoRep.GetAll().Where(x => x.FornecedorId == fornecedorViewModel.Id);

                    var idsFormaPagtosCadastradosAtivos = fornecedorFormaPagtos.Where(a => a.Ativo)
                        .Select(x => x.FormaPagtoId).ToList();
                    var idsFormaPagtosCadastradosDesativados = fornecedorFormaPagtos.Where(a => !a.Ativo)
                        .Select(x => x.FormaPagtoId).ToList();
                    var idsTotalCadastrados =
                        idsFormaPagtosCadastradosAtivos.Concat(idsFormaPagtosCadastradosDesativados);
                    var idsFormaPagtoRecebidosPlataforma = fornecedorViewModel.FormaPagtos.Select(x => x.FormaPagtoId)
                        .ToList();
                    var idsFormaPagtoNaoCadastrados = idsFormaPagtoRecebidosPlataforma
                        .Where(x => !idsTotalCadastrados.Contains(x)).ToList();
                    var idsFormaPagtoDesativando = idsFormaPagtosCadastradosAtivos
                        .Except(idsFormaPagtoRecebidosPlataforma).ToList();
                    var idsFormaPagtoAtivando = idsFormaPagtosCadastradosDesativados
                        .Where(x => idsFormaPagtoRecebidosPlataforma.Contains(x)).ToList();

                    //Caso a forma de pagamento não exista inseri
                    if (idsFormaPagtoNaoCadastrados.Count > 0)
                        foreach (var item in fornecedorViewModel.FormaPagtos)
                            foreach (var formaRecebPlataforma in idsFormaPagtoNaoCadastrados)
                                if (item.FormaPagtoId == formaRecebPlataforma)
                                {
                                    var ffPagto = new FornecedorFormaPagto
                                    {
                                        UsuarioCriacao = usuario,
                                        DtCriacao = DateTime.Now,
                                        FornecedorId = fornecedorViewModel.Id,
                                        FormaPagtoId = item.FormaPagtoId,
                                        Desconto = item.Desconto,
                                        ValorPedido = item.VlFormaPagto.TryParseDecimal(),
                                        ValorMinParcela = item.ValorMinParcela.TryParseDecimal(),
                                        ValorMinParcelaPedido = item.ValorMinParcelaPedido.TryParseDecimal(),
                                        Ativo = true
                                    };

                                    _fornecedor.FornecedorFormaPagtos.Add(ffPagto);
                                }

                    //Caso a forma de pagamento já exista esteja ativo, desativa o mesmo
                    if (idsFormaPagtoDesativando.Count > 0)
                        foreach (var formaPagto in idsFormaPagtoDesativando)
                        {
                            var pagto = fornecedorFormaPagtos.FirstOrDefault(x => x.FormaPagtoId == formaPagto);
                            pagto.Ativo = false;
                            pagto.DtAlteracao = DateTime.Now;
                            pagto.UsuarioAlteracao = usuario;

                            _fornecedorFormaPagtoRep.Edit(pagto);
                        }

                    //Caso a forma de pagamento já exista esteja desativado, ativa o mesmo
                    if (idsFormaPagtoAtivando.Count > 0)
                        foreach (var item in fornecedorViewModel.FormaPagtos)
                            foreach (var formaPagto in idsFormaPagtoAtivando)
                                if (item.FormaPagtoId == formaPagto)
                                {
                                    var pagto = fornecedorFormaPagtos.FirstOrDefault(x => x.FormaPagtoId == formaPagto);
                                    pagto.Ativo = true;
                                    pagto.DtAlteracao = DateTime.Now;
                                    pagto.UsuarioAlteracao = usuario;
                                    pagto.ValorPedido = item.VlFormaPagto.TryParseDecimal();
                                    pagto.ValorMinParcela = item.ValorMinParcela.TryParseDecimal();
                                    pagto.ValorMinParcelaPedido = item.ValorMinParcelaPedido.TryParseDecimal();
                                    _fornecedorFormaPagtoRep.Edit(pagto);
                                }

                    foreach (var formaPagto in fornecedorViewModel.FormaPagtos)
                    {
                        var pagto = fornecedorFormaPagtos.FirstOrDefault(f => f.FormaPagtoId == formaPagto.FormaPagtoId);
                        bool alterou = false;

                        if (pagto != null)
                        {
                            if (pagto.ValorPedido != formaPagto.VlFormaPagto.TryParseDecimal() && formaPagto.VlFormaPagto != null)
                            {
                                pagto.ValorPedido = formaPagto.VlFormaPagto.TryParseDecimal();
                                alterou = true;
                            }

                            if (pagto.ValorMinParcela != formaPagto.ValorMinParcela.TryParseDecimal() && formaPagto.ValorMinParcela != null)
                            {
                                pagto.ValorMinParcela = formaPagto.ValorMinParcela.TryParseDecimal();
                                alterou = true;
                            }

                            if (pagto.ValorMinParcelaPedido != formaPagto.ValorMinParcelaPedido.TryParseDecimal() && formaPagto.ValorMinParcelaPedido != null)
                            {
                                pagto.ValorMinParcelaPedido = formaPagto.ValorMinParcelaPedido.TryParseDecimal();
                                alterou = true;
                            }
                        }

                        if (alterou)
                            _fornecedorFormaPagtoRep.Edit(pagto);
                    }

                    #endregion

                    _fornecedor.Pessoa.Enderecos.ForEach(x => { x.LocalizacaoGoogle(); });

                    _fornecedorRep.Edit(_fornecedor);

                    _unitOfWork.Commit();


                    // Update view model
                    fornecedorViewModel = Mapper.Map<Fornecedor, FornecedorViewModel>(_fornecedor);
                    response = request.CreateResponse(HttpStatusCode.OK, fornecedorViewModel);
                }

                return response;
            });
        }

        [HttpGet]
        [Route("fornecedorCategorias")]
        public HttpResponseMessage GetMembroCategoria(HttpRequestMessage request, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var fornecedorCategorias = _fornecedorCategoriaRep.GetAll().Where(x => x.Fornecedor.Id == fornecedorId)
                    .ToList();


                var fornCategoriasVM =
                    Mapper.Map<IEnumerable<FornecedorCategoria>, IEnumerable<CategoriaViewModel>>(fornecedorCategorias);

                response = request.CreateResponse(HttpStatusCode.OK, fornCategoriasVM);

                return response;
            });
        }

        [HttpPost]
        [Route("inserirFornecedorCategoria/{fornecedorId:int}")]
        public HttpResponseMessage InserirFornecedorCategoria(HttpRequestMessage request, int fornecedorId, int[] listCategoria)
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
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    var _forencedor = _fornecedorRep.GetSingle(fornecedorId);


                    var fornecedorCategorias =
                        _fornecedorCategoriaRep.GetAll().Where(x => x.FornecedorId == fornecedorId);

                    if (fornecedorCategorias.Any())
                        _fornecedorCategoriaRep.DeleteAll(fornecedorCategorias);


                    foreach (var item in listCategoria)
                    {
                        var fornecedorCategoria = new FornecedorCategoria
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            FornecedorId = fornecedorId,
                            CategoriaId = item,
                            Ativo = true
                        };

                        _forencedor.FornecedorCategorias.Add(fornecedorCategoria);
                    }

                    _fornecedorRep.Edit(_forencedor);
                    _unitOfWork.Commit();
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                }

                return response;
            });
        }

        [HttpGet]
        [Route("fornecedorregioes")]
        public HttpResponseMessage GetFornecedorRegioes(HttpRequestMessage request, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var fornecedorRegCorrido = _fornecedorRegiaoRep.GetAll()
                .Where(x => x.Fornecedor.Id == fornecedorId).ToList();

                var groupFornecedores = fornecedorRegCorrido
                    .GroupBy(p => new { p.CidadeId, p.VlPedMinRegiao, p.TaxaEntrega, p.Cif })
                    .Select(s => new dadosFornecedorRegiao
                    {
                        CidadeId = s.Key.CidadeId,
                        VlPedMinRegiao = $"{s.Key.VlPedMinRegiao.Value:C2}",
                        TipoPrazo = TipoPrazoEntrega.Dias,
                        TaxaEntrega = $"{s.Key.TaxaEntrega.Value:C2}",
                        Cif = s.Key.Cif.Value
                    });


                var fornecedorRegPrazoSemanal = _fornecedorPrazoSemanalRep.GetAll()
                   .Where(x => x.Fornecedor.Id == fornecedorId)
                   .Distinct().ToList();

                var groupPrazoSemanal = fornecedorRegPrazoSemanal
                .GroupBy(p => new { p.CidadeId, p.VlPedMinRegiao, p.TaxaEntrega, p.Cif })
                    .Select(s => new dadosFornecedorRegiao
                    {
                        CidadeId = s.Key.CidadeId,
                        VlPedMinRegiao = $"{s.Key.VlPedMinRegiao.Value:C2}",
                        TipoPrazo = TipoPrazoEntrega.DiasSemana,
                        TaxaEntrega = $"{s.Key.TaxaEntrega.Value:C2}",
                        Cif = s.Key.Cif.Value
                    });

                var fornecedoresRegiao = groupFornecedores.Concat(groupPrazoSemanal);

                response = request.CreateResponse(HttpStatusCode.OK, new { fornecedoresRegiao });

                return response;
            });
        }

        [HttpPost]
        [Route("atualizaprazo")]
        public HttpResponseMessage AtualizaPrazo(HttpRequestMessage request, DadosPrazoRegiao prazoRegiao)
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
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var fornecedorRegioes = new List<FornecedorRegiao>();
                    var listaCidadesAlteradas = new List<FornecedorRegiao>();


                    if (prazoRegiao.regiaoId == 0)
                        fornecedorRegioes =
                            _fornecedorRegiaoRep.GetAll().Where(x => x.FornecedorId == prazoRegiao.fornecedorId &&
                                                                     x.CidadeId == prazoRegiao.cidadeId).ToList();
                    else
                        fornecedorRegioes =
                            _fornecedorRegiaoRep.GetAll().Where(x => x.FornecedorId == prazoRegiao.fornecedorId &&
                                                                     x.Cidade.RegiaoId == prazoRegiao.regiaoId &&
                                                                     x.CidadeId == prazoRegiao.cidadeId).ToList();

                    if (fornecedorRegioes.Count > 0)
                    {
                        foreach (var item in fornecedorRegioes)
                        {
                            item.UsuarioAlteracao = usuario;
                            item.DtAlteracao = DateTime.Now;
                            item.Prazo = prazoRegiao.prazo;
                            listaCidadesAlteradas.Add(item);
                        }

                        _unitOfWork.Commit();

                        var fornecedorRegioesNovos =
                            _fornecedorRegiaoRep.GetAll().Where(x => x.FornecedorId == prazoRegiao.fornecedorId);

                        var regiaoPrazoFornecedorVM =
                            Mapper.Map<IEnumerable<FornecedorRegiao>, IEnumerable<FornecedorRegiaoViewModel>>(
                                fornecedorRegioesNovos);

                        response = request.CreateResponse(HttpStatusCode.OK, regiaoPrazoFornecedorVM);
                    }
                    else
                    {
                        var fornecedorPrazoSemanal =
                            _fornecedorPrazoSemanalRep.GetAll().Where(x => x.FornecedorId == prazoRegiao.fornecedorId &&
                                                                           x.CidadeId == prazoRegiao.cidadeId).ToList();

                        if (fornecedorPrazoSemanal.Any())
                        {
                            _fornecedorPrazoSemanalRep.DeleteAll(fornecedorPrazoSemanal);
                            _unitOfWork.Commit();
                        }

                        var taxaCif = fornecedorPrazoSemanal.FirstOrDefault(x => x.VlPedMinRegiao > 0);

                        var fornReg = new FornecedorRegiao
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            FornecedorId = prazoRegiao.fornecedorId,
                            CidadeId = prazoRegiao.cidadeId,
                            Prazo = prazoRegiao.prazo,
                            VlPedMinRegiao = taxaCif?.VlPedMinRegiao,
                            TaxaEntrega = taxaCif?.TaxaEntrega.Value,
                            Cif = taxaCif?.Cif,
                            Ativo = true
                        };

                        _fornecedorRegiaoRep.Add(fornReg);
                        _unitOfWork.Commit();

                        var fornecedorRegioesNovos =
                            _fornecedorRegiaoRep.GetAll().Where(x => x.FornecedorId == prazoRegiao.fornecedorId);

                        var regiaoPrazoFornecedorVM =
                            Mapper.Map<IEnumerable<FornecedorRegiao>, IEnumerable<FornecedorRegiaoViewModel>>(
                                fornecedorRegioesNovos);

                        response = request.CreateResponse(HttpStatusCode.OK, regiaoPrazoFornecedorVM);
                    }
                }

                return response;
            });
        }

        [HttpPost]
        [Route("inserirprazosemana")]
        public HttpResponseMessage Inserirprazosemana(HttpRequestMessage request, List<FornecedorPrazoSemanalViewModel> listaPrazoSemanal)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var taxaCifObject = new dadosFornecedorRegiao();

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                            .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var linha = listaPrazoSemanal[0];

                    var fornecedorPrazoSemanal =
                        _fornecedorPrazoSemanalRep.GetAll().Where(x => x.FornecedorId == linha.FornecedorId &&
                                                                       x.CidadeId == linha.CidadeId).ToList();

                    var fornecedorDiasCorridos = _fornecedorRegiaoRep.FirstOrDefault(x =>
                        x.FornecedorId == linha.FornecedorId && x.CidadeId == linha.CidadeId);

                    if (fornecedorPrazoSemanal.Count > 0)
                    {
                        _fornecedorPrazoSemanalRep.DeleteAll(fornecedorPrazoSemanal);

                        taxaCifObject.VlPedMinRegiao = fornecedorPrazoSemanal.First().VlPedMinRegiao.ToString();
                        taxaCifObject.TaxaEntrega = fornecedorPrazoSemanal.First().TaxaEntrega.ToString();
                        taxaCifObject.Cif = fornecedorPrazoSemanal.First().Cif.Value;
                    }

                    if (fornecedorDiasCorridos != null)
                    {
                        _fornecedorRegiaoRep.Delete(fornecedorDiasCorridos);

                        taxaCifObject.VlPedMinRegiao = fornecedorDiasCorridos.VlPedMinRegiao.ToString();
                        taxaCifObject.TaxaEntrega = fornecedorDiasCorridos.TaxaEntrega.ToString();
                        taxaCifObject.Cif = fornecedorDiasCorridos.Cif.Value;

                        _unitOfWork.Commit();
                    }

                    for (var i = 0; i < listaPrazoSemanal.Count; i++)
                    {
                        var prazoSemanal = new FornecedorPrazoSemanal
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            FornecedorId = listaPrazoSemanal[i].FornecedorId,
                            CidadeId = listaPrazoSemanal[i].CidadeId,
                            DiaSemana = listaPrazoSemanal[i].DiaSemana,
                            VlPedMinRegiao = taxaCifObject.VlPedMinRegiao.TryParseDecimal(),
                            TaxaEntrega = taxaCifObject.TaxaEntrega.TryParseDecimal(),
                            Cif = taxaCifObject.Cif,
                            Ativo = true
                        };

                        _fornecedorPrazoSemanalRep.Add(prazoSemanal);
                        _unitOfWork.Commit();
                    }

                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

        [HttpGet]
        [Route("qtdmembrosolicitafornecedor")]
        public HttpResponseMessage GetQtdMembroSolicitaFornecedor(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var idFornecedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.Pessoa.Id).Select(u => u.Id)
                    .FirstOrDefault();

                var membroSolicitaFornecedor = _membroFornecedor.GetAll()
                    .Where(x => x.Fornecedor.Id == idFornecedor && !x.Ativo)
                    .ToList()
                    .Count;

                response = request.CreateResponse(HttpStatusCode.OK, membroSolicitaFornecedor);

                return response;
            });
        }

        [HttpGet]
        [Route("pesquisarmembrofornecedor/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetMembroSolicitaFornecedor(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            var currentPage = page.Value;
            var currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var membro = new List<Membro>();
                var totalMembros = new int();


                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


                var idFornecedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.PessoaId).Select(f => f.Id)
                    .FirstOrDefault();


                var membroSolicitaFornecedor =
                    _membroFornecedor.GetAll().Where(x => !x.Ativo && x.FornecedorId == idFornecedor && x.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica).ToList();


                if (membroSolicitaFornecedor.Count > 0)
                    foreach (var mb in membroSolicitaFornecedor)
                    {
                        var member = _membro.FindBy(c => c.Id == mb.MembroId && c.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                            .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                            .Skip(currentPage * currentPageSize)
                            .Take(currentPageSize)
                            .FirstOrDefault();

                        membro.Add(member);
                    }

                var membrosVM = Mapper.Map<IEnumerable<Membro>, IEnumerable<MembroViewModel>>(membro);

                var pagSet = new PaginacaoConfig<MembroViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalMembros,
                    TotalPages = (int)Math.Ceiling((decimal)totalMembros / currentPageSize),
                    Items = membrosVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpGet]
        [Route("pesquisarmembrofornecedorPF/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetMembroSolicitaFornecedorPF(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            var currentPage = page.Value;
            var currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var membro = new List<Membro>();
                var totalMembros = new int();


                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


                var idFornecedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.PessoaId).Select(f => f.Id)
                    .FirstOrDefault();


                var membroSolicitaFornecedor =
                    _membroFornecedor.GetAll().Where(x => !x.Ativo && x.FornecedorId == idFornecedor && x.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica).ToList();


                if (membroSolicitaFornecedor.Count > 0)
                    foreach (var mb in membroSolicitaFornecedor)
                    {
                        var member = _membro.FindBy(c => c.Id == mb.MembroId && c.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica)
                            .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                            .Skip(currentPage * currentPageSize)
                            .Take(currentPageSize)
                            .FirstOrDefault();

                        membro.Add(member);
                    }

                var membrosVM = Mapper.Map<IEnumerable<Membro>, IEnumerable<MembroPFViewModel>>(membro);

                var pagSet = new PaginacaoConfig<MembroPFViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalMembros,
                    TotalPages = (int)Math.Ceiling((decimal)totalMembros / currentPageSize),
                    Items = membrosVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("fornecedorAceitaMembro/{idMembro:int}")]
        public HttpResponseMessage FornecedorAceitaMembro(HttpRequestMessage request, int idMembro, string[] listFormaPagtoRemoveMembro)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var corpoEmailNomeFornecedor = string.Empty;
                var notificationAlertaService = new NotificacoesAlertasService();


                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                            .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    var membro = _membro.FirstOrDefault(m => m.Id == idMembro);
                    var idFornecedor = _fornecedorRep.FirstOrDefault(f => f.PessoaId == usuario.PessoaId).Id;


                    var soliSelect = _solicitacaoMembroFornecedor.GetAll()
                        .Where(s => s.FornecedorId == idFornecedor && s.MembroId == idMembro)
                        .GroupBy(x => x.FornecedorId)
                        .Select(f => new
                        {
                            FornecedorID = f.Key,
                            Solicitacao = f.FirstOrDefault(p => p.DtCriacao == f.Select(d => d.DtCriacao).Max())
                        });

                    var solicitacao = soliSelect.Select(x => x.Solicitacao).FirstOrDefault();

                    var membroFornecedor =
                        _membroFornecedor.FirstOrDefault(m => m.MembroId == idMembro && m.FornecedorId == idFornecedor);

                    if (membroFornecedor != null)
                    {
                        var mfVM = new MembroFornecedorViewModel
                        {
                            MembroId = idMembro,
                            FornecedorId = idFornecedor,
                            Ativo = true
                        };

                        var memFornView = new SolicitacaoMembroFornecedorViewModel
                        {
                            MembroId = idMembro,
                            FornecedorId = idFornecedor,
                            Observacao = "Aceito",
                            Ativo = true
                        };

                        //Atualiza tabela de membro fornecedor
                        membroFornecedor.AtualizarMembroFornecedor(mfVM, usuario);


                        //Atualiza tabela de solicitação membro fornecedor
                        solicitacao.AtualizarSolicitacaoMembroFornecedor(memFornView, usuario);


                        //Enviar email para o membro que o mesmo foi aceito pelo fornecedor
                        var corpoEmail = _templateEmail.FindBy(e => e.Id == 12).Select(e => e.Template)
                            .FirstOrDefault();
                        var usuarioMembro = membroFornecedor.UsuarioCriacao;

                        if (corpoEmail != null)
                        {


                            if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                            {
                                corpoEmailNomeFornecedor = corpoEmail.Replace("#NomeMembro#", membroFornecedor.Membro.Pessoa.PessoaJuridica.NomeFantasia).Replace("#NomeFornecedor#", membroFornecedor.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia);
                            }
                            else
                            {

                                corpoEmailNomeFornecedor = corpoEmail.Replace("#NomeMembro#", membroFornecedor.Membro.Pessoa.PessoaFisica.Nome).Replace("#NomeFornecedor#", membroFornecedor.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia);

                            }
                            var emails = new Emails
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                AssuntoEmail = "Novo Fornecedor. O fornecedor acabou de responder sua solicitação para começar a comprar com ele",
                                EmailDestinatario = usuarioMembro.UsuarioEmail,
                                CorpoEmail = corpoEmailNomeFornecedor.Trim(),
                                Status = Status.NaoEnviado,
                                Origem = Origem.MembroSolicitaFornecedor,
                                Ativo = true
                            };

                            var sms = new Sms
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                Numero = usuarioMembro.Telefones.Select(t => t.DddCel).FirstOrDefault() + usuarioMembro
                                             .Telefones.Select(t => t.Celular).FirstOrDefault(),
                                Mensagem =
                                    "Economiza Já - Novo Fornecedor. O fornecedor acabou de responder sua solicitação para começar a comprar com ele.",
                                Status = StatusSms.NaoEnviado,
                                OrigemSms = TipoOrigemSms.FornecedorAceitaMembro,
                                Ativo = true
                            };

                            #region Verifica se Membro quer receber EMAIL E SMS

                            if (notificationAlertaService.PodeEnviarNotificacao(usuarioMembro.Id,
                                TipoAviso.FornecedorAceitaTrabalharMembro.TryParseInt(), TipoAlerta.EMAIL))
                                _emailsNotificaFornecedorMembro.Add(emails);

                            if (notificationAlertaService.PodeEnviarNotificacao(usuarioMembro.Id,
                                TipoAviso.FornecedorAceitaTrabalharMembro.TryParseInt(), TipoAlerta.SMS))
                                _sms.Add(sms);

                            #endregion
                        }

                        var lUsu = _usuarioRep.FindBy(u => u.PessoaId == membroFornecedor.Fornecedor.PessoaId).ToList();
                        var delAviso = new Avisos();
                        foreach (var usu in lUsu)
                        {
                            delAviso = _avisosRep.GetAll().FirstOrDefault(ar => ar.UsuarioNotificadoId == usu.Id
                                                                                && ar.IdReferencia == membroFornecedor
                                                                                    .Membro.Id
                                                                                && ar.TipoAvisosId == (int)TipoAviso
                                                                                    .PendentedeAceiteFornecedorMembro);
                            if (delAviso != null)
                                _avisosRep.Delete(delAviso);
                        }



                        foreach (var item in listFormaPagtoRemoveMembro)
                        {
                           var fornecedorFormaPagtoMembro = new FornecedorFormaPagtoMembro
                            {
                                FornecedorFormaPagtoId = item.TryParseInt(),
                                FornecedorId= idFornecedor,
                                MembroId = idMembro,
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,                       
                                Ativo = true
                            };
                            _fornecedorFormaPagtoMembro.Add(fornecedorFormaPagtoMembro);
                        }
                        _unitOfWork.Commit();

                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true, tipoPessoa = usuario.Pessoa.TipoPessoa });

                        new NotificacoesHub(_unitOfWork)
                        .NotificaFornecedorAceitouMembro(usuario.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? 
                                                         usuario.Pessoa.PessoaJuridica.NomeFantasia :
                                                         usuario.Pessoa.PessoaFisica.Nome, membro.Pessoa.Usuarios);
                    }
                }

                return response;
            });
        }

        [HttpPost]
        [Route("fornecedorRecusaMembro")]
        public HttpResponseMessage SolicitacaoMembroFornecedor(HttpRequestMessage request, SolicitacaoMembroFornecedorViewModel fornecedoRecusaMembroViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var corpoEmailFornecedorMembro = string.Empty;
                var membro = new Membro();
                var membroFornecedor = new MembroFornecedor();

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                            .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var idFornecedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.PessoaId).Select(f => f.Id)
                        .FirstOrDefault();

                    membro =
                        _membro.FindBy(m => m.Id == fornecedoRecusaMembroViewModel.MembroId).FirstOrDefault();

                    membroFornecedor =
                        _membroFornecedor.FindBy(m => m.MembroId == fornecedoRecusaMembroViewModel.MembroId &&
                                                      m.FornecedorId == idFornecedor).FirstOrDefault();

                    var solicitaMembroFornecedor =
                        _solicitacaoMembroFornecedor.FindBy(
                                s => s.MembroId == fornecedoRecusaMembroViewModel.MembroId &&
                                     s.FornecedorId == idFornecedor)
                            .FirstOrDefault();


                    _membroFornecedor.Delete(membroFornecedor);
                    _unitOfWork.Commit();


                    //Inserir motivo do fornecedor por não aceitar membro
                    var frm = new SolicitacaoMembroFornecedorViewModel
                    {
                        Observacao = fornecedoRecusaMembroViewModel.Observacao,
                        MembroId = fornecedoRecusaMembroViewModel.MembroId,
                        FornecedorId = idFornecedor,
                        Ativo = false
                    };

                    solicitaMembroFornecedor.AtualizarSolicitacaoMembroFornecedor(frm, usuario);


                    //Enviar email para o membro que o mesmo foi aceito pelo fornecedor
                    var corpoEmail = _templateEmail.FindBy(e => e.Id == 14).Select(e => e.Template).FirstOrDefault();

                    var motivoRecusa = fornecedoRecusaMembroViewModel.Observacao;

                    if (!string.IsNullOrEmpty(corpoEmail) && !string.IsNullOrEmpty(motivoRecusa))
                    {
                        corpoEmailFornecedorMembro = corpoEmail
                            .Replace("#NomeMembro#", membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? membro.Pessoa.PessoaJuridica.NomeFantasia : membro.Pessoa.PessoaFisica.Nome)
                            .Replace("#NomeFornecedor#", usuario.Pessoa.PessoaJuridica.NomeFantasia)
                            .Replace("#MotivoRecusa#", motivoRecusa);


                        //Enviar email para o membro que o mesmo foi rejeitado pelo fornecedor
                        var emails = new Emails
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Novo Fornecedor. O fornecedor acabou de responder sua solicitação para começar a comprar com ele",
                            EmailDestinatario = membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? membro.Pessoa.PessoaJuridica.Email : membro.Pessoa.PessoaFisica.Email,
                            CorpoEmail = corpoEmailFornecedorMembro.Trim(),
                            Status = Status.NaoEnviado,
                            Origem = Origem.FornecedorRecusaMembro
                        };


                        var sms = new Sms
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Numero = membro.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() +
                                     membro.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                            Mensagem = "Economiza Já -  O fornecedor acabou de responder sua solicitação para começar a comprar com ele.",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.FornecedorRecusaMembro,
                            Ativo = true
                        };

                        _emailsNotificaFornecedorMembro.Add(emails);
                        _sms.Add(sms);
                    }


                    var lUsu = _usuarioRep.FindBy(u => u.PessoaId == solicitaMembroFornecedor.FornecedorId).ToList();
                    var delAviso = new Avisos();
                    foreach (var usu in lUsu)
                    {
                        delAviso = _avisosRep.GetAll()
                        .FirstOrDefault(ar => ar.UsuarioNotificadoId == usu.Id
                                               && ar.IdReferencia == solicitaMembroFornecedor.MembroId
                                              && ar.TipoAvisosId == (int)TipoAviso.PendentedeAceiteFornecedorMembro);

                        _avisosRep.Delete(delAviso);
                    }


                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                }

                return response;
            });
        }

        [HttpGet]
        [Route("carregafornecedor")]
        public HttpResponseMessage CarregaFornecedor(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var _forencedor = _fornecedorRep.FindBy(f => f.PessoaId == usuario.PessoaId).FirstOrDefault();

                var fornecedorVm = _forencedor.FornecedorRegiao.Select(x => x.Prazo).ToList().Max();


                response = request.CreateResponse(HttpStatusCode.OK, fornecedorVm);

                return response;
            });
        }

        [HttpGet]
        [Route("getfornecedores")]
        public HttpResponseMessage GetFornecedores(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var forencedores = _fornecedorRep.GetAll();
                var forencedoresVM = Mapper.Map<IEnumerable<Fornecedor>, IEnumerable<FornecedorViewModel>>(forencedores);
                response = request.CreateResponse(HttpStatusCode.OK, forencedoresVM);

                return response;
            });
        }

        public class dadosInserirRegiao
        {
            public int fornecedorId { get; set; }
            public int regiaoId { get; set; }
            public List<dadosFornecedorRegiao> dadosRegiaoFornecedores { get; set; }
        }

        public class dadosFornecedorRegiao
        {
            public int CidadeId { get; set; }
            public string VlPedMinRegiao { get; set; }
            public TipoPrazoEntrega TipoPrazo { get; set; }
            public bool InserirRegiao { get; set; }
            public string TaxaEntrega { get; set; }
            public bool Cif { get; set; }
        }
    }
}