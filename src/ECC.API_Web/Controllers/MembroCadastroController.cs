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

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/membroCadastro")]
    public class MembroCadastroController : ApiControllerBase
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
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IEntidadeBaseRep<AvaliacaoFornecedor> _avaliacaofornecedorRep;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;


        public MembroCadastroController(
            IUtilService utilService,
            IEntidadeBaseRep<TemplateEmail> templateEmailRep,
            IEmailService emailService,
            ISmsService smsService,
            IEntidadeBaseRep<TemplateSms> templateSmsRep,
            IEntidadeBaseRep<Sms> smsRep,
              IEntidadeBaseRep<Emails> emailsRep,

            
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
            _emailsRep = emailsRep;
            _avaliacaofornecedorRep = avaliacaofornecedor;
            _avisosRep = avisosRep;
            _fornecedorPrazoSemanalRep = fornecedorPrazoSemanalRep;
        }

        [AllowAnonymous]
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
                        var usuario = _usuarioRep.GetSingle(1);

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
                            DataFimPeriodoGratuito=DateTime.Now.AddDays(30)

                        };
                        _membroRep.Add(novoMembro);

                        _unitOfWork.Commit();


                        var email = new Emails
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Novo Membro Cliente PF",
                            EmailDestinatario = "renaisanci@gmail.com",
                            CorpoEmail = "NOVO CLIENTE",
                            Status = Status.NaoEnviado,
                            Origem = 0,
                            Ativo = true

                        };
                        _emailsRep.Add(email);
                        _unitOfWork.Commit();
                        // Update view model
                        membroPFViewModel = Mapper.Map<Membro, MembroPFViewModel>(novoMembro);
                        response = request.CreateResponse(HttpStatusCode.Created, membroPFViewModel);
                    }
                }
                return response;
            });
        }

        [AllowAnonymous]
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
                        var usuario = _usuarioRep.GetSingle(1);

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
                            DataFimPeriodoGratuito = DateTime.Now.AddDays(30)
                        };
                        _membroRep.Add(novoMembro);

                        _unitOfWork.Commit();


                        var email = new Emails
                        {
                            UsuarioCriacao = usuario,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Novo Membro Cliente PJ",
                            EmailDestinatario = "renaisanci@gmail.com",
                            CorpoEmail = "NOVO CLIENTE",
                            Status = Status.NaoEnviado,
                            Origem = 0,
                            Ativo = true

                        };
                        _emailsRep.Add(email);
                        _unitOfWork.Commit();

                        // Update view model
                        membroViewModel = Mapper.Map<Membro, MembroViewModel>(novoMembro);
                        response = request.CreateResponse(HttpStatusCode.Created, membroViewModel);
                    }
                }

                return response;
            });
        }

        [AllowAnonymous]
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

                    var usuario = _usuarioRep.GetSingle(1);
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

        [AllowAnonymous]
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

                    var usuario = _usuarioRep.GetSingle(1);
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

        [AllowAnonymous]
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

        [AllowAnonymous]
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

                    var usuario = _usuarioRep.GetSingle(1);
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

    }
}
