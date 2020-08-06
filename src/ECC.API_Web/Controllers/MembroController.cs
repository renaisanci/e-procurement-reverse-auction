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
using ECC.Dados.Extensions;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using ECC.API_Web.Hubs;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.EntidadeEmail;
using ECC.EntidadeEndereco;
using ECC.EntidadeFormaPagto;
using ECC.EntidadePedido;
using ECC.EntidadeProduto;
using ECC.EntidadeSms;
using ECC.Servicos.Abstrato;
using WebGrease.Css.Extensions;
using ECC.EntidadeAvisos;
using ECC.Entidades.EntidadePessoa;
using ECC.Servicos;
using ECC.EntidadeRecebimento;
using ECC.Entidades.EntidadeRecebimento;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [System.Web.Http.Authorize(Roles = "Admin, Membro")]
    [RoutePrefix("api/membro")]
    public class MembroController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Pessoa> _pessoaRep;
        private readonly IEntidadeBaseRep<PessoaJuridica> _pessoaJuridicaRep;
        private readonly IEntidadeBaseRep<PessoaFisica> _pessoaFisicaRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Telefone> _telefoneRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membeoCategoriaRep;
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedorRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<Regiao> _regiaoMembroRep;
        private readonly ISmsService _smsService;
        private readonly IUtilService _utilService;
        private readonly IEntidadeBaseRep<FornecedorRegiao> _fornecedorRegiaoRep;
        private readonly IEntidadeBaseRep<FornecedorFormaPagto> _fornecedorFormaPagtRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmail;
        private readonly IEntidadeBaseRep<Emails> _emailsNotificaFornecedorMembro;
        private readonly IEntidadeBaseRep<SolicitacaoMembroFornecedor> _solicitacaoMembroFornecedor;
        private readonly IEntidadeBaseRep<FornecedorCategoria> _fornecedorCategoria;
        private readonly IEntidadeBaseRep<FornecedorPrazoSemanal> _fornecedorPrazoSemanalRep;
        private readonly IEntidadeBaseRep<Sms> _sms;
        private readonly IEntidadeBaseRep<AvaliacaoFornecedor> _avaliacaofornecedorRep;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;


        public MembroController(
            IUtilService utilService,
            IEntidadeBaseRep<TemplateEmail> templateEmailRep,
            IEmailService emailService,
            ISmsService smsService,
            IEntidadeBaseRep<TemplateSms> templateSmsRep,
            IEntidadeBaseRep<Sms> smsRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<MembroFornecedor> membroFornecedorRep,
            IEntidadeBaseRep<FornecedorRegiao> fornecedorRegiaoRep,
            IEntidadeBaseRep<FornecedorFormaPagto> fornecedorFormaPagtRep,
            IEntidadeBaseRep<MembroCategoria> membeoCategoriaRep,
            IEntidadeBaseRep<Telefone> telefoneRep,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<Pessoa> pessoaRep,
            IEntidadeBaseRep<PessoaJuridica> pessoaJuridicaRep,
            IEntidadeBaseRep<PessoaFisica> pessoaFisicaRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<Regiao> regiaoMembroRep,
            IEntidadeBaseRep<TemplateEmail> templateEmail,
            IEntidadeBaseRep<Emails> emailsNotificaFornecedorMembro,
            IEntidadeBaseRep<SolicitacaoMembroFornecedor> solicitacaoMembroFornecedor,
            IEntidadeBaseRep<FornecedorCategoria> fornecedorCategoria,
            IEntidadeBaseRep<AvaliacaoFornecedor> avaliacaofornecedor,
            IEntidadeBaseRep<Avisos> avisosRep,
            IEntidadeBaseRep<FornecedorPrazoSemanal> fornecedorPrazoSemanalRep,


        IEntidadeBaseRep<Erro> errosRepository, IUnitOfWork unitOfWork)
            : base(usuarioRep, errosRepository, unitOfWork)
        {
            _pessoaRep = pessoaRep;
            _pessoaJuridicaRep = pessoaJuridicaRep;
            _pessoaFisicaRep = pessoaFisicaRep;
            _membroRep = membroRep;
            _usuarioRep = usuarioRep;
            _telefoneRep = telefoneRep;
            _membeoCategoriaRep = membeoCategoriaRep;
            _membroFornecedorRep = membroFornecedorRep;
            _fornecedorRep = fornecedorRep;
            _fornecedorRegiaoRep = fornecedorRegiaoRep;
            _fornecedorFormaPagtRep = fornecedorFormaPagtRep;
            _regiaoMembroRep = regiaoMembroRep;
            _smsService = smsService;
            _utilService = utilService;
            _templateEmail = templateEmail;
            _emailsNotificaFornecedorMembro = emailsNotificaFornecedorMembro;
            _solicitacaoMembroFornecedor = solicitacaoMembroFornecedor;
            _fornecedorCategoria = fornecedorCategoria;
            _sms = smsRep;
            _avaliacaofornecedorRep = avaliacaofornecedor;
            _avisosRep = avisosRep;
            _fornecedorPrazoSemanalRep = fornecedorPrazoSemanalRep;

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
                List<Membro> membros = null;
                int totalMembros = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    membros = _membroRep.FindBy(c => c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter) ||
                            c.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                            c.Pessoa.PessoaJuridica.Cnpj.ToLower().Contains(filter)).Where(x => x.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                        .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalMembros = _membroRep
                        .GetAll().Where(x => x.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                        .Count(c => c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter) ||
                            c.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                            c.Pessoa.PessoaJuridica.Cnpj.ToLower().Contains(filter));
                }
                else
                {
                    membros = _membroRep.GetAll().Where(x => x.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                        .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalMembros = _membroRep.GetAll().Where(x => x.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica).Count();
                }
                membros.ForEach(x =>
                {
                    x.Pessoa.Enderecos.ForEach(y =>
                    {
                        var loc = y.Localizacao;
                    });
                });

                IEnumerable<MembroViewModel> memmbrosVM = Mapper.Map<IEnumerable<Membro>, IEnumerable<MembroViewModel>>(membros);

                PaginacaoConfig<MembroViewModel> pagSet = new PaginacaoConfig<MembroViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMembros,
                    TotalPages = (int)Math.Ceiling((decimal)totalMembros / currentPageSize),
                    Items = memmbrosVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }


        [HttpGet]
        [Route("pesquisarPf/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetPf(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Membro> membros = null;
                int totalMembros = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    membros = _membroRep.FindBy(c => c.Pessoa.PessoaFisica.Nome.ToLower().Contains(filter) ||

                            c.Pessoa.PessoaFisica.Cpf.ToLower().Contains(filter)).Where(x => x.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica)
                        .OrderBy(c => c.Pessoa.PessoaFisica.Nome)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalMembros = _membroRep
                        .GetAll().Where(x => x.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica)
                        .Count(c => c.Pessoa.PessoaFisica.Nome.ToLower().Contains(filter) ||

                            c.Pessoa.PessoaFisica.Cpf.ToLower().Contains(filter));
                }
                else
                {
                    membros = _membroRep.GetAll().Where(x => x.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica)
                        .OrderBy(c => c.Pessoa.PessoaFisica.Nome)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalMembros = _membroRep.GetAll().Where(x => x.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica).Count();
                }
                membros.ForEach(x =>
                {
                    x.Pessoa.Enderecos.ForEach(y =>
                    {
                        var loc = y.Localizacao;
                    });
                });

                IEnumerable<MembroPFViewModel> memmbrosVM = Mapper.Map<IEnumerable<Membro>, IEnumerable<MembroPFViewModel>>(membros);

                PaginacaoConfig<MembroPFViewModel> pagSet = new PaginacaoConfig<MembroPFViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMembros,
                    TotalPages = (int)Math.Ceiling((decimal)totalMembros / currentPageSize),
                    Items = memmbrosVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

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

                var membro = this._membroRep.GetAll().FirstOrDefault(x => x.PessoaId.Equals(usuario.PessoaId));



                if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                {
                    var membroPJViewModel = Mapper.Map<Membro, MembroViewModel>(membro);
                    return request.CreateResponse(HttpStatusCode.OK, membroPJViewModel);

                }
                else
                {
                    var membroPFViewModel = Mapper.Map<Membro, MembroPFViewModel>(membro);
                    return request.CreateResponse(HttpStatusCode.OK, membroPFViewModel);

                }

            });
        }



        [HttpPost]
        [Route("inserirPf")]
        public HttpResponseMessage InserirPf(HttpRequestMessage request, MembroPFViewModel membroPFViewModel)
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




                    if (_membroRep.CpfExistente(membroPFViewModel.Cpf) > 0)
                    {
                        ModelState.AddModelError("CPF Existente", "CPF:" + membroPFViewModel.Cpf + " já existe .");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                    }
                    else if (_membroRep.CroExistente(membroPFViewModel.Cro) > 0)
                    {

                        ModelState.AddModelError("CRO Existente", "CRO:" + membroPFViewModel.Cro + " já existe .");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                    }
                    else
                    {
                        var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                        var pessoa = new Pessoa
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            TipoPessoa = TipoPessoa.PessoaFisica,
                            Ativo = membroPFViewModel.Ativo,
                        };

                        var pessoaFisica = new PessoaFisica
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Pessoa = pessoa,
                            Nome = membroPFViewModel.Nome,
                            Cpf = membroPFViewModel.Cpf,
                            Rg = membroPFViewModel.Rg,
                            Cro = membroPFViewModel.Cro,
                            DtNascimento = membroPFViewModel.DtNascimento,
                            Email = membroPFViewModel.Email,
                            Sexo = (Sexo)membroPFViewModel.Sexo,
                            Ativo = membroPFViewModel.Ativo
                        };

                        _pessoaFisicaRep.Add(pessoaFisica);
                        _unitOfWork.Commit();

                        pessoa.PessoaFisica = pessoaFisica;
                        _pessoaRep.Edit(pessoa);
                        _unitOfWork.Commit();

                        var novoMembro = new Membro
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Pessoa = pessoa,
                            Comprador = membroPFViewModel.Comprador,
                            Ativo = membroPFViewModel.Ativo,
                            Vip = membroPFViewModel.Vip,
                            DddTel = membroPFViewModel.DddTelComl,
                            Telefone = membroPFViewModel.TelefoneComl,
                            DddCel = membroPFViewModel.DddCel,
                            Celular = membroPFViewModel.Celular,
                            Contato = membroPFViewModel.Contato,
                            DataFimPeriodoGratuito = DateTime.Now.AddDays(1)
                        };
                        _membroRep.Add(novoMembro);

                        _unitOfWork.Commit();

                        // Update view model
                        membroPFViewModel = Mapper.Map<Membro, MembroPFViewModel>(novoMembro);
                        response = request.CreateResponse(HttpStatusCode.Created, membroPFViewModel);
                    }
                }
                return response;
            });
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, MembroViewModel membroViewModel)
        {
            membroViewModel.Cnpj = membroViewModel.Cnpj.Replace('.', ' ').Replace('/', ' ').Replace('-', ' ');
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
                    if (_membroRep.CnpjExistente(membroViewModel.Cnpj) > 0)
                    {
                        ModelState.AddModelError("CNPJ Existente", "CNPJ:" + membroViewModel.Cnpj + " já existe .");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                    }
                    else
                    {
                        var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                        var pessoa = new Pessoa
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            TipoPessoa = TipoPessoa.PessoaJuridica,
                            Ativo = membroViewModel.Ativo,
                        };

                        var pessoaJuridica = new PessoaJuridica
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Pessoa = pessoa,
                            NomeFantasia = membroViewModel.NomeFantasia,
                            Cnpj = membroViewModel.Cnpj,
                            RazaoSocial = membroViewModel.RazaoSocial,
                            DtFundacao = membroViewModel.DtFundacao,
                            Email = membroViewModel.Email,
                            InscEstadual = membroViewModel.InscEstadual,
                            Ativo = membroViewModel.Ativo
                        };

                        _pessoaJuridicaRep.Add(pessoaJuridica);
                        _unitOfWork.Commit();

                        pessoa.PessoaJuridica = pessoaJuridica;
                        _pessoaRep.Edit(pessoa);
                        _unitOfWork.Commit();

                        var novoMembro = new Membro
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Pessoa = pessoa,
                            Comprador = membroViewModel.Comprador,
                            Ativo = membroViewModel.Ativo,
                            Vip = membroViewModel.Vip,
                            DddTel = membroViewModel.DddTelComl,
                            Telefone = membroViewModel.TelefoneComl,
                            DddCel = membroViewModel.DddCel,
                            Celular = membroViewModel.Celular,
                            Contato = membroViewModel.Contato,
                            FranquiaId = membroViewModel.FranquiaId,
                            DataFimPeriodoGratuito = membroViewModel.DataFimPeriodoGratuito

                        };
                        _membroRep.Add(novoMembro);

                        _unitOfWork.Commit();

                        // Update view model
                        membroViewModel = Mapper.Map<Membro, MembroViewModel>(novoMembro);
                        response = request.CreateResponse(HttpStatusCode.Created, membroViewModel);
                    }
                }

                return response;
            });
        }


        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, MembroViewModel membroViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response;

                //Remove essa propridades para não fazera validação pois 
                //na atualização do membro nao atualizao usuario do mesmo
                ModelState.Remove("membroViewModel.Usuario.Senha");
                ModelState.Remove("membroViewModel.Usuario.ConfirmSenha");

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    var membro = _membroRep.GetSingle(membroViewModel.Id);

                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    membro.AtualizarMembro(membroViewModel, usuario);

                    _unitOfWork.Commit();

                    //if (membroViewModel.TelefoneId == 0)
                    //{
                    //    var telefone = new Telefone
                    //    {
                    //        UsuarioCriacao = usuario,
                    //        DtCriacao = DateTime.Now,
                    //        DddTelComl = membroViewModel.DddTelComl,
                    //        TelefoneComl = membroViewModel.TelefoneComl,
                    //        DddCel = membroViewModel.DddCel,
                    //        Celular = membroViewModel.Celular,
                    //        Ativo = membroViewModel.Ativo,
                    //        Contato = membroViewModel.Contato

                    //    };

                    //    membro.Pessoa.Telefones.Add(telefone);
                    //}
                    //else
                    //{
                    //    var telefone = _telefoneRep.GetSingle(membroViewModel.TelefoneId);

                    //    telefone.AtualizarTelefoneMembro(membroViewModel, usuario);
                    //}

                    membro.Pessoa.Enderecos.ForEach(x => { x.LocalizacaoGoogle(); });

                    _unitOfWork.Commit();

                    // Update view model
                    membroViewModel = Mapper.Map<Membro, MembroViewModel>(membro);
                    response = request.CreateResponse(HttpStatusCode.OK, membroViewModel);
                }

                return response;
            });
        }


        [HttpPost]
        [Route("atualizarPf")]
        public HttpResponseMessage AtualizarPF(HttpRequestMessage request, MembroPFViewModel membroViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response;

                //Remove essa propridades para não fazera validação pois 
                //na atualização do membro nao atualizao usuario do mesmo
                ModelState.Remove("membroViewModel.Usuario.Senha");
                ModelState.Remove("membroViewModel.Usuario.ConfirmSenha");

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    var membro = _membroRep.GetSingle(membroViewModel.Id);

                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    membro.AtualizarMembroPf(membroViewModel, usuario);

                    _unitOfWork.Commit();



                    membro.Pessoa.Enderecos.ForEach(x => { x.LocalizacaoGoogle(); });

                    _unitOfWork.Commit();

                    // Update view model
                    membroViewModel = Mapper.Map<Membro, MembroPFViewModel>(membro);
                    response = request.CreateResponse(HttpStatusCode.OK, membroViewModel);
                }

                return response;
            });
        }


        [HttpGet]
        [Route("membroCategorias")]
        public HttpResponseMessage GetMembroCategoria(HttpRequestMessage request, int membroId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var membroCategorias = _membeoCategoriaRep.GetAll().Where(x => x.Membro.Id == membroId).ToList();


                IEnumerable<CategoriaViewModel> membroCategoriasVM = Mapper.Map<IEnumerable<MembroCategoria>, IEnumerable<CategoriaViewModel>>(membroCategorias);

                response = request.CreateResponse(HttpStatusCode.OK, membroCategoriasVM);

                return response;
            });
        }


        [HttpGet]
        [Route("membroFornecedores")]
        public HttpResponseMessage GetMembroFornecedor(HttpRequestMessage request, int membroId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var membroFornecedores = _membroFornecedorRep.GetAll().Where(x => x.Membro.Id == membroId).ToList();


                IEnumerable<MembroFornecedorViewModel> membroFornecedoresVM = Mapper.Map<IEnumerable<MembroFornecedor>, IEnumerable<MembroFornecedorViewModel>>(membroFornecedores);

                response = request.CreateResponse(HttpStatusCode.OK, membroFornecedoresVM);

                return response;
            });
        }


        [HttpGet]
        [Route("membroFornecedoresProduto")]
        public HttpResponseMessage MembroFornecedoresProduto(HttpRequestMessage request, int categoriaId)
        {
            return CreateHttpResponse(request, () =>
            {

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.FirstOrDefault(m => m.PessoaId == usuario.Pessoa.Id);


                HttpResponseMessage response = null;
                var membroFornecedores =
                    _membroFornecedorRep.GetAll()
                        .Where(
                            x =>
                                x.Membro.Id == membro.Id &&
                                x.Fornecedor.FornecedorCategorias.Any(g => g.CategoriaId == categoriaId))
                        .ToList();


                IEnumerable<MembroFornecedorViewModel> membroFornecedoresVM = Mapper.Map<IEnumerable<MembroFornecedor>, IEnumerable<MembroFornecedorViewModel>>(membroFornecedores);

                response = request.CreateResponse(HttpStatusCode.OK, membroFornecedoresVM);

                return response;
            });
        }

        [HttpGet]
        [Route("fornecedorRegiaoMembro")]
        public HttpResponseMessage GetFornecedorRegiao(HttpRequestMessage request, int membroId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var membro = _membroRep.GetSingle(membroId);

                var cidades = membro.Pessoa.Enderecos.Select(x => x.CidadeId).Distinct();
                var categorias = membro.MembroCategorias.Select(x => x.CategoriaId).ToList();

                var listaFornecedores = new List<Fornecedor>();

                var cidadesFornecedorCorrido = _fornecedorRegiaoRep.GetAll().Where(x => cidades.Contains(x.CidadeId)).Distinct().Select(c => c.Fornecedor).ToList();

                var cidadesFornecedorSemana = _fornecedorPrazoSemanalRep.GetAll().Where(x => cidades.Contains(x.CidadeId)).Distinct().Select(c => c.Fornecedor).ToList();

                listaFornecedores.AddRange(cidadesFornecedorCorrido);
                listaFornecedores.AddRange(cidadesFornecedorSemana);
                var listaFornecedoresUnicos = listaFornecedores.Distinct().ToList();


                var fornecedores = listaFornecedoresUnicos.Where(
                        x => x.FornecedorCategorias.Any(p => categorias.Contains(p.CategoriaId))).ToList();

                var membroFornecedoresVM = Mapper.Map<IEnumerable<Fornecedor>, IEnumerable<FornecedorViewModel>>(fornecedores);

                response = request.CreateResponse(HttpStatusCode.OK, membroFornecedoresVM);

                return response;
            });
        }

        [HttpPost]
        [Route("inserirMembroCategoria/{membroId:int}")]
        public HttpResponseMessage InserirMembroCategoria(HttpRequestMessage request, int membroId, int[] listCategoria)
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
                    Membro _membro = _membroRep.GetSingle(membroId);


                    var membroCategorias =
                        _membeoCategoriaRep.GetAll().Where(x => x.MembroId == membroId);

                    if (membroCategorias.Any())
                        _membeoCategoriaRep.DeleteAll(membroCategorias);


                    foreach (var item in listCategoria)
                    {
                        MembroCategoria membroCategoria = new MembroCategoria()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            MembroId = membroId,
                            CategoriaId = item,
                            Ativo = true


                        };

                        _membro.MembroCategorias.Add(membroCategoria);
                    }

                    _membroRep.Edit(_membro);
                    _unitOfWork.Commit();



                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });


                }

                return response;
            });
        }


        [HttpPost]
        [Route("inserirMembroFornecedor")]
        public HttpResponseMessage InserirMembroFornecedor(HttpRequestMessage request, MembroFornecedoresAdm membroFornecedoresAdm)
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
                    var notificacoes = new NotificacoesAlertasService();

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    Membro _membro = _membroRep.GetSingle(membroFornecedoresAdm.MembroId);


                    var membroFornecedores =
                        _membroFornecedorRep.FindBy(x => x.MembroId == membroFornecedoresAdm.MembroId && membroFornecedoresAdm.FornecedoresDel.Contains(x.FornecedorId));

                    var solicitacaoMembros =
                        _solicitacaoMembroFornecedor.FindBy(x => x.MembroId == membroFornecedoresAdm.MembroId && membroFornecedoresAdm.FornecedoresDel.Contains(x.FornecedorId));

                    if (membroFornecedores.Any())
                        _membroFornecedorRep.DeleteAll(membroFornecedores);

                    if (solicitacaoMembros.Any())
                        _solicitacaoMembroFornecedor.DeleteAll(solicitacaoMembros);


                    if (membroFornecedoresAdm.MembroId <= 0)
                        membroFornecedoresAdm.MembroId = usuario.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                                                         Convert.ToInt32(usuario.Pessoa.PessoaJuridicaId) : Convert.ToInt32(usuario.Pessoa.PessoaFisicaId);


                    foreach (var item in membroFornecedoresAdm.FornecedoresAdd)
                    {
                        MembroFornecedor membroFornecedor = new MembroFornecedor()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            MembroId = membroFornecedoresAdm.MembroId,
                            FornecedorId = item,
                            Ativo = false
                        };

                        _membro.MembroFornecedores.Add(membroFornecedor);

                        var solicitacaoMembroFornecedor = new SolicitacaoMembroFornecedor
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            MembroId = membroFornecedoresAdm.MembroId,
                            FornecedorId = item,
                            MotivoRecusa = "Aguardando",
                            Ativo = false
                        };

                        _solicitacaoMembroFornecedor.Add(solicitacaoMembroFornecedor);

                        var corpoEmail = _templateEmail.FindBy(e => e.Id == 36).Select(e => e.Template).FirstOrDefault();
                        String corpoEmailNomeFornecedor = null;
                        var fornecedor = _fornecedorRep.GetSingle(item);

                        if (_membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                        {
                            corpoEmailNomeFornecedor = corpoEmail.Replace("#NomeFornecedor#", fornecedor.Pessoa.PessoaJuridica.NomeFantasia)
                            .Replace("#Membro#", _membro.Pessoa.PessoaJuridica.NomeFantasia);
                        }
                        else
                        {
                            corpoEmailNomeFornecedor = corpoEmail.Replace("#NomeFornecedor#", fornecedor.Pessoa.PessoaJuridica.NomeFantasia)
                            .Replace("#Membro#", _membro.Pessoa.PessoaFisica.Nome);
                        }

                        var emails = new Emails
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Novo Membro Comprador - Estão querendo comprar com você. Corra para aceitar, faça novos negocios",
                            EmailDestinatario = fornecedor.Pessoa.PessoaJuridica.Email,
                            CorpoEmail = corpoEmailNomeFornecedor.Trim(),
                            Status = Status.NaoEnviado,
                            Origem = Origem.MembroSolicitaFornecedor,
                            Ativo = true

                        };

                        var sms = new Sms
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            Numero = fornecedor.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + fornecedor.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                            Mensagem = "Economiza Já - Novo Membro Comprador - Estão querendo comprar com você. Corra para aceitar e comece a fazer novos negocios.",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.MembroSolicitaFornecedor,
                            Ativo = true

                        };

                        var usuFornecedores = fornecedor.Pessoa.Usuarios.Select(u => u.Id).ToList();

                        usuFornecedores.ForEach(id =>
                        {
                            if (notificacoes.PodeEnviarNotificacao(id, (int)TipoAviso.PendentedeAceiteFornecedorMembro, TipoAlerta.EMAIL))
                            {
                                _emailsNotificaFornecedorMembro.Add(emails);
                            }

                            if (notificacoes.PodeEnviarNotificacao(id, (int)TipoAviso.PendentedeAceiteFornecedorMembro, TipoAlerta.SMS))
                            {
                                _sms.Add(sms);
                            }
                        });

                    }

                    _membroRep.Edit(_membro);

                    _unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                }

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("pesquisarfornecedor/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetMembroSolicitaFornecedor(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            var currentPage = page ?? 0;
            var currentPageSize = pageSize ?? 10;

            return CreateHttpResponse(request, () =>
            {
                var fornecedoresDiasCorridos = new List<Fornecedor>();
                var fornecedorPrazoSemanal = new List<Fornecedor>();
                var fornecedores = new List<Fornecedor>();
                var fornecedoresFilter = new List<Fornecedor>();
                var totalFornecedores = new int();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.FirstOrDefault(m => m.PessoaId == usuario.Pessoa.Id);
                var cidadeId = usuario.Pessoa.Enderecos.FirstOrDefault(x => x.EnderecoPadrao).CidadeId;
                var categorias = membro.MembroCategorias.Select(x => x.CategoriaId).ToList();

                if (!string.IsNullOrEmpty(filter))
                {
                    fornecedoresFilter = _fornecedorRep.GetAll().Where(c =>
                    c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter) ||
                    c.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                    c.Pessoa.PessoaJuridica.Cnpj.ToLower().Contains(filter) ||
                    c.Descricao.ToLower().Contains(filter) ||
                    c.PalavrasChaves.ToLower().Contains(filter) ||
                    c.FornecedorCategorias.Select(f => f.Categoria.DescCategoria).Contains(filter))
                    .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial).ToList();
                }

                if (fornecedoresFilter.Count > 0)
                {

                    fornecedoresDiasCorridos = fornecedoresFilter.Where(x => x.Ativo &&
                    x.FornecedorCategorias.Any(c => categorias.Contains(c.CategoriaId)) &&
                    x.FornecedorRegiao.Any(y => y.CidadeId == cidadeId)).ToList();

                    fornecedores.AddRange(fornecedoresDiasCorridos);

                    //fornecedorPrazoSemanal =
                    //    fornecedoresFilter.Where(x => x.Ativo &&
                    //      x.FornecedorCategorias.Any(c => categorias.Contains(c.CategoriaId)) &&
                    //    x.FornecedorRegiaoSemanal.Any(y => y.CidadeId == cidadeId)).ToList();

                    //fornecedores.AddRange(fornecedorPrazoSemanal);
                }
                else
                {

                    fornecedoresDiasCorridos = _fornecedorRep.FindBy(x => x.Ativo &&
                    x.FornecedorCategorias.Any(c => categorias.Contains(c.CategoriaId)) &&
                    x.FornecedorRegiao.Any(f => f.CidadeId == cidadeId)).ToList();

                    fornecedores.AddRange(fornecedoresDiasCorridos);

                    //fornecedorPrazoSemanal =
                    //    _fornecedorRep.GetAll().Where(x => x.Ativo &&
                    //      x.FornecedorCategorias.Any(c => categorias.Contains(c.CategoriaId)) &&
                    //    x.FornecedorRegiaoSemanal.Any(y => y.CidadeId == cidadeId)).ToList();

                    //fornecedores.AddRange(fornecedorPrazoSemanal);
                }

                var listaFornecedores = new List<Fornecedor>();

                if (membro.FranquiaId == null)
                {
                    listaFornecedores = fornecedores.Distinct().Where(x => x.Pessoa.Usuarios.Any()).ToList();
                }
                else
                {
                    var idsFornecedoresFranquia = membro.Franquia.Fornecedores.Select(x => x.FornecedorId).ToList();
                    //Pega somente os fornecedores que a Franquia estiver relacionada na tabela FranquiaFornecedor
                    listaFornecedores = fornecedores.Where(x => idsFornecedoresFranquia.Contains(x.Id)).Distinct().ToList();
                }

                totalFornecedores = listaFornecedores.Count;

                var listForn = listaFornecedores.OrderBy(x => x.Pessoa.PessoaJuridica.RazaoSocial)
                     .Skip(currentPage * currentPageSize)
                     .Take(currentPageSize);

                //var fornecedoresVM = Mapper.Map<IEnumerable<Fornecedor>, IEnumerable<MembroSolicitaFornecedorViewModel>>(listForn);
                var fornecedoresVM = new List<MembroSolicitaFornecedorViewModel>();

                listForn.ForEach(x =>
                {

                    fornecedoresVM.Add(new MembroSolicitaFornecedorViewModel
                    {
                        DescAtivo = x.Ativo,
                        FornecedorId = x.Id,
                        NomeFornecedor = x.Pessoa.PessoaJuridica.NomeFantasia,
                        NomeRazaoSocialFornecedor = x.Pessoa.PessoaJuridica.RazaoSocial,
                        CnpjFornecedor = x.Pessoa.PessoaJuridica.Cnpj,
                        PrazoEntegaFornecedor = x.FornecedorRegiao.FirstOrDefault(r => r.CidadeId == cidadeId).Prazo.ToString(),

                        FormasPagamento = x.FornecedorFormaPagtos != null ? x.FornecedorFormaPagtos.Select(f => new FormaPagtoViewModel
                        {
                            Id = f.Id,
                            DescFormaPagto = f.FormaPagto.DescFormaPagto,
                            Avista = f.FormaPagto.Avista,
                            Desconto = f.Desconto,
                            QtdParcelas = f.FormaPagto.QtdParcelas

                        }).ToList() : new List<FormaPagtoViewModel>(),

                        //FornecedorPrazoSemanal = x.FornecedorRegiaoSemanal != null && x.FornecedorRegiaoSemanal.Count(fs => fs.CidadeId == cidadeId) > 0 ?
                        //        x.FornecedorRegiaoSemanal.Where(fs => fs.CidadeId == cidadeId).Select(p => new FornecedorPrazoSemanalViewModel
                        //        {
                        //            Id = p.Id,
                        //            CidadeId = p.CidadeId,
                        //            DescCidade = p.Cidade.DescCidade,
                        //            DiaSemana = p.DiaSemana,
                        //            FornecedorId = p.FornecedorId,
                        //            Cif = p.Cif ?? p.Cif.Value,
                        //            TaxaEntrega = p.TaxaEntrega,
                        //            VlPedMinRegiao = p.VlPedMinRegiao ?? p.VlPedMinRegiao.Value

                        //        }).ToList() : new List<FornecedorPrazoSemanalViewModel>(),

                        ObservacaoFormPagto = x.Observacao,
                        ObservacaoEntrega = x.ObservacaoEntrega,
                        VlPedMinimo = x.FornecedorRegiao.Count > 0 && x.FornecedorRegiao.FirstOrDefault(re => re.CidadeId == cidadeId).VlPedMinRegiao.Value > 0?
                                        (x.FornecedorRegiao.FirstOrDefault(re => re.CidadeId == cidadeId).VlPedMinRegiao ?? x.VlPedidoMin) :
                                      x.FornecedorRegiaoSemanal.Count > 0 && x.FornecedorRegiaoSemanal.FirstOrDefault(re => re.CidadeId == cidadeId).VlPedMinRegiao.Value > 0 ?
                                        x.FornecedorRegiaoSemanal.FirstOrDefault(re => re.CidadeId == cidadeId).VlPedMinRegiao :
                                      x.VlPedidoMin,

                        MediaAvaliacaoPedido = x.AvaliacaoFornecedor.Sum(so => so.NotaQualidadeProdutos + so.NotaTempoEntrega + so.NotaAtendimento),
                        QtdNotas = x.AvaliacaoFornecedor.Count * 3,
                        FormaPagtos = x.FornecedorFormaPagtos.Select(q => q.FormaPagtoId).ToArray(),
                        TrabalhaMembro = membro.MembroFornecedores.Any(mf => mf.FornecedorId == x.Id)
                    });

                });

                var pagSet = new PaginacaoConfig<MembroSolicitaFornecedorViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalFornecedores,
                    TotalPages = (int)Math.Ceiling((decimal)totalFornecedores / currentPageSize),
                    Items = fornecedoresVM
                };

                var response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpPost]
        [Route("inserirMembroFornecedorTelaMembro/{membroId:int}/{fornecedorId:int}")]
        public HttpResponseMessage InserirMembroFornecedorTelaMembro(HttpRequestMessage request, int membroId, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response;
                NotificacoesAlertasService notificacoesAlertasService = new NotificacoesAlertasService();

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    var membro = _membroRep.FirstOrDefault(m => m.PessoaId == usuario.Pessoa.Id);
                    var solicitado = membro.MembroFornecedores.Any(x => x.FornecedorId == fornecedorId && !x.Ativo);

                    if (!solicitado)
                    {
                        var membroFornecedor = new MembroFornecedor
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            MembroId = membro.Id,
                            FornecedorId = fornecedorId,
                            Ativo = false
                        };

                        membro.MembroFornecedores.Add(membroFornecedor);

                        _membroRep.Edit(membro);

                        var solicitacaoMembroFornecedor = new SolicitacaoMembroFornecedor
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            MembroId = membro.Id,
                            FornecedorId = fornecedorId,
                            MotivoRecusa = "Aguardando",
                            Ativo = false
                        };

                        _solicitacaoMembroFornecedor.Add(solicitacaoMembroFornecedor);

                        //Envia e-mail para fornecedor
                        var fornecedor = _fornecedorRep.FindBy(f => f.Id == membroFornecedor.FornecedorId).FirstOrDefault();

                        var corpoEmail = _templateEmail.FindBy(e => e.Id == 9).Select(e => e.Template).FirstOrDefault();

                        if (fornecedor != null)
                        {
                            if (corpoEmail != null)
                            {

                                String corpoEmailNomeFornecedor = null;

                                if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                                {

                                    corpoEmailNomeFornecedor = corpoEmail.Replace("#NomeFornecedor#", fornecedor.Pessoa.PessoaJuridica.NomeFantasia).Replace("#NomeMembro#", membro.Pessoa.PessoaJuridica.NomeFantasia);
                                }
                                else
                                {
                                    corpoEmailNomeFornecedor = corpoEmail.Replace("#NomeFornecedor#", fornecedor.Pessoa.PessoaJuridica.NomeFantasia).Replace("#NomeMembro#", membro.Pessoa.PessoaFisica.Nome);

                                }

                                var emails = new Emails
                                {
                                    UsuarioCriacao = usuario,
                                    DtCriacao = DateTime.Now,
                                    AssuntoEmail = "Novo Membro Comprador - Estão querendo comprar com você. Corra para aceitar, faça novos negocios",
                                    EmailDestinatario = fornecedor.Pessoa.PessoaJuridica.Email,
                                    CorpoEmail = corpoEmailNomeFornecedor.Trim(),
                                    Status = Status.NaoEnviado,
                                    Origem = Origem.MembroSolicitaFornecedor,
                                    Ativo = true

                                };

                                var sms = new Sms
                                {
                                    UsuarioCriacao = usuario,
                                    DtCriacao = DateTime.Now,
                                    Numero = fornecedor.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + fornecedor.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                                    Mensagem = "Economiza Já - Novo Membro Comprador - Estão querendo comprar com você. Corra para aceitar e comece a fazer novos negocios.",
                                    Status = StatusSms.NaoEnviado,
                                    OrigemSms = TipoOrigemSms.MembroSolicitaFornecedor,
                                    Ativo = true

                                };


                                List<int> idUsuariosFornecedor = fornecedor.Pessoa.Usuarios.Where(x => x.Ativo)
                                    .Select(i => i.Id).ToList();
                                int tipoAvisoId = (int)TipoAviso.PendentedeAceiteFornecedorMembro;

                                foreach (var id in idUsuariosFornecedor)
                                {
                                    if (notificacoesAlertasService.PodeEnviarNotificacao(id, tipoAvisoId, TipoAlerta.EMAIL))
                                    {
                                        //Envia EMAIL para fornecedor
                                        _emailsNotificaFornecedorMembro.Add(emails);
                                    }

                                    if (notificacoesAlertasService.PodeEnviarNotificacao(id, tipoAvisoId, TipoAlerta.SMS))
                                    {
                                        //Envia SMS para fonecedor
                                        _sms.Add(sms);
                                    }
                                }
                            }

                            //inserir Aviso
                            var lUsu = _usuarioRep.FindBy(u => u.PessoaId == fornecedor.PessoaId).ToList();
                            Avisos novoAviso = new Avisos();
                            foreach (var usu in lUsu)
                            {
                                novoAviso = new Avisos();

                                novoAviso.UsuarioCriacaoId = 1;
                                novoAviso.DtCriacao = DateTime.Now;
                                novoAviso.Ativo = true;
                                novoAviso.IdReferencia = membro.Id;
                                novoAviso.DataUltimoAviso = DateTime.Now;
                                novoAviso.ExibeNaTelaAvisos = true;
                                novoAviso.TipoAvisosId = (int)TipoAviso.PendentedeAceiteFornecedorMembro; //Id Aviso
                                novoAviso.URLPaginaDestino = "/#/membro";
                                novoAviso.TituloAviso = "Pendente aceite novo Membro";
                                novoAviso.ToolTip = "Novo Membro";
                                string descAviso = string.Empty;

                                if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                                {
                                    descAviso = "Pendente aceite novo Membro " + membro.Pessoa.PessoaJuridica.NomeFantasia;
                                }
                                else
                                {
                                    descAviso = "Pendente aceite novo Membro " + membro.Pessoa.PessoaFisica.Nome;
                                }

                                novoAviso.DescricaoAviso = descAviso.Length > 99 ? descAviso.Substring(0, 99) : descAviso;
                                novoAviso.ModuloId = 4; //Modulo Fornecedor
                                novoAviso.UsuarioNotificadoId = usu.Id;
                                _avisosRep.Add(novoAviso);
                            }
                        }

                        _unitOfWork.Commit();

                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                        var usuarioFornecedor = fornecedor.Pessoa.Usuarios;

                        if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
                        {
                            new NotificacoesHub(this._unitOfWork).NotificaMembroSolicitaFornecedor(membro.Pessoa.PessoaJuridica.NomeFantasia, usuarioFornecedor);
                        }
                        else
                        {
                            new NotificacoesHub(this._unitOfWork).NotificaMembroSolicitaFornecedor(membro.Pessoa.PessoaFisica.Nome, usuarioFornecedor);

                        }
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.NotModified, new { success = false });

                    }

                }

                return response;
            });
        }

        [HttpPost]
        [Route("cancelarMembroFornecedorTelaMembro/{fornecedorId:int}")]
        public HttpResponseMessage CancelarMembroFornecedorTelaMembro(HttpRequestMessage request, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {
                var response = new HttpResponseMessage();

                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var membro = _membroRep.FirstOrDefault(m => m.PessoaId == usuario.Pessoa.Id);

                var membroFornecedor = membro.MembroFornecedores.FirstOrDefault(x => x.FornecedorId == fornecedorId);

                if (membroFornecedor == null)
                {
                    response = request.CreateResponse(HttpStatusCode.OK, new { success = false });
                }
                else
                {
                    this._membroFornecedorRep.Delete(membroFornecedor);

                    this._unitOfWork.Commit();

                    response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                }

                return response;

            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("pesquisarespostafornecedor/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetSolicitacaoMembroFornecedor(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<SolicitacaoMembroFornecedor> fornecedoresRecusouMembro = null;
                IEnumerable<SolicitacaoMembroFornecedor> solicitacao = null;
                int totalFornecedoresRecusouMembro = new int();

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                Membro membro = _membroRep.FindBy(m => m.Pessoa.Id == usuario.Pessoa.Id).FirstOrDefault();
                var fornecedoresId = membro.MembroFornecedores.Select(x => x.FornecedorId).ToList();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    fornecedoresRecusouMembro =
                        _solicitacaoMembroFornecedor.FindBy(c => c.Fornecedor.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter) ||
                                                   c.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                                                   c.Fornecedor.Pessoa.PessoaJuridica.Cnpj.Contains(filter) &&
                                                   c.MembroId == membro.Id)
                                                   .OrderBy(o => o.Fornecedor.Pessoa.PessoaJuridica.RazaoSocial)
                                                   .Skip(currentPage * currentPageSize)
                                                   .Take(currentPageSize)
                                                   .ToList();

                    var fornecedoresAgrupados = fornecedoresRecusouMembro.GroupBy(x => x.FornecedorId).Select(x => new { fornecedor = x.Key, solicitacao = x });

                    var fornecedores =
                        fornecedoresAgrupados
                        .Select(x => new
                        {
                            Idfornecedor = x.fornecedor,
                            Solicitacao = x.solicitacao
                        .FirstOrDefault(p => p.DtCriacao == x.solicitacao.Select(d => d.DtCriacao).Max())
                        });

                    solicitacao = fornecedores.Select(x => x.Solicitacao)
                        .Skip(currentPage * currentPageSize).Take(currentPageSize);

                    totalFornecedoresRecusouMembro =
                        _solicitacaoMembroFornecedor.FindBy(c => c.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                        c.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                        c.Fornecedor.Pessoa.PessoaJuridica.Cnpj.Contains(filter) &&
                                                   c.MembroId == membro.Id).GroupBy(x => x.FornecedorId).Count();



                }
                else
                {
                    fornecedoresRecusouMembro = _solicitacaoMembroFornecedor.GetAll()
                      .Where(f => f.MembroId == membro.Id)
                      .OrderBy(f => f.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia).ToList();

                    var fornecedoresAgrupados = fornecedoresRecusouMembro.GroupBy(x => x.FornecedorId).Select(x => new { fornecedor = x.Key, solicitacao = x });

                    var fornecedores = fornecedoresAgrupados
                            .Select(x => new
                            {
                                Idfornecedor = x.fornecedor,
                                Solicitacao = x.solicitacao
                                    .FirstOrDefault(p => p.DtCriacao == x.solicitacao.Select(d => d.DtCriacao).Max())
                            });

                    solicitacao = fornecedores.Select(x => x.Solicitacao)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize);

                    totalFornecedoresRecusouMembro = _solicitacaoMembroFornecedor
                    .GetAll().Where(f => f.MembroId == membro.Id)
                    .GroupBy(x => x.FornecedorId).Count();
                }

                var fornecedoresRecusaVM = Mapper.Map<IEnumerable<SolicitacaoMembroFornecedor>, IEnumerable<SolicitacaoMembroFornecedorViewModel>>(solicitacao);

                var pagSet = new PaginacaoConfig<SolicitacaoMembroFornecedorViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalFornecedoresRecusouMembro,
                    TotalPages = (int)Math.Ceiling((decimal)totalFornecedoresRecusouMembro / currentPageSize),
                    Items = fornecedoresRecusaVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpGet]
        [Route("pesquisarmembrofornecedor")]
        public HttpResponseMessage GetMembroSolicitaFornecedor(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var idMembro = _membroRep.FindBy(f => f.PessoaId == usuario.PessoaId).Select(f => f.Id).FirstOrDefault();

                var membroSolicitaFornecedor = _membroFornecedorRep.GetAll().Where(x => x.Ativo == false && x.MembroId == idMembro).ToList();

                var fornecedorVM = Mapper.Map<IEnumerable<MembroFornecedor>, IEnumerable<MembroFornecedorViewModel>>(membroSolicitaFornecedor);

                var pagSet = new PaginacaoConfig<MembroFornecedorViewModel>
                {
                    Items = fornecedorVM
                };

                var response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpGet]
        [Route("membroTeste")]
        public HttpResponseMessage GetMembroTeste(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                //teste exec procedure
                // _membeoCategoriaRep.ExecuteWithStoreProcedure( "spGetProducts @bigCategoryId",new SqlParameter("bigCategoryId", SqlDbType.BigInt) { Value = 1 });
                // _unitOfWork.Commit();

                //teste envio sms
                //_smsService.EnviaSms("11980858011", "1 Teste Economiza Já");
                //_smsService.EnviaSms("11945070440", "2 Teste Economiza Já");
                //_smsService.EnviaSms("11976410088", "3 Teste Economiza Já");
                //_smsService.EnviaSms("11987781444", "4 Teste Economiza Já");
                //_smsService.EnviaSms("11951447209", "5 Teste Economiza Já");
                //_smsService.EnviaSms("11980858011", "Teste Economiza Já");
                //_smsService.EnviaSms("11980858011", "Economiza Já - E aí Renatinho tá funfando essa bagaça!!!");




                HttpResponseMessage response = null;
                var membroCategorias = _membeoCategoriaRep.GetAll();


                IEnumerable<CategoriaViewModel> membroCategoriasVM = Mapper.Map<IEnumerable<MembroCategoria>, IEnumerable<CategoriaViewModel>>(membroCategorias);

                response = request.CreateResponse(HttpStatusCode.OK, membroCategoriasVM);

                return response;
            });
        }
    }
}
