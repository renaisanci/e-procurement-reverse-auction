using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AutoMapper;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.API_Web.Models;
using ECC.Dados.Extensions;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeArquivo;
using ECC.EntidadeEmail;
using ECC.EntidadeEndereco;
using ECC.Entidades;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using ECC.EntidadeRecebimento;
using ECC.Servicos.Abstrato;
using Microsoft.AspNet.Identity;
using ECC.Servicos;

namespace ECC.API_Web.Controllers
{
    [Authorize(Roles = "Admin, Fornecedor, Membro, Franquia")]
    [RoutePrefix("api/usuario")]
    public class UsuarioController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Pessoa> _pessoaRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Grupo> _grupoRep;
        private readonly IEntidadeBaseRep<UsuarioGrupo> _usuarioGrupoRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        private readonly IMembershipService _membershipService;
        private readonly IEncryptionService _encryptionService;
        private readonly IEmailService _emailService;
        private readonly IEntidadeBaseRep<Endereco> _enderecoRep;
        private readonly IEntidadeBaseRep<HorasEntregaMembro> _horarioEntregaMembroRep;
        private readonly IEntidadeBaseRep<TermoUso> _termoUsoRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<Mensalidade> _mensalidadeRep;
        private readonly IEntidadeBaseRep<Fatura> _faturaRep;
        private readonly IEntidadeBaseRep<UsuarioCancelado> _usuarioCanceladoRep;
        private readonly IPagamentoService _pagamentoService;


        public UsuarioController(IMembershipService membershipService,
            IEncryptionService encryptionService,
            IEmailService emailService,
            IEntidadeBaseRep<Endereco> enderecoRep,
            IEntidadeBaseRep<Pessoa> pessoaRep,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<TemplateEmail> templateEmailRep,
            IEntidadeBaseRep<UsuarioGrupo> usuarioGrupoRep,
            IEntidadeBaseRep<Grupo> grupoRep,
            IEntidadeBaseRep<HorasEntregaMembro> horarioEntregaMembroRep,
            IEntidadeBaseRep<TermoUso> termoUsoRep,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<Mensalidade> mensalidadeRep,
            IEntidadeBaseRep<Fatura> faturaRep,
            IEntidadeBaseRep<UsuarioCancelado> usuarioCanceladoRep,
            IPagamentoService pagamentoService,
            IEntidadeBaseRep<Erro> errosRepository, IUnitOfWork unitOfWork)
            : base(usuarioRep, errosRepository, unitOfWork)
        {
            _usuarioGrupoRep = usuarioGrupoRep;
            _pessoaRep = pessoaRep;
            _usuarioRep = usuarioRep;
            _grupoRep = grupoRep;
            _templateEmailRep = templateEmailRep;
            _enderecoRep = enderecoRep;
            _membershipService = membershipService;
            _encryptionService = encryptionService;
            _emailService = emailService;
            _horarioEntregaMembroRep = horarioEntregaMembroRep;
            _termoUsoRep = termoUsoRep;
            _membroRep = membroRep;
            _fornecedorRep = fornecedorRep;
            _mensalidadeRep = mensalidadeRep;
            _faturaRep = faturaRep;
            _usuarioCanceladoRep = usuarioCanceladoRep;
            _pagamentoService = pagamentoService;
        }

