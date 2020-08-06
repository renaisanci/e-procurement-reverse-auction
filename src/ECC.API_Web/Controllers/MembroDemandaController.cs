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
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.EntidadeEmail;
using ECC.EntidadeProduto;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeSms;
using ECC.Servicos.Abstrato;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro")]
    [RoutePrefix("api/membroDemanda")]
    public class MembroDemandaController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Pessoa> _pessoaRep;
        private readonly IEntidadeBaseRep<PessoaJuridica> _pessoaJuridicaRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<MembroDemanda> _membroDemandaRep;
        private readonly IEmailService _emailService;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        private readonly ISmsService _smsService;
        private readonly IEntidadeBaseRep<TemplateSms> _templateSmsRep;
        private readonly IEntidadeBaseRep<Sms> _smsRep;
        private readonly IUtilService _utilService;

        public MembroDemandaController(IUtilService utilService, IEntidadeBaseRep<TemplateEmail> templateEmailRep, IEmailService emailService, ISmsService smsService, IEntidadeBaseRep<TemplateSms> templateSmsRep, IEntidadeBaseRep<Sms> smsRep, IEntidadeBaseRep<MembroDemanda> membroDemandaRep, IEntidadeBaseRep<MembroCategoria> membroCategoriaRep, IEntidadeBaseRep<Usuario> usuarioRep, IEntidadeBaseRep<Pessoa> pessoaRep, IEntidadeBaseRep<PessoaJuridica> pessoaJuridicaRep, IEntidadeBaseRep<Membro> membroRep, IEntidadeBaseRep<Erro> _errosRepository, IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _pessoaRep = pessoaRep;
            _pessoaJuridicaRep = pessoaJuridicaRep;
            _membroRep = membroRep;
            _usuarioRep = usuarioRep;
            _membroDemandaRep = membroDemandaRep;
            _membroCategoriaRep = membroCategoriaRep;
            _emailService = emailService;
            _templateEmailRep = templateEmailRep;
            _smsService = smsService;
            _templateSmsRep = templateSmsRep;
            _smsRep = smsRep;
            _utilService = utilService;
        }

        [HttpGet]
        [Route("pesquisar/{page:int=0}/{pageSize=4}/{membroId:int=0}")]
        public HttpResponseMessage GetByCategoriaId(HttpRequestMessage request, int? page, int? pageSize, int membroId)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<MembroDemanda> demandas = null;
                int totalMembros = new int();

                if (membroId != 0)
                {
                    demandas = _membroDemandaRep.FindBy(c => c.MembroId.Equals(membroId))
                        .OrderBy(c => c.SubCategoria.DescSubCategoria)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalMembros = _membroDemandaRep
                        .GetAll()
                        .Count(c => c.MembroId.Equals(membroId));
                }
                //else
                //{
                //    demandas = _membroDemandaRep.GetAll()
                //        .OrderBy(c => c.SubCategoria.DescSubCategoria)
                //        .Skip(currentPage * currentPageSize)
                //        .Take(currentPageSize)
                //    .ToList();

                //    totalMembros = _membroRep.GetAll().Count();
                //}

                IEnumerable<MembroDemandaViewModel> membroDemandasVM = Mapper.Map<IEnumerable<MembroDemanda>, IEnumerable<MembroDemandaViewModel>>(demandas);

                var pagSet = new PaginacaoConfig<MembroDemandaViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalMembros,
                    TotalPages = (int)Math.Ceiling((decimal)totalMembros / currentPageSize),
                    Items = membroDemandasVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, MembroDemandaViewModel membroDemandaViewModel)
        {

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (ModelState.IsValid)
                {

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                    MembroDemanda novoMembroDemanda = new MembroDemanda()
                    {
                        MembroId = membroDemandaViewModel.MembroId,
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        PeriodicidadeId = membroDemandaViewModel.PeriodicidadeId,
                        UnidadeMedidaId = membroDemandaViewModel.UnidadeMedidaId,
                        SubCategoriaId = membroDemandaViewModel.SubCategoriaId,
                        Quantidade = membroDemandaViewModel.Quantidade,
                        Observacao = membroDemandaViewModel.Observacao,
                        Ativo = membroDemandaViewModel.Ativo

                    };
                    _membroDemandaRep.Add(novoMembroDemanda);

                    _unitOfWork.Commit();

                    // Update view model
                    membroDemandaViewModel = Mapper.Map<MembroDemanda, MembroDemandaViewModel>(novoMembroDemanda);
                    response = request.CreateResponse(HttpStatusCode.Created, membroDemandaViewModel);
                }
                else
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }

                return response;
            });
        }


        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, MembroDemandaViewModel membroDemandaViewModel)
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
                    MembroDemanda _membroDemanda = _membroDemandaRep.GetSingle(membroDemandaViewModel.Id);

                    Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    _membroDemanda.AtualizarMembroDemanda(membroDemandaViewModel, usuario);

                    _unitOfWork.Commit();

                    // Update view model
                    membroDemandaViewModel = Mapper.Map<MembroDemanda, MembroDemandaViewModel>(_membroDemanda);
                    response = request.CreateResponse(HttpStatusCode.OK, membroDemandaViewModel);

                    //TODO: Verificar se deve enviar e-mail ao cadastrar demanda.
                    //_utilService.MembroInserirUsuarioEnviarEmail(membroDemandaViewModel.Id, usuario.Id);
                }

                return response;
            });
        }

    }
}
