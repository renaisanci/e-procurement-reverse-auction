using AutoMapper;
using ECC.API_Web.br.com.correios.apps;
using ECC.API_Web.InfraWeb;
using ECC.API_Web.InfraWeb.ExtensionsWeb;
using ECC.API_Web.Models;
using ECC.Dados.Extensions;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeEndereco;
using ECC.Entidades;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using ECC.EntidadePessoa;
using ECC.Entidades.EntidadePessoa;
using WebGrease.Css.Extensions;

namespace ECC.API_Web.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize(Roles = "Admin, Membro, Fornecedor, Franquia")]
    [RoutePrefix("api/endereco")]
    public class EnderecoController : ApiControllerBase
    {
        private readonly IEntidadeBaseRep<Logradouro> _logradouroRep;
        private readonly IEntidadeBaseRep<Estado> _estadoRep;
        private readonly IEntidadeBaseRep<CepEndereco> _cepEnderecoRep;
        private readonly IEntidadeBaseRep<Fornecedor> _fornecedorRep;
        private readonly IEntidadeBaseRep<Endereco> _enderecoRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Cidade> _cidadeRep;
        private readonly IEntidadeBaseRep<Regiao> _regiaoRep;
        private readonly IEntidadeBaseRep<Bairro> _bairroRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<FornecedorRegiao> _fornecedorRegiaoRep;
        private readonly IEntidadeBaseRep<PeriodoEntrega> _periodoEntregaRep;
        private readonly IEntidadeBaseRep<HorasEntregaMembro> _horarioEntregaMembroRep;
        private readonly IEntidadeBaseRep<FornecedorPrazoSemanal> _fornecedorPrazoSemanalRep;

        private readonly IUtilService _utilService;

        public EnderecoController(IEntidadeBaseRep<Bairro> bairroRep,
            IEntidadeBaseRep<Cidade> cidadeRep,
            IEntidadeBaseRep<Endereco> enderecoRep,
            IEntidadeBaseRep<CepEndereco> cepEnderecoRep,
            IEntidadeBaseRep<Estado> estadoRep,
            IEntidadeBaseRep<Fornecedor> fornecedorRep,
            IEntidadeBaseRep<Logradouro> logradouroRep,
            IEntidadeBaseRep<Usuario> usuarioRep,
            IEntidadeBaseRep<Regiao> regiaoRep,
            IEntidadeBaseRep<Erro> errosRepository,
            IEntidadeBaseRep<Membro> membroRep,
            IEntidadeBaseRep<FornecedorRegiao> fornecedorRegiaoRep,
            IEntidadeBaseRep<PeriodoEntrega> periodoEntregaRep,
            IEntidadeBaseRep<HorasEntregaMembro> horarioEntregaMembroRep,
            IEntidadeBaseRep<FornecedorPrazoSemanal> fornecedorPrazoSemanalRep,
        IUtilService utilService,
            IUnitOfWork unitOfWork)
            : base(usuarioRep, errosRepository, unitOfWork)
        {


            _logradouroRep = logradouroRep;
            _estadoRep = estadoRep;
            _cepEnderecoRep = cepEnderecoRep;
            _enderecoRep = enderecoRep;
            _usuarioRep = usuarioRep;
            _bairroRep = bairroRep;
            _cidadeRep = cidadeRep;
            _regiaoRep = regiaoRep;
            _membroRep = membroRep;
            _periodoEntregaRep = periodoEntregaRep;
            _horarioEntregaMembroRep = horarioEntregaMembroRep;
            _utilService = utilService;
            _fornecedorRep = fornecedorRep;
            _fornecedorRegiaoRep = fornecedorRegiaoRep;
            _fornecedorPrazoSemanalRep = fornecedorPrazoSemanalRep;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logradouro")]
        public HttpResponseMessage GetLogradouro(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var logradouros = _logradouroRep.GetAll().ToList();

                var logradourosVM = Mapper.Map<IEnumerable<Logradouro>, IEnumerable<LogradouroViewModel>>(logradouros);

                response = request.CreateResponse(HttpStatusCode.OK, logradourosVM);

                return response;
            });
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("cidade")]
        public HttpResponseMessage GetCidade(HttpRequestMessage request, int estadoId)
        {
            return CreateHttpResponse(request, () =>
            {
                var cidades = _cidadeRep.GetAll().Where(x => x.Estado.Id == estadoId).ToList();

                var cidadesVM = Mapper.Map<IEnumerable<Cidade>, IEnumerable<CidadeViewModel>>(cidades);

                var response = request.CreateResponse(HttpStatusCode.OK, cidadesVM);

                return response;
            });
        }

        [HttpGet]
        [Route("cidadeporregiao")]
        public HttpResponseMessage GetCidade(HttpRequestMessage request, int estadoId, int regiaoId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Cidade> cidades = new List<Cidade>();

                if (regiaoId == 0)
                {
                    cidades = _cidadeRep.GetAll().Where(x => x.Estado.Id == estadoId)
                       .OrderBy(o => o.DescCidade)
                       .ToList();
                }
                else
                {
                    cidades = _cidadeRep.GetAll().Where(x => x.Estado.Id == estadoId && x.Regiao.Id == regiaoId)
                        .OrderBy(o => o.DescCidade)
                        .ToList();
                }

                IEnumerable<CidadeViewModel> cidadesVM = Mapper.Map<IEnumerable<Cidade>, IEnumerable<CidadeViewModel>>(cidades);

                response = request.CreateResponse(HttpStatusCode.OK, cidadesVM);

                return response;
            });
        }


        [HttpGet]
        [Route("cidadeporregiaopaginada/{page:int=0}/{pageSize=4}/{estadoId=0}/{regiaoId=0}/{filter?}")]
        public HttpResponseMessage GetCidadeRegiaoPaginada(HttpRequestMessage request, int? page, int? pageSize, int estadoId, int regiaoId, string filter = null)
        {
            int currentPage = page.Value;
            int currentPageSize = pageSize.Value;

            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                List<Cidade> cidades = new List<Cidade>();
                int totalCidades = new int();

                if (regiaoId == 0)
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        filter = filter.Trim().ToLower();

                        cidades = _cidadeRep.GetAll()
                            .Where(x => x.Estado.Id == estadoId &&
                            x.DescCidade.ToLower().Contains(filter))
                            .OrderBy(o => o.DescCidade)
                            .Skip(currentPage * currentPageSize)
                            .Take(currentPageSize)
                            .ToList();

                        totalCidades = _cidadeRep.GetAll().Count(x => x.Estado.Id == estadoId &&
                        x.DescCidade.Contains(filter));
                    }
                    else
                    {
                        cidades = _cidadeRep.GetAll()
                           .Where(x => x.Estado.Id == estadoId)
                           .OrderBy(o => o.DescCidade)
                           .Skip(currentPage * currentPageSize)
                           .Take(currentPageSize)
                           .ToList();

                        totalCidades = _cidadeRep.GetAll().Count(x => x.Estado.Id == estadoId);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        filter = filter.Trim().ToLower();

                        cidades = _cidadeRep.GetAll()
                        .Where(x => x.Estado.Id == estadoId && x.Regiao.Id == regiaoId &&
                        x.DescCidade.ToLower().Contains(filter))
                             .OrderBy(o => o.DescCidade)
                             .Skip(currentPage * currentPageSize)
                             .Take(currentPageSize)
                             .ToList();

                        totalCidades = _cidadeRep.GetAll().Count(x => x.Estado.Id == estadoId &&
                            x.Regiao.Id == regiaoId &&
                            x.DescCidade.Contains(filter));
                    }
                    else
                    {
                        cidades = _cidadeRep.GetAll()
                        .Where(x => x.Estado.Id == estadoId && x.Regiao.Id == regiaoId)
                         .OrderBy(o => o.DescCidade)
                         .Skip(currentPage * currentPageSize)
                         .Take(currentPageSize)
                         .ToList();

                        totalCidades = _cidadeRep.GetAll().Count(x => x.Estado.Id == estadoId && x.Regiao.Id == regiaoId);
                    }
                }
                IEnumerable<CidadeViewModel> cidadesVM = Mapper.Map<IEnumerable<Cidade>, IEnumerable<CidadeViewModel>>(cidades);

                PaginacaoConfig<CidadeViewModel> pagSet = new PaginacaoConfig<CidadeViewModel>()
                {
                    Page = currentPage,
                    TotalCount = totalCidades,
                    TotalPages = (int)Math.Ceiling((decimal)totalCidades / currentPageSize),
                    Items = cidadesVM
                };
                response = request.CreateResponse(HttpStatusCode.OK, pagSet);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("regiao")]
        public HttpResponseMessage GetRegiao(HttpRequestMessage request, int estadoId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var regioes = _regiaoRep.GetAll().Where(x => x.Estado.Id == estadoId).ToList();

                IEnumerable<RegiaoViewModel> regioesVM = Mapper.Map<IEnumerable<Regiao>, IEnumerable<RegiaoViewModel>>(regioes);

                response = request.CreateResponse(HttpStatusCode.OK, regioesVM);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("bairro")]
        public HttpResponseMessage GetBairro(HttpRequestMessage request, int cidadeId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var bairros = _bairroRep.GetAll().Where(x => x.Cidade.Id == cidadeId).ToList();

                IEnumerable<BairroViewModel> bairrosVM = Mapper.Map<IEnumerable<Bairro>, IEnumerable<BairroViewModel>>(bairros);

                response = request.CreateResponse(HttpStatusCode.OK, bairrosVM);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("estado")]
        public HttpResponseMessage GetEstado(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var estados = _estadoRep.GetAll().ToList();

                IEnumerable<EstadoViewModel> estadosVM = Mapper.Map<IEnumerable<Estado>, IEnumerable<EstadoViewModel>>(estados);

                response = request.CreateResponse(HttpStatusCode.OK, estadosVM);

                return response;
            });
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("enderecoCep")]
        public HttpResponseMessage GetEnderecoViaCep(HttpRequestMessage request, string cep)
        {
            return CreateHttpResponse(request, () =>
            {
                var ceps = _cepEnderecoRep.GetAll().Where(x => x.Cep == cep).FirstOrDefault();

                var cepEndereco = ceps ?? this.PesquisaCep(cep);

                var enderecoVM = Mapper.Map<CepEndereco, EnderecoViewModel>(cepEndereco);

                if (String.IsNullOrEmpty(enderecoVM.Cep))
                    enderecoVM.Cep = cep;


                var response = request.CreateResponse(HttpStatusCode.OK, enderecoVM);




                return response;
            });
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("inserirCadastro")]
        public HttpResponseMessage InserirCadastro(HttpRequestMessage request, EnderecoViewModel enderecoViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (enderecoViewModel.BairroDescNew != null ||
               enderecoViewModel.BairroDescNew != "")
                {
                    //remove a validação do bairro quase não tenha achado, pois nesse caso vamos cadastro o digitado manualmente
                    ModelState.Remove("enderecoViewModel.BairroId");
                }

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
                        Usuario usuario = _usuarioRep.GetSingle(1);

                        if (!string.IsNullOrEmpty(enderecoViewModel.BairroDescNew))
                        {
                            //var cit = _cidadeRep.GetSingle(fornecedorViewModel.Endereco.CidadeId);                      
                            Bairro bairroCad = new Bairro()
                            {
                                DescBairro = enderecoViewModel.BairroDescNew,
                                CidadeId = enderecoViewModel.CidadeId,
                                DtCriacao = DateTime.Now,
                                UsuarioCriacao = usuario
                            };

                            _bairroRep.Add(bairroCad);
                            _unitOfWork.Commit();

                            enderecoViewModel.BairroId = bairroCad.Id;
                        }

                        if (_enderecoRep.GetAll().Count(x => x.Pessoa.Id == enderecoViewModel.PessoaId) == 0)
                            enderecoViewModel.EnderecoPadrao = true;

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
                            Complemento = enderecoViewModel.Complemento,
                            DescEndereco = enderecoViewModel.Endereco.Trim(),
                            Cep = enderecoViewModel.Cep,
                            Ativo = enderecoViewModel.Ativo,
                            EnderecoPadrao = enderecoViewModel.EnderecoPadrao,
                            Referencia = enderecoViewModel.Referencia
                        };

                        novoEndereco.LocalizacaoGoogle();

                        _enderecoRep.Add(novoEndereco);

                        _unitOfWork.Commit();

                        Membro membroAtual = _membroRep.FindBy(p => p.PessoaId == enderecoViewModel.PessoaId).FirstOrDefault();

                        // Update view model
                        enderecoViewModel = Mapper.Map<Endereco, EnderecoViewModel>(novoEndereco);
                        response = request.CreateResponse(HttpStatusCode.Created, enderecoViewModel);



                    }
                }

                return response;
            });
        }


        [HttpPost]
        [Route("inserir")]
        public HttpResponseMessage Inserir(HttpRequestMessage request, EnderecoViewModel enderecoViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (enderecoViewModel.BairroDescNew != null ||
               enderecoViewModel.BairroDescNew != "")
                {
                    //remove a validação do bairro quase não tenha achado, pois nesse caso vamos cadastro o digitado manualmente
                    ModelState.Remove("enderecoViewModel.BairroId");
                }

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

                        if (!string.IsNullOrEmpty(enderecoViewModel.BairroDescNew))
                        {
                            //var cit = _cidadeRep.GetSingle(fornecedorViewModel.Endereco.CidadeId);                      
                            Bairro bairroCad = new Bairro()
                            {
                                DescBairro = enderecoViewModel.BairroDescNew,
                                CidadeId = enderecoViewModel.CidadeId,
                                DtCriacao = DateTime.Now,
                                UsuarioCriacao = usuario
                            };

                            _bairroRep.Add(bairroCad);
                            _unitOfWork.Commit();

                            enderecoViewModel.BairroId = bairroCad.Id;
                        }

                        if (_enderecoRep.GetAll().Count(x => x.Pessoa.Id == enderecoViewModel.PessoaId) == 0)
                            enderecoViewModel.EnderecoPadrao = true;

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
                            Complemento = enderecoViewModel.Complemento,
                            DescEndereco = enderecoViewModel.Endereco.Trim(),
                            Cep = enderecoViewModel.Cep,
                            Ativo = enderecoViewModel.Ativo,
                            EnderecoPadrao = enderecoViewModel.EnderecoPadrao,
                            Referencia = enderecoViewModel.Referencia
                        };

                        novoEndereco.LocalizacaoGoogle();

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


                        Membro membroAtual = _membroRep.FindBy(p => p.PessoaId == enderecoViewModel.PessoaId).FirstOrDefault();


                        // Update view model
                        enderecoViewModel = Mapper.Map<Endereco, EnderecoViewModel>(novoEndereco);
                        response = request.CreateResponse(HttpStatusCode.Created, enderecoViewModel);



                    }
                }

                return response;
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("atualizar")]
        public HttpResponseMessage Atualizar(HttpRequestMessage request, EnderecoViewModel enderecoViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                HorasEntregaMembro horasEntrega = new HorasEntregaMembro();
                List<PeriodoEntrega> listaPeriodo = new List<PeriodoEntrega>();

                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest,
                        ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                              .Select(m => m.ErrorMessage).ToArray());
                }
                else
                {
                    Endereco _endereco = _enderecoRep.GetSingle(enderecoViewModel.Id);
                    var usuario = new Usuario();

                    if (HttpContext.Current.User.Identity.GetUserId() != null)
                        usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));
                    else
                        usuario = _usuarioRep.GetSingle(1);

                    #region Atualiza Período de Entrega

                    var pegarTodosHorariosEntrega = _horarioEntregaMembroRep.GetAll().Where(x => x.EnderecoId == enderecoViewModel.Id).ToList();

                    if (pegarTodosHorariosEntrega.Count > 0)
                    {
                        _horarioEntregaMembroRep.DeleteAll(pegarTodosHorariosEntrega);
                        _unitOfWork.Commit();

                        foreach (var periodo in enderecoViewModel.PeriodoEntrega)
                        {
                            horasEntrega.UsuarioCriacao = usuario;
                            horasEntrega.DtCriacao = DateTime.Now;
                            horasEntrega.PeriodoId = periodo.Id;
                            horasEntrega.EnderecoId = enderecoViewModel.Id;
                            horasEntrega.DescHorarioEntrega = enderecoViewModel.DescHorarioEntrega;
                            horasEntrega.Ativo = true;


                            _horarioEntregaMembroRep.Add(horasEntrega);
                            _unitOfWork.Commit();
                        }
                    }
                    else
                    {
                        foreach (var periodo in enderecoViewModel.PeriodoEntrega)
                        {
                            horasEntrega.UsuarioCriacao = usuario;
                            horasEntrega.DtCriacao = DateTime.Now;
                            horasEntrega.PeriodoId = periodo.Id;
                            horasEntrega.EnderecoId = enderecoViewModel.Id;
                            horasEntrega.DescHorarioEntrega = enderecoViewModel.DescHorarioEntrega;
                            horasEntrega.Ativo = true;

                            _horarioEntregaMembroRep.Add(horasEntrega);
                            _unitOfWork.Commit();
                        }
                    }

                    #endregion

                    _endereco.AtualizarEndereco(enderecoViewModel, usuario);

                    _unitOfWork.Commit();

                    // Update view model

                    // enderecoViewModel = Mapper.Map<Endereco, EnderecoViewModel>(_endereco);

                    // enderecoViewModel.PeriodoEntrega.Clear();

                    // enderecoViewModel.PeriodoEntrega.AddRange(listaPeriodo);

                    response = request.CreateResponse(HttpStatusCode.OK, enderecoViewModel);

                }

                return response;
            });
        }

        [AllowAnonymous]
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
        [AllowAnonymous]
        [HttpGet]
        [Route("enderecoMembro")]
        public HttpResponseMessage GetEnderecoViaCep(HttpRequestMessage request, int pessoaId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                var enderecos = _enderecoRep.GetAll().Where(x => x.Pessoa.Id == pessoaId);


                IEnumerable<EnderecoViewModel> enderecosVM = Mapper.Map<IEnumerable<Endereco>, IEnumerable<EnderecoViewModel>>(enderecos);



                response = request.CreateResponse(HttpStatusCode.OK, enderecosVM);
                return response;
            });
        }

        [HttpGet]
        [Route("enderecoMembroAtivo")]
        public HttpResponseMessage GetEnderecoMembroAtivo(HttpRequestMessage request, int pessoaId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var listaEnderecosVM = new List<EnderecoViewModel>();

                var membro = _membroRep.FirstOrDefault(x => x.PessoaId == pessoaId);

                var enderecosMembro = membro.Pessoa.Enderecos.Where(a => a.Ativo).ToList();
                var enderecosFornecedor = membro.MembroFornecedores.SelectMany(x => x.Fornecedor.Pessoa.Enderecos).ToList();

                enderecosMembro.ForEach(x =>
                {
                    var listPeriodo = new List<PeriodoEntregaViewModel>();

                    x.HorasEntregaMembro.ForEach(t =>
                    {
                        listPeriodo.Add(new PeriodoEntregaViewModel
                        {
                            Id = t.Id,
                            DescPeriodoEntrega = t.DescHorarioEntrega
                        });
                    });


                    listaEnderecosVM.Add(new EnderecoViewModel
                    {
                        Id = x.Id,
                        EstadoId = x.EstadoId,
                        CidadeId = x.CidadeId,
                        BairroId = x.BairroId,
                        LogradouroId = x.LogradouroId,
                        Bairro = x.Bairro.DescBairro,
                        Cep = x.Cep,
                        Complemento = x.Complemento,
                        Cidade = x.Cidade.DescCidade,
                        Estado = x.Estado.DescEstado,
                        Logradouro = x.Logradouro.DescLogradouro,
                        NumEndereco = x.Numero,
                        Ativo = x.Ativo,
                        EnderecoPadrao = x.EnderecoPadrao,
                        DescHorarioEntrega = x.HorasEntregaMembro.Select(h => h.DescHorarioEntrega).FirstOrDefault(),
                        PeriodoEntrega = listPeriodo,
                        DescAtivo = x.Ativo ? "Ativo" : "Inativo",
                        Endereco = x.DescEndereco,
                        FornecedorNesteEndereco = enderecosFornecedor.Exists(d => d.CidadeId == x.CidadeId)
                    });

                });

                response = request.CreateResponse(HttpStatusCode.OK, listaEnderecosVM);
                return response;
            });
        }

        [HttpGet]
        [Route("enderecoMembroPadrao")]
        public HttpResponseMessage GetEnderecoPadrao(HttpRequestMessage request, string usuarioEmail)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var endereco = _enderecoRep.GetAll().FirstOrDefault(x => x.Pessoa.Id == usuario.PessoaId && x.EnderecoPadrao);

                var enderecoViewModel = Mapper.Map<Endereco, EnderecoViewModel>(endereco);

                response = request.CreateResponse(HttpStatusCode.OK, enderecoViewModel);

                return response;
            });
        }

        public CepEndereco PesquisaCep(string cep)
        {
            var cepEndereco = new CepEndereco();
            var cepService = new AtendeClienteService();
            var result = cepService.consultaCEP(cep);

            var usuario = _usuarioRep.GetSingle(1);

            if (result == null) return cepEndereco;

            //Pega o id do estado.
            var estado = _estadoRep.GetAll().FirstOrDefault(x => x.Uf == result.uf);

            //Se não existir o estado, cadastrar
            if (estado == null)
            {
                estado = new Estado
                {
                    Ativo = true,
                    DtCriacao = DateTime.Now,
                    UsuarioCriacao = usuario,
                    Uf = result.uf,
                    DescEstado = result.uf
                };

                //Cadastrando a cidade.
                _estadoRep.Add(estado);
                _unitOfWork.Commit();
            }

            cepEndereco.Estado = estado;
            cepEndereco.EstadoId = estado.Id;

            //Pega o id da cidade.
            var cidade = _cidadeRep.GetAll().FirstOrDefault(x => x.EstadoId == cepEndereco.EstadoId && x.DescCidade.Trim() == result.cidade.Trim());

            // Se não existir a cidade, cadastramos.
            if (cidade == null)
            {
                cidade = new Cidade
                {
                    Ativo = true,
                    RegiaoId = 0, //TODO: Revisitar vai gravar zero pq a cidade é do correio e nao esta cadastrado portanto nao tem o id regiao ainda
                    EstadoId = cepEndereco.EstadoId,
                    DescCidade = result.cidade,
                    DtCriacao = DateTime.Now,
                    UsuarioCriacao = usuario
                };

                //Cadastrando a cidade.
                _cidadeRep.Add(cidade);
                _unitOfWork.Commit();
            }

            cepEndereco.Cidade = cidade;
            cepEndereco.CidadeId = cidade.Id;

            //Pega o id do bairro.
            var bairro = _bairroRep.GetAll().FirstOrDefault(x => x.CidadeId == cepEndereco.CidadeId && x.DescBairro.Trim() == result.bairro.Trim());

            // Se não existir o bairro, cadastramos.
            if (bairro == null)
            {
                if (result.bairro != "")
                {
                    bairro = new Bairro
                    {
                        Ativo = true,
                        CidadeId = cepEndereco.CidadeId,
                        //Cidade = cepEndereco.Cidade,
                        DescBairro = result.bairro,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacao = usuario
                    };

                    //Cadastrando a cidade.
                    _bairroRep.Add(bairro);
                    _unitOfWork.Commit();
                }
            }

            var lograCc = result.end.Split(' ')[0];

            var logradouro = _logradouroRep.GetAll();
            var lograPesq = logradouro.FirstOrDefault(x => x.DescLogradouro.Trim() == lograCc.Trim());

            logradouro.ForEach(x => { result.end = result.end.Replace(x.DescLogradouro, ""); });
            result.end = result.end.Trim();

            if (bairro != null && result.end != "" && result.cep != "" && lograPesq != null)
            {
                //Preenche o restante dos dados e cadastramos o endereço.
                //cepEndereco.Bairro = bairro;
                cepEndereco.BairroId = bairro.Id;
                cepEndereco.LogradouroId = lograPesq.Id;
                cepEndereco.Cep = result.cep;
                cepEndereco.DescLogradouro = result.end;
                cepEndereco.Complemento = !string.IsNullOrEmpty(cepEndereco.Complemento)
                    ? result.complemento
                    : result.complemento2;
                cepEndereco.DtCriacao = DateTime.Now;
                cepEndereco.UsuarioCriacao = usuario;

                _cepEnderecoRep.Add(cepEndereco);
                _unitOfWork.Commit();
            }

            return cepEndereco;

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("periodoentrega")]
        public HttpResponseMessage GetPeriodoEntrega(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                Usuario usuario = _usuarioRep.GetSingle(int.Parse(HttpContext.Current.User.Identity.GetUserId()));

                var periodoentrega = _periodoEntregaRep.GetAll().Where(x => x.Ativo == true);

                IEnumerable<PeriodoEntregaViewModel> periodoEntregaVM = Mapper.Map<IEnumerable<PeriodoEntrega>, IEnumerable<PeriodoEntregaViewModel>>(periodoentrega);

                response = request.CreateResponse(HttpStatusCode.OK, periodoEntregaVM);

                return response;
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("carregaRegiaoPrazo/{idFornecedor:int}")]
        public HttpResponseMessage GetCarregaRegiaoPrazo(HttpRequestMessage request, int? idFornecedor)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var cidades = new List<int>();

                // var fornecedor = _fornecedorRep.FirstOrDefault(x => x.Id == idFornecedor);

                var regiaoPrazoFornecedor = _fornecedorRegiaoRep.FindBy(x => x.Fornecedor.Id == idFornecedor).Select(x => new { x.Id, x.FornecedorId, x.CidadeId, x.Prazo, x.Cidade.DescCidade }).ToList();
                //var regiaoPrazoFornecedorVM = fornecedor?.FornecedorRegiao
                //    .Where(x => x.VlPedMinRegiao != 0).Select(x => new { x.Id, x.FornecedorId, x.CidadeId, x.Prazo, x.Cidade.DescCidade } )
                //    .ToList();
                var regiaoPrazoSemanalFornecedor = _fornecedorPrazoSemanalRep.FindBy(x => x.Fornecedor.Id == idFornecedor).Select(x => new { x.Id, x.FornecedorId, x.CidadeId, x.Cidade.DescCidade, x.DiaSemana, x.VlPedMinRegiao, x.TaxaEntrega, x.Cif }).ToList();

                //var regiaoPrazoSemanalFornecedor = fornecedor?.FornecedorRegiaoSemanal
                //    .Where(x => x.VlPedMinRegiao != 0)
                //    .ToList();





                //IEnumerable<FornecedorRegiaoViewModel> regiaoPrazoFornecedorVM = Mapper.Map<IEnumerable<FornecedorRegiao>, IEnumerable<FornecedorRegiaoViewModel>>(regiaoPrazoFornecedor);
                //  IEnumerable<FornecedorPrazoSemanalViewModel> fornecedorPrazoSemanalVM = Mapper.Map<IEnumerable<FornecedorPrazoSemanal>, IEnumerable<FornecedorPrazoSemanalViewModel>>(regiaoPrazoSemanalFornecedor);

                var regiaoSemanalVM = regiaoPrazoSemanalFornecedor
                .GroupBy(g => new { g.DescCidade, g.CidadeId })
                .Select(g => new
                {
                    NomeCidade = g.Key.DescCidade,
                    IdCidade = g.Key.CidadeId,
                    DiaSemana = g.Select(d => d.DiaSemana)
                })
                .ToList();

                var listaCidadeCorridos = regiaoPrazoFornecedor.Select(c => c.CidadeId).ToList();
                cidades.AddRange(listaCidadeCorridos);
                var listaCidadesSemanal = regiaoSemanalVM.Select(c => c.IdCidade).ToList();
                cidades.AddRange(listaCidadesSemanal);
                var cidadesUnicas = cidades.Distinct();
                var cidadesCorridosSemanal = _cidadeRep.GetAll().Where(c => cidadesUnicas.Contains(c.Id));

                IEnumerable<CidadeViewModel> cidadesVM = Mapper.Map<IEnumerable<Cidade>, IEnumerable<CidadeViewModel>>(cidadesCorridosSemanal);


                response = request.CreateResponse(HttpStatusCode.OK, new { regiaoPrazoFornecedor, regiaoSemanalVM, cidadesVM });

                return response;
            });
        }


        [HttpGet]
        [Route("fornecedorRegiaoPrazo")]
        public HttpResponseMessage GetFornecedorRegiaoPrazo(HttpRequestMessage request, int estadoId, int regiaoId, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {

                HttpResponseMessage response = null;
                var listaAgrupaCorridosSemanais = new List<Cidade>();
                var listaIdCorridosSemanais = new List<int>();

                if (regiaoId == 0)
                {
                    var cidades = _cidadeRep.GetAll().Where(x => x.Estado.Id == estadoId)
                    .OrderBy(o => o.DescCidade).ToList();

                    var fornecedorRegiaoCidade =
                        _fornecedorRegiaoRep.GetAll()
                            .Where(f => f.FornecedorId == fornecedorId)
                            .Select(i => i.CidadeId)
                            .ToList();

                    var cidadesRegiaoForn = cidades.Where(c => fornecedorRegiaoCidade.Contains(c.Id));

                    IEnumerable<CidadeViewModel> cidadesVM = Mapper.Map<IEnumerable<Cidade>, IEnumerable<CidadeViewModel>>(cidadesRegiaoForn);

                    response = request.CreateResponse(HttpStatusCode.OK, cidadesVM);
                }
                else
                {
                    var cidades = _cidadeRep.GetAll().Where(x => x.Estado.Id == estadoId && x.Regiao.Id == regiaoId)
                    .ToList()
                    .OrderBy(o => o.DescCidade)
                    .ToList();

                    var fornecedorRegiaoCidade =
                        _fornecedorRegiaoRep.GetAll()
                            .Where(f => f.FornecedorId == fornecedorId)
                            .Select(i => i.CidadeId)
                            .ToList();

                    listaIdCorridosSemanais.AddRange(fornecedorRegiaoCidade);

                    var fornecedorRegiaoSemanal = _fornecedorPrazoSemanalRep.GetAll()
                    .Where(f => f.FornecedorId == fornecedorId)
                    .Select(i => i.CidadeId).ToList();

                    listaIdCorridosSemanais.AddRange(fornecedorRegiaoSemanal);
                    var unicosIdCorridosSemanal = listaIdCorridosSemanais.Distinct();

                    var cidadesRegiaoForn = cidades.Where(c => unicosIdCorridosSemanal.Contains(c.Id));

                    IEnumerable<CidadeViewModel> cidadesVM = Mapper.Map<IEnumerable<Cidade>, IEnumerable<CidadeViewModel>>(cidadesRegiaoForn);

                    response = request.CreateResponse(HttpStatusCode.OK, cidadesVM);
                }

                return response;
            });
        }

        [HttpGet]
        [Route("fornecedorRegiaoPrazoSemanal")]
        public HttpResponseMessage GetFornecedorRegiaoPrazoSemanal(HttpRequestMessage request, int estadoId, int regiaoId, int fornecedorId)
        {
            return CreateHttpResponse(request, () =>
            {

                HttpResponseMessage response = null;
                var listaIdCidades = new List<int>();

                if (regiaoId == 0)
                {
                    var cidades = _cidadeRep.GetAll().Where(x => x.Estado.Id == estadoId)
                    .OrderBy(o => o.DescCidade).ToList();

                    var fornecedorRegiaoCidadeSemanal =
                        _fornecedorPrazoSemanalRep.GetAll()
                            .Where(f => f.FornecedorId == fornecedorId)
                            .Select(i => i.CidadeId)
                            .ToList();

                    listaIdCidades.AddRange(fornecedorRegiaoCidadeSemanal);

                    var fornecedorRegiaoCorridos = _fornecedorRegiaoRep.GetAll()
                            .Where(f => f.FornecedorId == fornecedorId)
                            .Select(i => i.CidadeId)
                            .ToList();

                    listaIdCidades.AddRange(fornecedorRegiaoCorridos);

                    var listaIdCidadeUnicos = listaIdCidades.Distinct();

                    var cidadesRegiaoForn = cidades.Where(c => listaIdCidadeUnicos.Contains(c.Id));

                    IEnumerable<CidadeViewModel> cidadesVM = Mapper.Map<IEnumerable<Cidade>, IEnumerable<CidadeViewModel>>(cidadesRegiaoForn);

                    response = request.CreateResponse(HttpStatusCode.OK, cidadesVM);
                }
                else
                {
                    var cidades = _cidadeRep.GetAll().Where(x => x.Estado.Id == estadoId && x.Regiao.Id == regiaoId).ToList().OrderBy(o => o.DescCidade).ToList();

                    var fornecedorRegiaoCidadeSemanal =
                       _fornecedorPrazoSemanalRep.GetAll()
                           .Where(f => f.FornecedorId == fornecedorId)
                           .Select(i => i.CidadeId)
                           .ToList();

                    listaIdCidades.AddRange(fornecedorRegiaoCidadeSemanal);

                    var fornecedorRegiaoCorridos = _fornecedorRegiaoRep.GetAll()
                            .Where(f => f.FornecedorId == fornecedorId)
                            .Select(i => i.CidadeId)
                            .ToList();

                    listaIdCidades.AddRange(fornecedorRegiaoCorridos);

                    var listaIdCidadeUnicos = listaIdCidades.Distinct();

                    var cidadesRegiaoForn = cidades.Where(c => listaIdCidadeUnicos.Contains(c.Id));

                    IEnumerable<CidadeViewModel> cidadesVM = Mapper.Map<IEnumerable<Cidade>, IEnumerable<CidadeViewModel>>(cidadesRegiaoForn);

                    response = request.CreateResponse(HttpStatusCode.OK, cidadesVM);
                }

                return response;
            });
        }


    }
}