        [HttpGet]
        [Route("perfil")]
        public HttpResponseMessage GetPerfil(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var usuarioViewModel = Mapper.Map<Usuario, UsuarioViewModel>(usuario);

                var response = request.CreateResponse(HttpStatusCode.OK, usuarioViewModel);

                return response;
            });
        }

        [Route("inserir")]
        [HttpPost]
        public HttpResponseMessage Inserir(HttpRequestMessage request, UsuarioViewModel novoUsuario)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var usuarioEmail = _usuarioRep.GetSingleByEmail(novoUsuario.UsuarioEmail, novoUsuario.PerfilId);

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                }
                else if (usuarioEmail != null)
                {
                    ModelState.AddModelError("Email Existente", "Email:" + usuarioEmail.UsuarioEmail + " já existe .");
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                    ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                          .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var booNovoAdmin = novoUsuario.PessoaId == 0 && novoUsuario.Senha == "ABCD1234" && novoUsuario.PerfilId == 1;

                    if (booNovoAdmin)
                    {
                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        var random = new Random();
                        var senhaRand = new string(
                            Enumerable.Repeat(chars, 8)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());

                        novoUsuario.Senha = senhaRand;

                        var novaPessoa = new Pessoa
                        {
                            Ativo = true,
                            DtCriacao = DateTime.Now,
                            UsuarioCriacao = usuario,
                            TipoPessoa = TipoPessoa.PessoaFisica
                        };
                        _pessoaRep.Add(novaPessoa);
                        _unitOfWork.Commit();
                        //novoUsuario.PessoaId = _pessoaRep.GetAll().OrderByDescending(u => u.Id).FirstOrDefault().Id;
                        novoUsuario.PessoaId = novaPessoa.Id;
                    }

                    var user = _membershipService.CreateUser(novoUsuario.UsuarioNome, novoUsuario.UsuarioEmail, novoUsuario.Senha,
                      novoUsuario.PerfilId, novoUsuario.PessoaId, usuario.Id, novoUsuario.FlgMaster);

                    if (booNovoAdmin)
                    {
                        var template = _templateEmailRep.GetSingle(3).Template;
                        _emailService.EnviaEmail(novoUsuario.UsuarioEmail, "",
                            _emailService.MontaEmail(novoUsuario, template), "Novo Usuário - Economiza Já");
                    }

                    // Update view model
                    var usuarioVM = Mapper.Map<Usuario, UsuarioViewModel>(user);
                    response = request.CreateResponse(HttpStatusCode.Created, usuarioVM);
                }

                return response;
            });
        }


        [Route("atualizar")]
        [HttpPost]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, UsuarioViewModel atuUsuario)
        {
            return CreateHttpResponse(request, () =>
            {
                if (atuUsuario.Senha == "" && atuUsuario.ConfirmSenha == "")
                {
                    ModelState.Remove("atuUsuario.Senha");
                    ModelState.Remove("atuUsuario.ConfirmSenha");
                }

                if (!ModelState.IsValid)
                {
                    var response = request.CreateResponse(HttpStatusCode.BadRequest,
                         ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                               .Select(m => m.ErrorMessage).ToArray());
                    return response;
                }
                else
                {
                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    HttpResponseMessage response;

                    var user = _usuarioRep.GetSingle(atuUsuario.Id);

                    //Se o e-mail cadastrado for diferente do e-mail de atualização,
                    //Verifica se já existe um usuário usando este e-mail
                    if (user.UsuarioEmail != atuUsuario.UsuarioEmail && _usuarioRep.GetSingleByEmail(atuUsuario.UsuarioEmail, atuUsuario.PerfilId) != null)
                    {
                        ModelState.AddModelError("Email Existente", "Email:" + atuUsuario.UsuarioEmail + " já existe .");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                            ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                .Select(m => m.ErrorMessage).ToArray());
                        return response;
                    }

                    user.AtualizarUsuario(atuUsuario, usuario, _encryptionService);

                    _unitOfWork.Commit();

                    // Update view model
                    var usuarioVM = Mapper.Map<Usuario, UsuarioViewModel>(user);

                    response = request.CreateResponse(HttpStatusCode.OK, usuarioVM);

                    return response;
                }
            });
        }


        [HttpGet]
        [Route("usuarios")]
        public HttpResponseMessage GetUsuarioPessoa(HttpRequestMessage request, int pessoaId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var usuariosPessoa = _usuarioRep.GetAll().Where(x => x.Pessoa.Id == pessoaId);


                IEnumerable<UsuarioViewModel> usuariosVM = Mapper.Map<IEnumerable<Usuario>, IEnumerable<UsuarioViewModel>>(usuariosPessoa);

                response = request.CreateResponse(HttpStatusCode.OK, usuariosVM);

                return response;
            });
        }

        [HttpPost]
        [Route("salvarnovasenha")]
        public HttpResponseMessage SalvarNovaSenha(HttpRequestMessage request, UsuarioViewModel usuarioNovaSenha) //, UsuarioViewModel usuarioDadosAtuais
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                ModelState.Remove("usuarioNovaSenha.UsuarioNome");
                ModelState.Remove("usuarioNovaSenha.UsuarioEmail");


                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                         ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                               .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    Usuario usuarioLogado = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    if (string.Equals(_encryptionService.EncryptSenha(usuarioNovaSenha.Senha, usuarioLogado.Chave), usuarioLogado.Senha))
                    {
                        usuarioNovaSenha.UsuarioNome = usuarioLogado.UsuarioNome;
                        usuarioNovaSenha.UsuarioEmail = usuarioLogado.UsuarioEmail;
                        usuarioNovaSenha.FlgMaster = usuarioLogado.FlgMaster;
                        usuarioNovaSenha.Ativo = usuarioLogado.Ativo;
                        usuarioNovaSenha.PerfilId = usuarioLogado.PerfilId;

                        Usuario _user = _usuarioRep.GetSingle(usuarioLogado.Id);
                        Usuario usuarioAlterado = _usuarioRep.GetSingle(usuarioLogado.Id);

                        _user.AtualizarUsuario(usuarioNovaSenha, usuarioLogado, _encryptionService);
                        _unitOfWork.Commit();

                        // Update view model
                        UsuarioViewModel usuarioVM = Mapper.Map<Usuario, UsuarioViewModel>(_user);
                        response = request.CreateResponse(HttpStatusCode.OK, usuarioVM);
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.BadRequest, "Senha atual inválida.");
                    }

                }

                return response;
            });
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
                List<Usuario> usuarioAdm = null;
                int totalUsuarioAdm = new int();

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();

                    usuarioAdm = _usuarioRep.FindBy(c => c.UsuarioNome.ToLower().Contains(filter) && c.PerfilId == 1)
                        .OrderBy(c => c.UsuarioNome)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalUsuarioAdm = _usuarioRep
                        .GetAll().Where(c => c.PerfilId == 1)
                        .Count(c => c.UsuarioNome.ToLower().Contains(filter));
                }
                else
                {
                    usuarioAdm = _usuarioRep.GetAll().Where(c => c.PerfilId == 1)
                        .OrderBy(c => c.UsuarioNome)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                    .ToList();

                    totalUsuarioAdm = _usuarioRep.GetAll().Count(c => c.PerfilId == 1);
                }

                IEnumerable<UsuarioViewModel> usuarioVM = Mapper.Map<IEnumerable<Usuario>, IEnumerable<UsuarioViewModel>>(usuarioAdm);

                PaginacaoConfig<UsuarioViewModel> pagSet = new PaginacaoConfig<UsuarioViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalUsuarioAdm,
                    TotalPages = (int)Math.Ceiling((decimal)totalUsuarioAdm / currentPageSize),
                    Items = usuarioVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpPost]
        [Route("buscargrupo")]
        public HttpResponseMessage BuscarGrupo(HttpRequestMessage request, UsuarioViewModel novoUsuario)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response;

                ModelState.Remove("novoUsuario.Senha");
                ModelState.Remove("novoUsuario.ConfirmSenha");


                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                }
                else
                {

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    List<UsuarioGrupoViewModel> listaUsuarioGrupos = new List<UsuarioGrupoViewModel>();
                    UsuarioGrupoViewModel usuarioGrupoVM;

                    var Grupos = _grupoRep.GetAll().OrderBy(g => g.DescGrupo).ToList();
                    int idNovoUsuario = novoUsuario == null ? 0 : novoUsuario.Id;
                    var UsuarioGrupos = _usuarioGrupoRep.GetAll().Where(ug => ug.UsuarioId == idNovoUsuario).ToList();


                    foreach (Grupo item in Grupos)
                    {
                        usuarioGrupoVM = new UsuarioGrupoViewModel();
                        usuarioGrupoVM.GrupoId = item.Id;
                        usuarioGrupoVM.UsuarioId = idNovoUsuario;
                        usuarioGrupoVM.DescGrupo = item.DescGrupo;
                        usuarioGrupoVM.Relacionado = UsuarioGrupos.Count(u => u.GrupoId == item.Id) > 0 ? true : false;
                        usuarioGrupoVM.Selecionado = UsuarioGrupos.Count(u => u.GrupoId == item.Id) > 0 ? true : false;
                        listaUsuarioGrupos.Add(usuarioGrupoVM);
                    }

                    // Update view model
                    response = request.CreateResponse(HttpStatusCode.OK, listaUsuarioGrupos);

                }

                return response;
            });
        }

        [HttpPost]
        [Route("atualizargrupo/{UsuarioId:int}")]
        public HttpResponseMessage AtualizarGrupo(HttpRequestMessage request, int UsuarioId, IEnumerable<UsuarioGrupoViewModel> gruposXusuarios)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;



                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                }
                else
                {
                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var gruposdelete = _usuarioGrupoRep.GetAll().Where(x => x.UsuarioId == UsuarioId);

                    if (gruposdelete.Any())
                    {
                        _usuarioGrupoRep.DeleteAll(gruposdelete);
                        _unitOfWork.Commit();
                    }

                    if (gruposXusuarios.Any())
                    {
                        foreach (UsuarioGrupoViewModel item in gruposXusuarios)
                        {
                            if (item.Selecionado)
                            {
                                var novoUsuarioGrupo = new UsuarioGrupo
                                {
                                    UsuarioId = UsuarioId,
                                    DtCriacao = DateTime.Now,
                                    UsuarioCriacao = usuario,
                                    GrupoId = item.GrupoId
                                };
                                _usuarioGrupoRep.Add(novoUsuarioGrupo);
                                _unitOfWork.Commit();
                            }
                        }

                    }


                    response = request.CreateResponse(HttpStatusCode.OK);

                }

                return response;
            });
        }

        //Endereço do membro, utilizado no módulo de cliente

        [HttpPost]
        [Route("inserirendereco")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, EnderecoViewModel enderecoViewModel)
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
                    if (_enderecoRep.VerificaEnderecoJaCadastrado(enderecoViewModel.Cep, enderecoViewModel.PessoaId) > 0)
                    {
                        ModelState.AddModelError("CEP Existente", "CEP:" + enderecoViewModel.Cep + " já existe.");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                    }
                    else
                    {
                        Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                        enderecoViewModel.PessoaId = usuario.PessoaId;

                        Endereco novoEndereco = new Endereco()
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            PessoaId = enderecoViewModel.PessoaId,
                            EstadoId = enderecoViewModel.EstadoId,
                            CidadeId = enderecoViewModel.CidadeId,
                            BairroId = enderecoViewModel.BairroId,
                            LogradouroId = enderecoViewModel.LogradouroId,
                            Numero = enderecoViewModel.NumEndereco,
                            Complemento = enderecoViewModel.Complemento ?? string.Empty,
                            DescEndereco = enderecoViewModel.Endereco,
                            Cep = enderecoViewModel.Cep,
                            Ativo = enderecoViewModel.Ativo,
                            EnderecoPadrao = enderecoViewModel.EnderecoPadrao,
                            Referencia = enderecoViewModel.Referencia ?? string.Empty


                        };

                        _enderecoRep.Add(novoEndereco);
                        _unitOfWork.Commit();

                        foreach (var itemPeriodo in enderecoViewModel.PeriodoEntrega)
                        {

                            HorasEntregaMembro horarioEntrega = new HorasEntregaMembro()
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                PeriodoId = itemPeriodo.Id,
                                EnderecoId = novoEndereco.Id,
                                DescHorarioEntrega = enderecoViewModel.DescHorarioEntrega
                            };

                            _horarioEntregaMembroRep.Add(horarioEntrega);
                        }


                        _unitOfWork.Commit();

                        // Update view model
                        enderecoViewModel = Mapper.Map<Endereco, EnderecoViewModel>(novoEndereco);
                        response = request.CreateResponse(HttpStatusCode.Created, enderecoViewModel);

                    }
                }

                return response;
            });
        }


        [HttpPost]
        [Route("atualizarendereco")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, EnderecoViewModel enderecoViewModel)
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
                    Endereco _endereco = _enderecoRep.GetSingle(enderecoViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    _endereco.AtualizarEndereco(enderecoViewModel, usuario);

                    _unitOfWork.Commit();


                    // Update view model
                    enderecoViewModel = Mapper.Map<Endereco, EnderecoViewModel>(_endereco);
                    response = request.CreateResponse(HttpStatusCode.OK, enderecoViewModel);

                }

                return response;
            });
        }


        [HttpPost]
        [Route("atualizarEndPadrao")]
        public HttpResponseMessage AtualizarEndP(HttpRequestMessage request, EnderecoViewModel enderecoViewModel)
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
                    Endereco _endereco = _enderecoRep.GetSingle(enderecoViewModel.Id);

                    List<Endereco> ends = _enderecoRep.GetAll().Where(x => x.Pessoa.Id == _endereco.PessoaId).ToList();

                    ends.ForEach(a => a.EnderecoPadrao = false);

                    _unitOfWork.Commit();

                    _endereco.EnderecoPadrao = true;

                    _unitOfWork.Commit();


                    // Update view model
                    enderecoViewModel = Mapper.Map<Endereco, EnderecoViewModel>(_endereco);
                    response = request.CreateResponse(HttpStatusCode.OK, enderecoViewModel);

                }

                return response;
            });
        }


        [HttpGet]
        [Route("enderecoMembro/{filter?}")]
        public HttpResponseMessage GetEnderecoViaCep(HttpRequestMessage request, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = new Usuario();
                var pessoaId = 0;
                List<Endereco> listaEnderecos;

                if (!string.IsNullOrEmpty(filter))
                {
                    usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


                    filter = filter.Trim().ToLower();

                    listaEnderecos = _enderecoRep.FindBy(c => c.DescEndereco.ToLower().Contains(filter) ||
                    c.Cidade.DescCidade.ToLower().Contains(filter) ||
                    c.Complemento.ToLower().Contains(filter) ||
                    c.Cep.ToLower().Contains(filter) ||
                    c.Bairro.DescBairro.ToLower().Contains(filter)
                    && c.PessoaId == usuario.PessoaId
                    ).OrderByDescending(x => x.EnderecoPadrao).ThenByDescending(x => x.Ativo).ToList();

                }
                else
                {

                    usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    listaEnderecos = _enderecoRep.GetAll().Where(x => x.Pessoa.Id == usuario.PessoaId).OrderByDescending(x => x.EnderecoPadrao).ThenByDescending(x => x.Ativo).ToList();
                }

                var enderecosVM = Mapper.Map<IEnumerable<Endereco>, IEnumerable<EnderecoViewModel>>(listaEnderecos);

                var response = request.CreateResponse(HttpStatusCode.OK, enderecosVM);

                return response;
            });
        }

        [Route("atualizarTokenSignalR")]
        [HttpPost]
        public HttpResponseMessage AtualizarTokenSignalR(HttpRequestMessage request, UsuarioViewModel usuarioVm)
        {
            return CreateHttpResponse(request, () =>
            {


                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                usuario.TokenSignalR = usuarioVm.TokenSignalRUser;
                usuario.Logado = true;
                usuario.DtUsuarioEntrou = DateTime.Now;
                usuario.DtUsuarioSaiu = null;
                usuario.OrigemLogin = usuarioVm.OrigemLogin;
                _usuarioRep.Edit(usuario);

                _unitOfWork.Commit();

                var response = request.CreateResponse(HttpStatusCode.OK, new { success = true });

                return response;

            });
        }

        [HttpGet]
        [Route("termoUso")]
        public HttpResponseMessage TermoUso(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var termoUsoVM = Mapper.Map<TermoUso, TermoUsoViewModel>(usuario.TermoUso);
                var response = request.CreateResponse(HttpStatusCode.OK, termoUsoVM);

                return response;
            });
        }

        [HttpGet]
        [Route("verificaTermoUso")]
        public HttpResponseMessage VerificaTermoUso(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {




                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));


                TermoUso termoUso = null;

                //Verifica o perfil do usuário para exibir o termo de uso correto Membro, Fornecedor ou Franqueador
                switch (usuario.PerfilId)
                {

                    case 1:
                        termoUso = _termoUsoRep.GetAll().FirstOrDefault(x => x.Ativo && x.Tipo == TipoTermoUso.Membro);
                        break;
                    case 2:
                        termoUso = _termoUsoRep.GetAll().FirstOrDefault(x => x.Ativo && x.Tipo == TipoTermoUso.Fornecedor);
                        break;
                    default:
                        termoUso = _termoUsoRep.GetAll().FirstOrDefault(x => x.Ativo && x.Tipo == TipoTermoUso.Franquia);
                        break;
                }


                var aceite = false;
                TermoUsoViewModel termoUsoVM = null;

                if (termoUso != null)
                {
                    aceite = !usuario.AceitoTermoUso || !termoUso.Id.Equals(usuario.TermoUsoId);
                    if (aceite)
                    {
                        if (usuario.TermoUsoId != null)
                        {
                            usuario.TermoUsoId = null;
                            usuario.AceitoTermoUso = false;
                            usuario.DtLeituraTermoUso = null;
                            _unitOfWork.Commit();
                        }

                        termoUsoVM = Mapper.Map<TermoUso, TermoUsoViewModel>(termoUso);
                    }
                }

                var result = new
                {
                    ExibirAceite = aceite,
                    TermoUso = termoUsoVM
                };

                var response = request.CreateResponse(HttpStatusCode.OK, result);

                return response;
            });
        }

        [HttpGet]
        [Route("aceitoTermoUso")]
        public HttpResponseMessage AceitoTermoUso(HttpRequestMessage request, int termoUsoId)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                usuario.TermoUsoId = termoUsoId;
                usuario.AceitoTermoUso = true;
                usuario.DtLeituraTermoUso = DateTime.Now;
                _unitOfWork.Commit();

                var response = request.CreateResponse(HttpStatusCode.OK);

                return response;
            });
        }

        [Route("cancelarAssinatura")]
        [HttpPost]
        public HttpResponseMessage CancelarAssinatura(HttpRequestMessage request)
        {

            return CreateHttpResponse(request, () =>
            {

                if (!ModelState.IsValid)
                {
                    var response = request.CreateResponse(HttpStatusCode.BadRequest,
                          ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                .Select(m => m.ErrorMessage).ToArray());
                    return response;
                }
                else
                {
                    HttpResponseMessage response;

                    var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    var fornecedor = _fornecedorRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                    var membro = _membroRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                    if (fornecedor != null)
                    {
                        // Atualizar usuário como inativo caso não se tenha fatura em aberto
                        // No Client deve-se verificar se voltou faturas, se sim mostrar pop'up falando que assinatura
                        // só será cancelada quando se quitar as dívidas, direcionando usuário para tela de faturas para o mesmo imprimir o boleto.
                        // Quando se pagar a dívida, na notificação enviada pela GerenciaNet deve-se verificar 
                        // a tabela de usuário cancelado e mudar o campo ativo do usuário e membro para false.

                        var usuarioCanceladoFornecedor = _usuarioCanceladoRep.FirstOrDefault(u => u.Usuario.PessoaId == fornecedor.PessoaId) != null;

                        if (usuario.FlgMaster)
                        {
                            if (!usuarioCanceladoFornecedor)
                            {
                                var faturas = _faturaRep
                                        .GetAll().Where(x => x.FornecedorId == fornecedor.Id &&
                                                               (x.Status != StatusFatura.Recebido && x.Status != StatusFatura.Link &&
                                                                x.Status != StatusFatura.Cancelado && x.Status != StatusFatura.Devolvido)).ToList();

                                if (faturas.Count == 0)
                                {

                                    var _usuariosDesativados = _usuarioRep.GetAll().Where(x => x.PessoaId == usuario.PessoaId).ToList();

                                    for (int i = 0; i < _usuariosDesativados.Count; i++)
                                    {
                                        _usuariosDesativados[i].Ativo = false;
                                        _usuarioRep.Edit(_usuariosDesativados[i]);
                                    }

                                    fornecedor.Ativo = false;
                                    _fornecedorRep.Edit(fornecedor);

                                    _usuarioCanceladoRep.Add(new UsuarioCancelado
                                    {
                                        UsuarioId = usuario.Id,
                                        DataCancelamento = DateTime.Now,
                                        UsuarioCriacaoId = usuario.Id,
                                        DtCriacao = DateTime.Now,
                                        Ativo = true
                                    });

                                    _unitOfWork.Commit();

                                    var faturasVM = Mapper.Map<IEnumerable<Fatura>, IEnumerable<FaturaViewModel>>(faturas);

                                    response = request.CreateResponse(HttpStatusCode.OK, faturasVM);

                                    return response;

                                }
                                else
                                {
                                    var faturasVM = Mapper.Map<IEnumerable<Fatura>, IEnumerable<FaturaViewModel>>(faturas);

                                    response = request.CreateResponse(HttpStatusCode.OK, faturasVM);

                                    return response;
                                }
                            }

                        }
                        else
                        {
                            var usuarioMaster = _usuarioRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId && !x.FlgMaster);

                            response = request.CreateResponse(HttpStatusCode.OK, "Somente o usuário " + usuarioMaster.UsuarioNome + " pode cancelar a assinatura, pois o mesmo é usuário Master!");

                            return response;

                        }

                    }
                    else if (membro != null)
                    {
                        // Inserir na tabela de usuário cancelado para se ter histórico de cancelamento, caso
                        // membro esteja em dia e esteja no mês corrente mostrar mensagem no client que a assinatura estara ativa até o dia anterior ao fechar a mensalidade para próximo mês.
                        // Colocar data na mensagem do client Ex: (sua assinatura estara ativa até o dia tal, somente para consultar de pedidos)
                        //colocar data de desabilitação para robô passar e mudar usuário para inativo
                        // na data estipulada no campo desta tabela.

                        // No Client deve-se verificar se voltou mensalidades, se sim mostrar pop'up falando que assinatura
                        // só será cancelada quando se quitar as dívidas, direcionando usuário para tela de mensalidades para o mesmo imprimir o boleto.
                        // Quando se pagar a dívida, na notificação enviada pela GerenciaNet deve-se verificar 
                        // a tabela de usuário cancelado e mudar o campo ativo do usuário e membro para false.

                        var usuarioCanceladoMembro = _usuarioCanceladoRep.FirstOrDefault(u => u.Usuario.PessoaId == membro.PessoaId) != null;

                        if (!usuarioCanceladoMembro)
                        {
                            if (usuario.FlgMaster)
                            {
                                var mens = _mensalidadeRep.FindBy(x => x.Membro.PessoaId == usuario.PessoaId &&
                                                                       x.Status == StatusMensalidade.Recebido ||
                                                                       x.Status == StatusMensalidade.AguardandoPagamento)
                                                          .OrderByDescending(x => x.Id)
                                                          .FirstOrDefault();


                                if (mens != null && mens.Status == StatusMensalidade.Recebido)
                                {
                                    // Cancelar Plano
                                    _pagamentoService.CancelarPlano(mens.Id);

                                    var dataFimAcesso = mens.DtRecebimento != null ?
                                        mens.DtRecebimento.Value.AddMonths(membro.PlanoMensalidade.QtdMeses) : DateTime.Now;

                                    var _usuariosDesativados = _usuarioRep.FindBy(x => x.PessoaId == usuario.PessoaId).ToList();

                                    if (dataFimAcesso < DateTime.Now)
                                    {
                                        for (int i = 0; i < _usuariosDesativados.Count; i++)
                                        {
                                            _usuariosDesativados[i].Ativo = false;
                                            _usuarioRep.Edit(_usuariosDesativados[i]);
                                        }

                                        membro.Ativo = false;
                                        _membroRep.Edit(membro);
                                    }


                                    _usuarioCanceladoRep.Add(new UsuarioCancelado
                                    {
                                        UsuarioId = usuario.Id,
                                        DataCancelamento = dataFimAcesso,
                                        UsuarioCriacaoId = usuario.Id,
                                        DtCriacao = DateTime.Now,
                                        Ativo = true
                                    });

                                    mens.Ativo = false;
                                    _mensalidadeRep.Edit(mens);

                                    _unitOfWork.Commit();

                                    response = request.CreateResponse(HttpStatusCode.OK,
                                        $"Você ainda terá até o dia {dataFimAcesso.ToShortDateString()} para acessar a plataforma, depois deste prazo sua conta será desativada automaticamente.");

                                    return response;


                                }
                                else
                                {
                                    if (!usuarioCanceladoMembro)
                                    {
                                        // Cancelar plano
                                        if (mens != null)
                                            _pagamentoService.CancelarPlano(mens.Id);

                                        var _usuariosDesativados = _usuarioRep.FindBy(x => x.PessoaId == usuario.PessoaId).ToList();

                                        for (int i = 0; i < _usuariosDesativados.Count; i++)
                                        {
                                            _usuariosDesativados[i].Ativo = false;
                                            _usuarioRep.Edit(_usuariosDesativados[i]);
                                        }

                                        membro.Ativo = false;
                                        _membroRep.Edit(membro);

                                        _usuarioCanceladoRep.Add(new UsuarioCancelado
                                        {
                                            UsuarioId = usuario.Id,
                                            DataCancelamento = DateTime.Now,
                                            UsuarioCriacaoId = usuario.Id,
                                            DtCriacao = DateTime.Now,
                                            Ativo = true
                                        });

                                        _unitOfWork.Commit();

                                        response = request.CreateResponse(HttpStatusCode.Created, "Assinatura cancelada com sucesso!");

                                        return response;
                                    }

                                }

                            }
                            else
                            {
                                var usuarioMaster = _usuarioRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId && x.FlgMaster);

                                response = request.CreateResponse(HttpStatusCode.OK, "Somente o usuário ''" + usuarioMaster.UsuarioNome + "'' poderá cancelar a assinatura, pois o mesmo é usuário Master!");

                                return response;
                            }
                        }

                    }

                    response = request.CreateResponse(HttpStatusCode.NoContent);

                    return response;
                }
            });
        }
    }
}
