using System;
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
using System.Collections.Generic;
using ECC.Dados.Extensions;
using ECC.EntidadeAvisos;
using ECC.EntidadeEndereco;
using ECC.EntidadeFranquia;
using ECC.EntidadeParametroSistema;
using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.Servicos.Abstrato;
using ECC.EntidadeEmail;
using ECC.EntidadePedido;
using ECC.Servicos;
using System.Data.Entity.SqlServer;

namespace ECC.API_Web.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro, Fornecedor, Franquia")]
    [RoutePrefix("api/franquia")]
    public class FranquiaController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Franquia> _franquiaRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<DataCotacaoFranquia> _dataCotacaoFranquiaRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IEntidadeBaseRep<FranquiaProduto> _franquiaProdutoRep;
        private readonly IEntidadeBaseRep<FranquiaFornecedor> _franquiaFornecedorRep;
        private readonly IEntidadeBaseRep<Pessoa> _pessoaRep;
        private readonly IEntidadeBaseRep<PessoaJuridica> _pessoaJuridicaRep;
        private readonly IEntidadeBaseRep<Telefone> _telefoneRep;
        private readonly IEntidadeBaseRep<Endereco> _enderecoRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedorRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly INotificacoesAlertasService _notificacoesAlertaServiceRep;
        private readonly IUtilService _utilServiceRep;
        private readonly ICalendarioFeriadoService _calendarioFeriadoServiceRep;

        public FranquiaController(
                IEntidadeBaseRep<Franquia> franquiaRep,
                IEntidadeBaseRep<Membro> membroRep,
                IEntidadeBaseRep<Usuario> usuarioRep,
                IEntidadeBaseRep<DataCotacaoFranquia> dataCotacaoFranquiaRep,
                IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
                IEntidadeBaseRep<FranquiaProduto> franquiaProdutoRep,
                IEntidadeBaseRep<FranquiaFornecedor> franquiaFornecedorRep,
                IEntidadeBaseRep<Pessoa> pessoaRep,
                IEntidadeBaseRep<PessoaJuridica> pessoaJuridicaRep,
                IEntidadeBaseRep<Telefone> telefoneRep,
                IEntidadeBaseRep<Endereco> enderecoRep,
                IUtilService utilServiceRep,
                IEntidadeBaseRep<Fornecedor> fornecedorRep,
                IEntidadeBaseRep<MembroFornecedor> membroFornecedorRep,
                IEntidadeBaseRep<TemplateEmail> templateEmailRep,
                IEntidadeBaseRep<Emails> emailsRep,
                INotificacoesAlertasService notificacoesAlertaServiceRep,
                ICalendarioFeriadoService calendarioFeriadoServiceRep,
                IEntidadeBaseRep<Erro> _errosRepository,
                IUnitOfWork _unitOfWork)
            : base(usuarioRep, _errosRepository, _unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _membroRep = membroRep;
            _franquiaRep = franquiaRep;
            _dataCotacaoFranquiaRep = dataCotacaoFranquiaRep;
            _parametroSistemaRep = parametroSistemaRep;
            _franquiaProdutoRep = franquiaProdutoRep;
            _franquiaFornecedorRep = franquiaFornecedorRep;
            _pessoaRep = pessoaRep;
            _pessoaJuridicaRep = pessoaJuridicaRep;
            _telefoneRep = telefoneRep;
            _enderecoRep = enderecoRep;
            _utilServiceRep = utilServiceRep;
            _fornecedorRep = fornecedorRep;
            _membroFornecedorRep = membroFornecedorRep;
            _notificacoesAlertaServiceRep = notificacoesAlertaServiceRep;
            _templateEmailRep = templateEmailRep;
            _emailsRep = emailsRep;
            _calendarioFeriadoServiceRep = calendarioFeriadoServiceRep;
        }

        [HttpGet]
        [Route("getFranquias/{page:int=0}/{pageSize=4}/{filter?}")]
        public HttpResponseMessage GetFranquias(HttpRequestMessage request, int? page, int? pageSize, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalFranquias = new int();
                var franquias = new List<Franquia>();

                if (!string.IsNullOrEmpty(filter))
                {
                    franquias = _franquiaRep.FindBy(c => c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter))
                       .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                       .Skip(currentPage * currentPageSize)
                       .Take(currentPageSize)
                       .ToList();

                    totalFranquias = _franquiaRep.GetAll()
                        .Count(c => c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter));
                }
                else
                {
                    franquias = _franquiaRep.GetAll()
                        .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                        .Skip(currentPage * currentPageSize)
                        .Take(currentPageSize)
                        .ToList();

                    totalFranquias = _franquiaRep.GetAll().Count();
                }

                IEnumerable<FranquiaViewModel> franquiasVM = Mapper.Map<IEnumerable<Franquia>, IEnumerable<FranquiaViewModel>>(franquias);

                PaginacaoConfig<FranquiaViewModel> pagSet = new PaginacaoConfig<FranquiaViewModel>
                {
                    Page = currentPage,
                    TotalCount = totalFranquias,
                    TotalPages = (int)Math.Ceiling((decimal)totalFranquias / currentPageSize),
                    Items = franquiasVM
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [HttpPost]
        [Route("inserirfranquia")]
        public HttpResponseMessage InserirFranquia(HttpRequestMessage request, FranquiaViewModel franquiaViewModel)
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

                    var usuarioEmail = _usuarioRep.GetSingleByEmail(franquiaViewModel.Email, 4);

                    if (usuarioEmail != null)
                    {
                        ModelState.AddModelError("Email Já Existe", "E-mail:" + franquiaViewModel.Email + " já existe");
                        response = request.CreateResponse(HttpStatusCode.BadRequest,
                            ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                .Select(m => m.ErrorMessage).ToArray());


                    }
                    else
                    {

                        if (_franquiaRep.CnpjExistente(franquiaViewModel.Cnpj) > 0)
                        {
                            ModelState.AddModelError("CNPJ Existente", "CNPJ:" + franquiaViewModel.Cnpj + " já existe .");
                            response = request.CreateResponse(HttpStatusCode.BadRequest,
                                ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                                    .Select(m => m.ErrorMessage).ToArray());
                        }
                        else
                        {




                            Usuario usuario =
                                _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                            var pessoa = new Pessoa
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                TipoPessoa = TipoPessoa.PessoaJuridica,
                                Ativo = true
                            };
                            _pessoaRep.Add(pessoa);
                            _unitOfWork.Commit();

                            var pessoaJuridica = new PessoaJuridica
                            {
                                UsuarioCriacaoId = usuario.Id,
                                DtCriacao = DateTime.Now,
                                PessoaId = pessoa.Id,
                                NomeFantasia = franquiaViewModel.NomeFantasia,
                                Cnpj = franquiaViewModel.Cnpj,
                                RazaoSocial = franquiaViewModel.RazaoSocial,
                                InscEstadual = franquiaViewModel.InscEstadual,
                                DtFundacao = franquiaViewModel.DtFundacao,
                                Email = franquiaViewModel.Email,
                                Ativo = true

                            };
                            pessoa.PessoaJuridicaId = pessoaJuridica.Id;

                            _pessoaJuridicaRep.Add(pessoaJuridica);
                            _pessoaRep.Edit(pessoa);
                            _unitOfWork.Commit();

                            pessoa.PessoaJuridicaId = pessoaJuridica.Id;
                            _pessoaRep.Edit(pessoa);

                            var franquia = new Franquia
                            {
                                UsuarioCriacaoId = usuario.Id,
                                DtCriacao = DateTime.Now,
                                PessoaId = pessoa.Id,
                                Responsavel = franquiaViewModel.Responsavel,
                                Descricao = franquiaViewModel.Descricao,
                                DataCotacao = franquiaViewModel.DataCotacao,
                                Ativo = true
                            };
                            _franquiaRep.Add(franquia);

                            var endereco = new Endereco
                            {
                                UsuarioCriacaoId = usuario.Id,
                                DtCriacao = DateTime.Now,
                                PessoaId = pessoa.Id,
                                EstadoId = franquiaViewModel.Endereco.EstadoId,
                                CidadeId = franquiaViewModel.Endereco.CidadeId,
                                BairroId = franquiaViewModel.Endereco.BairroId,
                                LogradouroId = franquiaViewModel.Endereco.LogradouroId,
                                Numero = franquiaViewModel.Endereco.NumEndereco,
                                Complemento = franquiaViewModel.Endereco.Complemento,
                                DescEndereco = franquiaViewModel.Endereco.Endereco,
                                Cep = franquiaViewModel.Endereco.Cep,
                                EnderecoPadrao = true,
                                Referencia = franquiaViewModel.Endereco.Referencia,
                                Ativo = true
                            };
                            _enderecoRep.Add(endereco);

                            var telefone = new Telefone
                            {
                                UsuarioCriacaoId = usuario.Id,
                                DtCriacao = DateTime.Now,
                                PessoaId = pessoa.Id,
                                DddTelComl = franquiaViewModel.DddTelComl,
                                TelefoneComl = franquiaViewModel.TelefoneComl,
                                DddCel = franquiaViewModel.DddCel,
                                Celular = franquiaViewModel.Celular,
                                Contato = franquiaViewModel.Contato,
                                Ativo = true
                            };
                            _unitOfWork.Commit();

                            //Cria Usuário para ADM Franquia e Envia Email
                            _utilServiceRep.FranquiaInserirUsuarioEnviarEmail(franquia.Id, usuario, telefone);

                            var franquiaVm = Mapper.Map<Franquia, FranquiaViewModel>(franquia);

                            response = request.CreateResponse(HttpStatusCode.OK, franquiaVm);
                        }
                    }
                }

                return response;
            });
        }

        [HttpPost]
        [Route("atualizarfranquia")]
        public HttpResponseMessage AtualizarFranquia(HttpRequestMessage request, FranquiaViewModel franquiaViewModel)
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
                    var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == franquiaViewModel.PessoaId);
                    var telefone = _telefoneRep.FirstOrDefault(x => x.PessoaId == franquia.PessoaId);
                    var endereco = franquia.Pessoa.Enderecos.FirstOrDefault();

                    franquia.UsuarioAlteracaoId = usuario.Id;
                    franquia.DtAlteracao = DateTime.Now;
                    franquia.Pessoa.PessoaJuridica.NomeFantasia = franquiaViewModel.NomeFantasia;
                    franquia.Pessoa.PessoaJuridica.RazaoSocial = franquiaViewModel.RazaoSocial;
                    franquia.Pessoa.PessoaJuridica.DtFundacao = franquiaViewModel.DtFundacao;
                    franquia.Pessoa.PessoaJuridica.Email = franquiaViewModel.Email;
                    franquia.Pessoa.PessoaJuridica.Cnpj = franquiaViewModel.Cnpj;
                    franquia.Descricao = franquiaViewModel.Descricao;
                    franquia.Responsavel = franquiaViewModel.Responsavel;
                    franquia.DataCotacao = franquiaViewModel.DataCotacao;
                    franquia.Ativo = franquiaViewModel.Ativo;

                    if (endereco != null)
                    {
                        endereco.UsuarioAlteracaoId = usuario.Id;
                        endereco.DtAlteracao = DateTime.Now;
                        endereco.EstadoId = franquiaViewModel.Endereco.EstadoId;
                        endereco.CidadeId = franquiaViewModel.Endereco.CidadeId;
                        endereco.BairroId = franquiaViewModel.Endereco.BairroId;
                        endereco.LogradouroId = franquiaViewModel.Endereco.LogradouroId;
                        endereco.Numero = franquiaViewModel.Endereco.NumEndereco;
                        endereco.Complemento = franquiaViewModel.Endereco.Complemento;
                        endereco.DescEndereco = franquiaViewModel.Endereco.Endereco;
                        endereco.Cep = franquiaViewModel.Endereco.Cep;
                        endereco.EnderecoPadrao = true;
                        endereco.Referencia = franquiaViewModel.Endereco.Referencia;
                        endereco.Referencia = franquiaViewModel.Endereco.Referencia;
                        endereco.Ativo = true;

                        _enderecoRep.Edit(endereco);
                    }

                    if (telefone != null)
                    {
                        telefone.UsuarioAlteracaoId = usuario.Id;
                        telefone.DtAlteracao = DateTime.Now;
                        telefone.DddTelComl = franquiaViewModel.DddTelComl;
                        telefone.TelefoneComl = franquiaViewModel.TelefoneComl;
                        telefone.DddCel = franquiaViewModel.DddCel;
                        telefone.Celular = franquiaViewModel.Celular;
                        telefone.Contato = franquiaViewModel.Contato;

                        _telefoneRep.Edit(telefone);
                    }

                    _franquiaRep.Edit(franquia);

                    _unitOfWork.Commit();

                    var FranquiaVM = Mapper.Map<Franquia, FranquiaViewModel>(franquia);

                    response = request.CreateResponse(HttpStatusCode.OK, FranquiaVM);
                }

                return response;
            });
        }

        [HttpGet]
        [Route("carregaFranquiasMembro")]
        public HttpResponseMessage GetFranquiaMembro(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var franquiaMembros = _franquiaRep.GetAll().ToList();

                IEnumerable<FranquiaViewModel> franquiaMembroVM = Mapper.Map<IEnumerable<Franquia>, IEnumerable<FranquiaViewModel>>(franquiaMembros);

                response = request.CreateResponse(HttpStatusCode.OK, franquiaMembroVM);

                return response;
            });
        }

        [HttpGet]
        [Route("carregaMembrosFranquia/{page:int=0}/{pageSize=4}/{cidadeId:int=0}/{filter?}")]
        public HttpResponseMessage GetMembrosFranquia(HttpRequestMessage request, int? page, int? pageSize, int? cidadeId, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalMembros = new int();
                var membros = new List<Membro>();
                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();
                    var strinFilter = '%' + filter.Replace(' ', '%') + '%';

                    membros = _membroRep.FindBy(x => x.FranquiaId.Value == franquia.Id &&
                      SqlFunctions.PatIndex(strinFilter, x.Pessoa.PessoaJuridica.NomeFantasia.ToLower()) > 0 &&
                      //x.Pessoa.PessoaJuridica.NomeFantasia.Contains(filter) &&
                      x.Pessoa.Enderecos.Any(e => e.CidadeId == cidadeId))
                     .OrderBy(c => c.Pessoa.PessoaJuridica.NomeFantasia)
                     .Skip(currentPage * currentPageSize)
                     .Take(currentPageSize)
                     .ToList();

                    totalMembros = _membroRep.GetAll()
                    .Count(x => x.FranquiaId.Value == franquia.Id &&
                                x.Pessoa.PessoaJuridica.NomeFantasia.Contains(filter));
                }
                else
                {
                    if (cidadeId > 0)
                    {
                        membros = _membroRep.FindBy(x => x.FranquiaId.Value == franquia.Id && x.Pessoa.Enderecos.Any(c => c.CidadeId == cidadeId))
                                 .OrderBy(c => c.Pessoa.PessoaJuridica.NomeFantasia)
                                 .Skip(currentPage * currentPageSize)
                                 .Take(currentPageSize)
                                 .ToList();

                        totalMembros = _membroRep.GetAll()
                        .Count(x => x.FranquiaId.Value == franquia.Id && x.Pessoa.Enderecos.Any(c => c.CidadeId == cidadeId));
                    }
                    else
                    {
                        membros = _membroRep.FindBy(x => x.FranquiaId.Value == franquia.Id)
                                 .OrderBy(c => c.Pessoa.PessoaJuridica.NomeFantasia)
                                 .Skip(currentPage * currentPageSize)
                                 .Take(currentPageSize)
                                 .ToList();

                        totalMembros = _membroRep.GetAll().Count(x => x.FranquiaId.Value == franquia.Id);
                    }


                }



                IEnumerable<MembroViewModel> membrosVM = Mapper.Map<IEnumerable<Membro>, IEnumerable<MembroViewModel>>(membros);

                PaginacaoConfig<MembroViewModel> pagSet = new PaginacaoConfig<MembroViewModel>()
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
        [Route("perfil")]
        public HttpResponseMessage GetPerfil(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId.Equals(usuario.PessoaId));
                var fornecedorVm = Mapper.Map<Franquia, FranquiaViewModel>(franquia);
                var response = request.CreateResponse(HttpStatusCode.OK, fornecedorVm);

                return response;
            });
        }

        [HttpGet]
        [Route("carregaDataCotacaoFranquia")]
        public HttpResponseMessage GetDataCotacaoFranquia(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var datasCotacoes = new List<DateTime>();

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var membro = _membroRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);
                var horaCotacao = Convert.ToDateTime(_parametroSistemaRep.FirstOrDefault(x => x.Codigo == "HORA_COTACAO").Valor).TimeOfDay;
                var date = DateTime.Now;
                var franquiaDiasCotacao = new List<DataCotacaoFranquia>();
                var dataCotacao = new DateTime();


                if (membro.FranquiaId != null)
                {

                    //Retorna feriado caso exista.
                    var diasFeriados = _calendarioFeriadoServiceRep.VerificaFeriadoMembro(date, membro,
                        membro.Pessoa.Enderecos?.FirstOrDefault(x => x.EnderecoPadrao).Estado.DescEstado);

                    franquiaDiasCotacao =
                    _dataCotacaoFranquiaRep.GetAll().Where(x => x.FranquiaId == membro.FranquiaId).ToList();

                    if (franquiaDiasCotacao.Any())
                    {
                        var diasSemana = new List<DayOfWeek>();
                        franquiaDiasCotacao.ForEach(x => diasSemana.Add((DayOfWeek)x.DiaSemana));

                        var dataHoje = DateTime.Now;

                        while (datasCotacoes.Count < diasSemana.Count)
                        {
                            foreach (var dias in diasSemana)
                            {
                                if (date.DayOfWeek == dias)
                                {
                                    var dataNova = new DateTime(date.Year, date.Month, date.Day, horaCotacao.Hours, horaCotacao.Minutes, horaCotacao.Seconds);
                                    var somenteData = new DateTime(date.Year, date.Month, date.Day);
                                    var feriado = new DateTime();

                                    //Retorna feriado caso exista.
                                    diasFeriados = _calendarioFeriadoServiceRep.VerificaFeriadoMembro(dataNova, membro,
                                        membro.Pessoa.Enderecos.FirstOrDefault(x => x.EnderecoPadrao).Estado.DescEstado);

                                    if (diasFeriados.CalendarioFeriado != null &&
                                    diasFeriados.TipoRetornoFeriado == Servicos.TipoRetornoFeriado.NaoExisteFeriado &&
                                    date.TimeOfDay < horaCotacao)
                                    {
                                        var dataFeriado = diasFeriados.CalendarioFeriado.DtEvento
                                        .AddHours(horaCotacao.Hours)
                                        .AddMinutes(horaCotacao.Minutes)
                                        .AddSeconds(horaCotacao.Seconds);
                                        dataNova = dataFeriado;

                                    }


                                    if (diasFeriados.CalendarioFeriado != null)
                                    {
                                        feriado = diasFeriados.CalendarioFeriado.DtEvento;
                                        if (dataHoje < dataNova && somenteData != feriado)
                                        {
                                            datasCotacoes.Add(dataNova);
                                        }

                                    }
                                    else if (dataHoje < dataNova)
                                    {
                                        datasCotacoes.Add(dataNova);
                                    }

                                }
                            }
                            date = date.AddDays(1);
                        }

                        var listaDatas = datasCotacoes.OrderBy(x => x.DayOfWeek)
                                    .GroupBy(d => d)
                                    .Select(s => s.Key)
                                    .OrderBy(x => x)
                                    .ToList();

                        dataCotacao = listaDatas.First();


                        response = request.CreateResponse(HttpStatusCode.OK,
                            new
                            {
                                DataCotacao = dataCotacao,
                                NaoPodeEscolherData = membro.Franquia.DataCotacao,
                                DiasSemanaCotacao = franquiaDiasCotacao.Select(x => x.DiaSemana),
                                DataServidor = DateTime.Now,
                                HoraCotacao = horaCotacao
                            });
                    }
                    else
                    {

                        var escolherDataCotacao = membro.Franquia.DataCotacao;

                        if (escolherDataCotacao == null)
                            membro.Franquia.DataCotacao = false;

                        dataCotacao = DataCotacaoDefinida(horaCotacao, null, date);

                        //Retorna feriado caso exista.
                        diasFeriados = _calendarioFeriadoServiceRep.VerificaFeriadoMembro(dataCotacao, membro,
                            membro.Pessoa.Enderecos.FirstOrDefault(x => x.EnderecoPadrao).Estado.DescEstado);

                        if (diasFeriados.CalendarioFeriado != null &&
                              diasFeriados.TipoRetornoFeriado == TipoRetornoFeriado.NaoExisteFeriado &&
                              date.TimeOfDay < horaCotacao)
                        {
                            var dataFeriado = diasFeriados.CalendarioFeriado.DtEvento
                            .AddHours(horaCotacao.Hours)
                            .AddMinutes(horaCotacao.Minutes)
                            .AddSeconds(horaCotacao.Seconds);

                            dataCotacao = dataFeriado;
                        }
                        else
                        {
                            dataCotacao = DataCotacaoDefinida(horaCotacao, diasFeriados.CalendarioFeriado, date);
                        }

                        response = request.CreateResponse(HttpStatusCode.OK,
                            new
                            {
                                DataCotacao = dataCotacao,
                                NaoPodeEscolherData = membro.Franquia.DataCotacao,
                                DiasSemanaCotacao = franquiaDiasCotacao.Select(x => x.DiaSemana),
                                DataServidor = DateTime.Now,
                                HoraCotacao = horaCotacao
                            });
                    }
                }
                else
                {

                    dataCotacao = DataCotacaoDefinida(horaCotacao, null, date);

                    //Retorna feriado caso exista.
                    var diasFeriados = _calendarioFeriadoServiceRep.VerificaFeriadoMembro(dataCotacao, membro,
                        membro.Pessoa.Enderecos.FirstOrDefault(x => x.EnderecoPadrao).Estado.DescEstado);

                    bool podeEscolherData = membro.Franquia != null;

                    if (diasFeriados.CalendarioFeriado != null &&
                    diasFeriados.TipoRetornoFeriado == TipoRetornoFeriado.NaoExisteFeriado &&
                    date.TimeOfDay < horaCotacao)
                    {
                        var dataFeriado = diasFeriados.CalendarioFeriado.DtEvento
                        .AddHours(horaCotacao.Hours)
                        .AddMinutes(horaCotacao.Minutes)
                        .AddSeconds(horaCotacao.Seconds);

                        dataCotacao = dataFeriado;
                    }
                    else
                    {
                        dataCotacao = DataCotacaoDefinida(horaCotacao, diasFeriados.CalendarioFeriado, date);
                    }

                    response = request.CreateResponse(HttpStatusCode.OK,
                        new
                        {
                            DataCotacao = dataCotacao,
                            NaoPodeEscolherData = podeEscolherData,
                            DiasSemanaCotacao = franquiaDiasCotacao.Select(x => x.DiaSemana),
                            DataServidor = DateTime.Now,
                            HoraCotacao = horaCotacao
                        });
                }

                return response;
            });
        }

        [HttpPost]
        [Route("inserirSemanaCotacao/{dataObrigatorio:bool}")]
        public HttpResponseMessage InserirSemanaCotacao(HttpRequestMessage request, List<int> listaDiaSemana, bool dataObrigatorio)
        {

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;


                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var dtSemanaCt = _dataCotacaoFranquiaRep.GetAll().Where(x => x.FranquiaId == franquia.Id);

                if (dtSemanaCt.Any())
                    _dataCotacaoFranquiaRep.DeleteAll(dtSemanaCt);


                if (listaDiaSemana.Count > 0)
                {

                    if (franquia.DataCotacao == null)
                    {
                        franquia.DataCotacao = true;
                        _franquiaRep.Edit(franquia);
                    }
                    else if (dataObrigatorio)
                    {
                        franquia.DataCotacao = true;
                        _franquiaRep.Edit(franquia);
                    }
                    else
                    {
                        franquia.DataCotacao = false;
                        _franquiaRep.Edit(franquia);
                    }

                }
                else
                {
                    franquia.DataCotacao = false;
                    _franquiaRep.Edit(franquia);
                }

                foreach (var diaSemana in listaDiaSemana)
                {

                    DataCotacaoFranquia dtCtFranquia = new DataCotacaoFranquia()
                    {
                        UsuarioCriacao = usuario,
                        DtCriacao = DateTime.Now,
                        DiaSemana = diaSemana,
                        FranquiaId = franquia.Id,
                        Ativo = true
                    };

                    _dataCotacaoFranquiaRep.Add(dtCtFranquia);
                }

                _unitOfWork.Commit();

                response = request.CreateResponse(HttpStatusCode.OK, new { diasCotacao = listaDiaSemana, PedidoDataObrigatorio = franquia.DataCotacao });


                return response;
            });
        }

        [HttpGet]
        [Route("getDataFranquiaAdm")]
        public HttpResponseMessage GetDataFranquiaAdm(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var diasSemana = _dataCotacaoFranquiaRep.GetAll().Where(x => x.FranquiaId == franquia.Id)
                        .Select(f => f.DiaSemana).ToList();

                response = request.CreateResponse(HttpStatusCode.OK, new { diasCotacao = diasSemana, PedidoDataObrigatorio = franquia.DataCotacao });

                return response;
            });
        }

        [HttpGet]
        [Route("categoriasFranquia")]
        public HttpResponseMessage GetCategoriasFranquia(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var categoriasFranquias = _membroRep.GetAll()
                    .Where(x => x.FranquiaId == franquia.Id).SelectMany(c => c.MembroCategorias)
                    .Select(c => c.Categoria)
                    .Distinct()
                    .ToList();

                IEnumerable<CategoriaViewModel> variavelVM = Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(categoriasFranquias);

                response = request.CreateResponse(HttpStatusCode.OK, variavelVM);

                return response;
            });
        }

        [HttpGet]
        [Route("subcategoriasFranquia")]
        public HttpResponseMessage GetSubCategoriasFranquia(HttpRequestMessage request, int filter)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var subCategoriasFranquias = _membroRep.GetAll()
                    .Where(x => x.FranquiaId == franquia.Id).SelectMany(c => c.MembroCategorias)
                    .SelectMany(c => c.Categoria.SubCategorias.Where(s => s.CategoriaId == filter))
                    .Distinct()
                    .ToList();

                IEnumerable<SubCategoriaViewModel> subCategoriaVM = Mapper.Map<IEnumerable<SubCategoria>, IEnumerable<SubCategoriaViewModel>>(subCategoriasFranquias);

                response = request.CreateResponse(HttpStatusCode.OK, subCategoriaVM);

                return response;
            });
        }

        [HttpGet]
        [Route("carregaProdutosFranquia")]
        public HttpResponseMessage GetProdutosFranquia(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;

            return CreateHttpResponse(request, () =>
            {


                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                var produtosFranquia = _franquiaProdutoRep.GetAll()
                    .Where(x => x.FranquiaId == franquia.Id).Select(c => c.ProdutoId)
                    .ToList();

                response = request.CreateResponse(HttpStatusCode.OK, produtosFranquia);
                return response;
            });
        }

        [HttpPost]
        [Route("inserirDeletarProdutosFranquia")]
        public HttpResponseMessage InserirDeletarProdutosFranquia(HttpRequestMessage request, List<int> produtos)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId).Id;

                if (produtos.Count > 0)
                {
                    foreach (var prd in produtos)
                    {
                        var produto = _franquiaProdutoRep.FirstOrDefault(x => x.ProdutoId == prd && x.FranquiaId == franquia);

                        if (produto == null)
                        {
                            var franquiaProduto = new FranquiaProduto
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                FranquiaId = franquia,
                                ProdutoId = prd,
                                Ativo = true
                            };
                            _franquiaProdutoRep.Add(franquiaProduto);
                            _unitOfWork.Commit();
                        }
                        else
                        {
                            _franquiaProdutoRep.Delete(produto);
                            _unitOfWork.Commit();
                        }
                    }
                }

                var produtosFranquia = _franquiaProdutoRep.GetAll()
                    .Where(x => x.FranquiaId == franquia).Select(c => c.ProdutoId)
                    .ToList();


                response = request.CreateResponse(HttpStatusCode.OK, new { produtosFranquia });

                return response;
            });
        }

        /// <summary>
        /// Retorna Data e Hora para Membros levando em consideração finais de semana e feriados. 
        /// </summary>
        /// <returns>Retorna Data e Hora da Cotação</returns>
        public DateTime DataCotacaoDefinida(TimeSpan time, CalendarioFeriado feriado, DateTime diaHoje)
        {
            var dataHoje = new DateTime();
            var data = diaHoje;

            if (feriado != null)
            {
                data = feriado.DtEvento.AddDays(1);

                dataHoje = data.DayOfWeek == DayOfWeek.Friday
                  ? data.AddDays(3)
                  : data.DayOfWeek == DayOfWeek.Saturday
                  ? data.AddDays(2)
                  : data.DayOfWeek == DayOfWeek.Sunday
                  ? data.AddDays(1)
                  : data;
            }
            else
            {
                dataHoje = DateTime.Now.DayOfWeek == DayOfWeek.Friday && DateTime.Now.TimeOfDay > time
                   ? data.AddDays(3)
                   : DateTime.Now.DayOfWeek == DayOfWeek.Saturday
                   ? data.AddDays(2)
                   : DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.TimeOfDay > time
                   ? data.AddDays(1)
                   : data;
            }

            var dataCotacaoMembro = new DateTime(dataHoje.Year, dataHoje.Month, dataHoje.Day, time.Hours, time.Minutes, time.Seconds);

            return dataCotacaoMembro;
        }

        [HttpGet]
        [Route("carregaFornecedoresFranquia/{page:int=0}/{pageSize=4}/{cidadeId:int=0}/{filter?}")]
        public HttpResponseMessage GetFornecedoresFranquia(HttpRequestMessage request, int? page, int? pageSize, int? cidadeId, string filter = null)
        {

            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var fornecedores = new List<Fornecedor>();
                var totalFornecedores = 0;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                //Pegando todos os membros da Franquia
                var membrosFranquia = franquia.Membros.ToList();

                //Pegando todas regiões dos Mebros da Franquia
                var membrosRegioes = membrosFranquia.SelectMany(x => x.Pessoa.Enderecos.Select(c => c.CidadeId))
                                                    .Distinct()
                                                    .ToList();

                //Pegando todas as categorias dos Membros da Franquia
                var membrosCategorias = membrosFranquia
                                        .SelectMany(c => c.MembroCategorias)
                                        .Select(i => i.CategoriaId)
                                        .Distinct().ToList();

                //Pegando todos os fornecedores da tabela FranquiaFornecedor
                var fornecedoresFranquia = franquia.Fornecedores.Select(x => x.FornecedorId).ToList();


                if (!string.IsNullOrEmpty(filter))
                {
                    filter = filter.Trim().ToLower();
                    var strinFilter = '%' + filter.Replace(' ', '%') + '%';


                    fornecedores = _fornecedorRep.GetAll().Where(c =>
                    c.FornecedorCategorias.Any(r => membrosCategorias.Contains(r.CategoriaId)) &&
                    (c.FornecedorRegiao.Any(r => membrosRegioes.Contains(r.CidadeId)) ||
                    c.FornecedorRegiaoSemanal.Any(r => membrosRegioes.Contains(r.CidadeId))) &&

                    (SqlFunctions.PatIndex(strinFilter, c.Pessoa.PessoaJuridica.RazaoSocial.ToLower()) > 0 ||
                    SqlFunctions.PatIndex(strinFilter, c.Pessoa.PessoaJuridica.NomeFantasia.ToLower()) > 0 ||
                    SqlFunctions.PatIndex(strinFilter, c.Pessoa.PessoaJuridica.Cnpj.ToLower()) > 0 ||
                    SqlFunctions.PatIndex(strinFilter, c.Descricao.ToLower()) > 0 ||
                    SqlFunctions.PatIndex(strinFilter, c.PalavrasChaves.ToLower()) > 0 ||
                    c.FornecedorCategorias.Any(f => SqlFunctions.PatIndex(strinFilter, f.Categoria.DescCategoria.ToLower()) > 0)))

                    //c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter) ||
                    //c.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                    //c.Pessoa.PessoaJuridica.Cnpj.ToLower().Contains(filter) ||
                    //c.Descricao.ToLower().Contains(filter) ||
                    //c.PalavrasChaves.ToLower().Contains(filter) ||
                    //c.FornecedorCategorias.Select(f => f.Categoria.DescCategoria).Contains(filter))

                    .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                    .ThenBy(x => x.Pessoa.PessoaJuridica.RazaoSocial)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                    totalFornecedores = _fornecedorRep.GetAll().Count(c =>
                      c.FornecedorCategorias.Any(r => membrosCategorias.Contains(r.CategoriaId)) &&
                      (c.FornecedorRegiao.Any(r => membrosRegioes.Contains(r.CidadeId)) ||
                      c.FornecedorRegiaoSemanal.Any(r => membrosRegioes.Contains(r.CidadeId))) &&
                        (SqlFunctions.PatIndex(strinFilter, c.Pessoa.PessoaJuridica.RazaoSocial.ToLower()) > 0 ||
                        SqlFunctions.PatIndex(strinFilter, c.Pessoa.PessoaJuridica.NomeFantasia.ToLower()) > 0 ||
                        SqlFunctions.PatIndex(strinFilter, c.Pessoa.PessoaJuridica.Cnpj.ToLower()) > 0 ||
                        SqlFunctions.PatIndex(strinFilter, c.Descricao.ToLower()) > 0 ||
                        SqlFunctions.PatIndex(strinFilter, c.PalavrasChaves.ToLower()) > 0 ||
                        c.FornecedorCategorias.Any(f => SqlFunctions.PatIndex(strinFilter, f.Categoria.DescCategoria.ToLower()) > 0)));

                    //totalFornecedores = _fornecedorRep.GetAll().Count(c =>
                    //    c.FornecedorCategorias.Any(r => membrosCategorias.Contains(r.CategoriaId)) &&
                    //    (c.FornecedorRegiao.Any(r => membrosRegioes.Contains(r.CidadeId)) ||
                    //    c.FornecedorRegiaoSemanal.Any(r => membrosRegioes.Contains(r.CidadeId))) &&
                    //    c.Pessoa.PessoaJuridica.RazaoSocial.ToLower().Contains(filter) ||
                    //    c.Pessoa.PessoaJuridica.NomeFantasia.ToLower().Contains(filter) ||
                    //    c.Pessoa.PessoaJuridica.Cnpj.ToLower().Contains(filter) ||
                    //    c.Descricao.ToLower().Contains(filter) ||
                    //    c.PalavrasChaves.ToLower().Contains(filter) ||
                    //    c.FornecedorCategorias.Select(f => f.Categoria.DescCategoria).Contains(filter));
                }
                else
                {
                    if (cidadeId > 0)
                    {
                        //Se selecionar uma cidade
                        fornecedores = _fornecedorRep.GetAll().Where(c =>
                     c.FornecedorCategorias.Any(r => membrosCategorias.Contains(r.CategoriaId)) &&
                     (c.FornecedorRegiao.Any(r => membrosRegioes.Contains(cidadeId.Value)) ||
                     c.FornecedorRegiaoSemanal.Any(r => membrosRegioes.Contains(cidadeId.Value))))
                     .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                    .ThenBy(x => x.Pessoa.PessoaJuridica.RazaoSocial)
                    .Skip(currentPage * currentPageSize)
                    .Take(currentPageSize)
                    .ToList();

                        totalFornecedores = _fornecedorRep.GetAll()
                        .Count(c =>
                        c.FornecedorCategorias.Any(r => membrosCategorias.Contains(r.CategoriaId)) &&
                        (c.FornecedorRegiao.Any(r => membrosRegioes.Contains(cidadeId.Value)) ||
                        c.FornecedorRegiaoSemanal.Any(r => membrosRegioes.Contains(cidadeId.Value))));
                    }
                    else
                    {
                        //Se não for selecionado nenhuma cidade
                        fornecedores = _fornecedorRep.GetAll().Where(c =>
                    c.FornecedorCategorias.Any(r => membrosCategorias.Contains(r.CategoriaId)) &&
                    (c.FornecedorRegiao.Any(r => membrosRegioes.Contains(r.CidadeId)) ||
                    c.FornecedorRegiaoSemanal.Any(r => membrosRegioes.Contains(r.CidadeId))))
                   .OrderBy(c => c.Pessoa.PessoaJuridica.RazaoSocial)
                   .ThenBy(x => x.Pessoa.PessoaJuridica.RazaoSocial)
                   .Skip(currentPage * currentPageSize)
                   .Take(currentPageSize)
                   .ToList();

                        totalFornecedores = _fornecedorRep.GetAll()
                        .Count(c =>
                        c.FornecedorCategorias.Any(r => membrosCategorias.Contains(r.CategoriaId)) &&
                        (c.FornecedorRegiao.Any(r => membrosRegioes.Contains(r.CidadeId)) ||
                        c.FornecedorRegiaoSemanal.Any(r => membrosRegioes.Contains(r.CidadeId))));
                    }
                }

                var fornecedoresVM = Mapper.Map<IEnumerable<Fornecedor>, IEnumerable<MembroSolicitaFornecedorViewModel>>(fornecedores);

                foreach (var f in fornecedoresVM)
                {
                    var prazoSemanal = new List<FornecedorPrazoSemanalViewModel>();
                    //Verifica se existe fornecedor que já trabalha com a franquia
                    f.TrabalhaFranquia = fornecedoresFranquia.Contains(f.FornecedorId);

                    //Entra caso o fornecedor trabalhe somente com entrega por dias da semana
                    if (f.PrazoEntegaFornecedor.TryParseInt() == 0)
                    {
                        if (f.FornecedorPrazoSemanal.Count > 0)
                        {
                            for (int i = 0; i < f.FornecedorPrazoSemanal.Count; i++)
                            {
                                if (f.FornecedorPrazoSemanal[i].CidadeId == membrosRegioes.FirstOrDefault())
                                {
                                    prazoSemanal.Add(f.FornecedorPrazoSemanal[i]);
                                }
                            }
                        }
                        if (prazoSemanal.Count > 0)
                        {
                            f.FornecedorPrazoSemanal = prazoSemanal;
                        }
                    }
                    else
                    {
                        f.FornecedorPrazoSemanal.Clear();
                    }
                }

                var pagSet = new PaginacaoConfig<MembroSolicitaFornecedorViewModel>
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

        [HttpPost]
        [Route("inserirDeletarFornecedorFranquia")]
        public HttpResponseMessage InserirDeletarFornecedorFranquia(HttpRequestMessage request, List<int> fornecedores)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                var franquia = _franquiaRep.FirstOrDefault(x => x.PessoaId == usuario.PessoaId);

                if (fornecedores.Count > 0)
                {
                    foreach (var fornecedor in fornecedores)
                    {
                        var franquiaFornecedor = _franquiaFornecedorRep
                        .FirstOrDefault(x => x.FornecedorId == fornecedor && x.FranquiaId == franquia.Id);

                        if (franquiaFornecedor == null)
                        {
                            var franqFornecedor = new FranquiaFornecedor
                            {
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                FranquiaId = franquia.Id,
                                FornecedorId = fornecedor,
                                Ativo = true
                            };
                            _franquiaFornecedorRep.Add(franqFornecedor);
                            _unitOfWork.Commit();
                        }
                        else
                        {
                            var membrosFornecedores = franquia.Membros.SelectMany(x => x.MembroFornecedores)
                                       .Where(f => f.FornecedorId == fornecedor).ToList();

                            if (membrosFornecedores.Count > 0)
                            {
                                var membrosRelacionados = membrosFornecedores.Select(x => x.Membro).ToList();

                                foreach (var membros in membrosRelacionados)
                                {
                                    var usu = membros.Pessoa.Usuarios.ToList();

                                    #region Envia Email para usuários do Membro

                                    foreach (var us in usu)
                                    {
                                        var notificacao = _notificacoesAlertaServiceRep.PodeEnviarNotificacao(us.Id,
                                            (int)TipoAviso.FranquiaRetiraFornecedorCotacoes, TipoAlerta.EMAIL);

                                        if (notificacao)
                                        {
                                            var template = _templateEmailRep.GetSingle(31).Template
                                            .Replace("#NomeFantasia#", us.Pessoa.PessoaJuridica.NomeFantasia)
                                            .Replace("#Fornecedor#", franquiaFornecedor.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia);

                                            var email = new Emails
                                            {
                                                UsuarioCriacao = us,
                                                DtCriacao = DateTime.Now,
                                                EmailDestinatario = us.UsuarioEmail,
                                                AssuntoEmail = "Fornecedor - Fornecedor removido das cotações",
                                                CorpoEmail = template,
                                                Origem = Origem.FranquiaRemoveFornecedorCotacao,
                                                Status = Status.NaoEnviado,
                                                Ativo = true

                                            };
                                            _emailsRep.Add(email);
                                            _unitOfWork.Commit();
                                        }
                                    }

                                    #endregion
                                }

                                //remove os relacionamentos dos membros da franquia com Fornecedor na tabela MembroFornecedor
                                _membroFornecedorRep.DeleteAll(membrosFornecedores);
                            }

                            _franquiaFornecedorRep.Delete(franquiaFornecedor);
                            _unitOfWork.Commit();
                        }
                    }
                }
                response = request.CreateResponse(HttpStatusCode.OK);

                return response;
            });
        }

    }
}
