using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadePedido;
using ECC.EntidadeRecebimento;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using ECC.EntidadeEmail;
using ECC.EntidadeEndereco;
using ECC.EntidadePessoa;
using ECC.EntidadeSms;
using Gerencianet.SDK;
using Gerencianet.SDK.Requests;
using ECC.Entidades.EntidadeRecebimento;
using Gerencianet.SDK.Responses;
using Newtonsoft.Json;

namespace ECC.Servicos
{
    public class PagamentoService : IPagamentoService
    {
        #region Variaveis

        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<Comissao> _comissaoRep;
        private readonly IEntidadeBaseRep<Fatura> _faturaRep;
        private readonly IEntidadeBaseRep<Mensalidade> _mensalidadeRep;
        private readonly IEntidadeBaseRep<MensalidadeDetalhe> _mensalidadeDetalheRep;
        private readonly IEntidadeBaseRep<ParametrosRecebimento> _parametrosRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEntidadeBaseRep<Fornecedor> _forneceRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateRep;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IEntidadeBaseRep<Sms> _smsRep;
        private readonly IEntidadeBaseRep<CartaoCredito> _cartaoCreditoRep;
        private readonly IEntidadeBaseRep<UsuarioCancelado> _usuarioCanceladoRep;


        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Construtor
        public PagamentoService() : this(new DbFactory()) { }

        public PagamentoService(DbFactory dbFactory)
            : this(new EntidadeBaseRep<Usuario>(dbFactory),
                   new EntidadeBaseRep<Pedido>(dbFactory),
                   new EntidadeBaseRep<Comissao>(dbFactory),
                   new EntidadeBaseRep<Fatura>(dbFactory),
                   new EntidadeBaseRep<Mensalidade>(dbFactory),
                   new EntidadeBaseRep<MensalidadeDetalhe>(dbFactory),
                   new EntidadeBaseRep<ParametrosRecebimento>(dbFactory),
                   new EntidadeBaseRep<Fornecedor>(dbFactory),
                   new EntidadeBaseRep<Membro>(dbFactory),
                   new EntidadeBaseRep<TemplateEmail>(dbFactory),
                   new EntidadeBaseRep<Emails>(dbFactory),
                   new EntidadeBaseRep<Sms>(dbFactory),
                   new EntidadeBaseRep<CartaoCredito>(dbFactory),
                   new EntidadeBaseRep<UsuarioCancelado>(dbFactory),
                   new EncryptionService(),
                   new UnitOfWork(dbFactory))
        { }

        public PagamentoService(IEntidadeBaseRep<Usuario> usuarioRep,
                                IEntidadeBaseRep<Pedido> pedidoRep,
                                IEntidadeBaseRep<Comissao> comissaoRep,
                                IEntidadeBaseRep<Fatura> faturaRep,
                                IEntidadeBaseRep<Mensalidade> mensalidadeRep,
                                IEntidadeBaseRep<MensalidadeDetalhe> mensalidadeDetalheRep,
                                IEntidadeBaseRep<ParametrosRecebimento> parametrosRep,
                                IEntidadeBaseRep<Fornecedor> fornecedorRep,
                                IEntidadeBaseRep<Membro> membroRep,
                                IEntidadeBaseRep<TemplateEmail> templateRep,
                                IEntidadeBaseRep<Emails> emailsRep,
                                IEntidadeBaseRep<Sms> smsRep,
                                IEntidadeBaseRep<CartaoCredito> cartaoCreditoRep,
                                IEntidadeBaseRep<UsuarioCancelado> usuarioCanceladoRep,
                                IEncryptionService encryptionService,
                                IUnitOfWork unitOfWork)
        {
            _usuarioRep = usuarioRep;
            _pedidoRep = pedidoRep;
            _comissaoRep = comissaoRep;
            _faturaRep = faturaRep;
            _mensalidadeRep = mensalidadeRep;
            _mensalidadeDetalheRep = mensalidadeDetalheRep;
            _forneceRep = fornecedorRep;
            _membroRep = membroRep;
            _templateRep = templateRep;
            _emailsRep = emailsRep;
            _smsRep = smsRep;
            _cartaoCreditoRep = cartaoCreditoRep;
            _usuarioCanceladoRep = usuarioCanceladoRep;
            _encryptionService = encryptionService;
            _unitOfWork = unitOfWork;
            _parametrosRep = parametrosRep;
            this.Parametros = _parametrosRep.GetSingle(1);

            this.ClienId = ConfigurationManager.AppSettings["ClientID"].ToString();
            this.ClientSecret = ConfigurationManager.AppSettings["ClientSecret"].ToString();
            this.Sandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["Sandbox"]);
        }

        #endregion

        #region Propiedades

        public ParametrosRecebimento Parametros { get; }
        public string ClienId { get; }
        public string ClientSecret { get; }
        public bool Sandbox { get; }

        #endregion

        #region Methods

        public DateTime DataProximoFechamento()
        {
            var dayNow = DateTime.Now.Day;
            var monthNow = DateTime.Now.Month;
            var yearNow = DateTime.Now.Year;

            //Se o mês atual for igual a dezembro e o dia atual for maior que o dia de fechamento,
            //Colocar o mês igual a janeiro e acrescentar 1 ano
            if (monthNow == 12 && dayNow > this.Parametros.FornecedorDiaFechamento)
            {
                monthNow = 1;
                yearNow++;
            }
            //Se o dia atual for maior que o dia de fechamento,
            //Acrescentar 1 mês
            else if (dayNow > this.Parametros.FornecedorDiaFechamento)
            {
                monthNow++;
            }

            //Data do próximo fechamento
            var proximoFechamento = new DateTime(yearNow, monthNow, this.Parametros.FornecedorDiaFechamento);
            return proximoFechamento;
        }

        public DateTime DataProximoVencimento()
        {
            var proximoFechamento = this.DataProximoFechamento();
            var month = proximoFechamento.Month;

            //Data do próximo fechamento
            var proximoVencimento = new DateTime(proximoFechamento.Year, ++month, this.Parametros.FornecedorDiaVencimento);

            //Se a data de vencimento da fatura cair no final de semana,
            //passa ela para o próximo dia útil
            switch (proximoVencimento.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    proximoVencimento.AddDays(2);
                    break;
                case DayOfWeek.Sunday:
                    proximoVencimento.AddDays(1);
                    break;
            }

            return proximoVencimento;
        }

        public void GerarComissao(int idUsuario, int idPedido, int idFornecedor)
        {
            var fatura = this._faturaRep.GetAll().FirstOrDefault(x => x.FornecedorId == idFornecedor && x.Status == StatusFatura.Parcial);

            if (fatura == null)
            {
                //Data do próximo fechamento
                var proximoFechamento = this.DataProximoFechamento();

                fatura = new Fatura
                {
                    Ativo = true,
                    DtCriacao = DateTime.Now,
                    UsuarioCriacaoId = idUsuario,
                    FornecedorId = idFornecedor,
                    Status = StatusFatura.Parcial,
                    DtFechamento = proximoFechamento,
                    DtVencimento = this.DataProximoVencimento()
                };

                this._faturaRep.Add(fatura);
            }
            else
            {
                fatura.DtAlteracao = DateTime.Now;
                fatura.UsuarioAlteracaoId = idUsuario;

                this._faturaRep.Edit(fatura);
            }

            var pedido = this._pedidoRep.GetSingle(idPedido);

            var formaPagamentoPedido = pedido.ItemPedidos.Select(x => x.FormaPagtoId).FirstOrDefault();

            var formaPagamentosFornecedor = pedido.ItemPedidos.Where(x => x.FornecedorId == idFornecedor)
              .Select(f => f.Fornecedor.FornecedorFormaPagtos.Where(p => p.FormaPagtoId == formaPagamentoPedido)
              .Select(d => d.Desconto)
              .FirstOrDefault())
              .FirstOrDefault();

            var totalVendas = pedido.ItemPedidos.Where(x => x.FornecedorId == idFornecedor
                                              && x.AprovacaoMembro
                                              && x.AprovacaoFornecedor)
                                              .Sum(x => (x.PrecoNegociadoUnit ?? 0) * x.Quantidade);

            var valorDesconto = (totalVendas * formaPagamentosFornecedor) / 100;

            totalVendas = totalVendas - valorDesconto;

            var comissaoValor = totalVendas * this.Parametros.FornecedorComissao / 100m;

            var comissao = fatura.Comissoes.FirstOrDefault(x => x.PedidoId == idPedido);

            if (comissao == null)
            {
                comissao = new Comissao
                {
                    Ativo = true,
                    DtCriacao = DateTime.Now,
                    UsuarioCriacaoId = idUsuario,
                    PedidoId = idPedido,
                    Valor = comissaoValor,
                    PedidoTotal = totalVendas,
                    Fatura = fatura
                };

                this._comissaoRep.Add(comissao);
            }
            else
            {
                comissao.DtAlteracao = DateTime.Now;
                comissao.UsuarioAlteracaoId = idUsuario;
                comissao.Valor = comissaoValor;
                comissao.PedidoTotal = totalVendas;
                this._comissaoRep.Edit(comissao);
            }

            _unitOfWork.Commit();
        }

        public void GerarFaturas()
        {
            var hoje = DateTime.Now.Date;
            var valorMinFatura = int.Parse(ConfigurationManager.AppSettings["ValorMinFatura"]);

            //Retorna todas as faturas com o status parcial e a data de fachamento menor que hoje
            var faturas = _faturaRep.GetAll().Where(x => x.Status == StatusFatura.Parcial && x.DtFechamento < hoje).ToList();

            //Efetua o fechamento das faturas selecionadas
            foreach (var fatura in faturas)
            {
                var fornecedor = _forneceRep.FirstOrDefault(x => x.Id == fatura.FornecedorId);
                var telefone = fornecedor.Pessoa.Telefones.FirstOrDefault(x => x.Ativo);
                var endereco = fornecedor.Pessoa.Enderecos.FirstOrDefault();

                var dataFinalPeriodoGratuito = fornecedor.DataFimPeriodoGratuito ?? fornecedor.DtCriacao;

                var faturaComissao = fatura.Comissoes.Count(f => f.DtCriacao > dataFinalPeriodoGratuito) > 0;

                //Montas os itens referentes a transação
                if (faturaComissao)
                {
                    var itens = fatura.Comissoes.Where(c => c.DtCriacao > dataFinalPeriodoGratuito).Select(x => new Item
                    {
                        Name = $"Pedido: {x.PedidoId}",
                        Value = (int)Convert.ToDecimal(string.Format("{0:N}", x.Valor).Replace(",", "").Replace(".", ""))
                    }).ToArray();

                    var configurations = new ConfigGerenciaNet
                    {
                        Fine = 200,
                        Interest = 33
                    };

                    var metadata = new Metadata
                    {
                        CustomId = fatura.Id.ToString(),
                        NotificationURL = $"{ConfigurationManager.AppSettings[$"{Environment.GetEnvironmentVariable("Amb_EconomizaJa")}_UrlRetorno"]}api/pagamentos/retornoFatura"
                    };

                    var transacao = new TransactionRequest
                    {
                        Items = itens,
                        Metadata = metadata
                    };

                    var valorFatura = itens.Sum(x => x.Value);

                    if (valorFatura > valorMinFatura)
                    {
                        var gerenciaNet = new GerencianetAPI();
                        //Cria a nova transação no gerencianet
                        var retornoTransacao = gerenciaNet.CreateTransaction(transacao);

                        //criando o boleto para pagamento
                        Payment payment = new Payment();
                        payment.BankingBillet = new BankingBillet();
                        payment.BankingBillet.ExpireAt = fatura.DtVencimento.ToString("yyyy-MM-dd");
                        payment.BankingBillet.Customer = new Customer();
                        payment.BankingBillet.Customer.Name = fornecedor.Pessoa.PessoaJuridica.RazaoSocial;
                        payment.BankingBillet.Customer.Birth = (fornecedor.Pessoa.PessoaJuridica.DtFundacao ?? DateTime.Now).ToString("yyyy-MM-dd");
                        payment.BankingBillet.Customer.Email = fornecedor.Pessoa.PessoaJuridica.Email;
                        payment.BankingBillet.Customer.PhoneNumber = $"{telefone?.DddTelComl}{telefone?.TelefoneComl}";
                        payment.BankingBillet.Customer.Address = new Address();
                        payment.BankingBillet.Customer.Address.ZipCode = endereco?.Cep;
                        payment.BankingBillet.Customer.Address.City = endereco.Cidade.DescCidade;
                        payment.BankingBillet.Customer.Address.Number = endereco.Numero.ToString();
                        payment.BankingBillet.Customer.Address.Neighborhood = endereco.Bairro.DescBairro;
                        payment.BankingBillet.Customer.Address.Complement = endereco.Complemento;
                        payment.BankingBillet.Customer.Address.State = endereco.Estado.Uf;
                        payment.BankingBillet.Customer.Address.Street = endereco.DescEndereco;
                        payment.BankingBillet.Customer.JuridicalPerson = new JuridicalPerson();
                        payment.BankingBillet.Customer.JuridicalPerson.CNPJ = fornecedor.Pessoa.PessoaJuridica.Cnpj;
                        payment.BankingBillet.Customer.JuridicalPerson.CorporateName = fornecedor.Pessoa.PessoaJuridica.RazaoSocial;
                        payment.BankingBillet.Configurations = configurations;

                        var retornoPagamento = gerenciaNet.CreateBankingBillet(retornoTransacao.ChargerId, payment);

                        fatura.Status = StatusFatura.Fechado;
                        fatura.DtAlteracao = DateTime.Now;
                        fatura.UsuarioAlteracaoId = 1;
                        fatura.ChargerId = retornoTransacao.ChargerId;
                        fatura.UrlBoleto = retornoPagamento.Pdf != null ? retornoPagamento.Pdf.Charge : retornoPagamento.Link;

                        #region Envia Email e Sms

                        Emails emails = new Emails
                        {
                            UsuarioCriacaoId = 1,
                            DtCriacao = DateTime.Now,
                            AssuntoEmail = "Boleto Gerado - Seu boleto já está disponível para pagamento.",
                            EmailDestinatario = fornecedor.Pessoa.PessoaJuridica.Email,

                            CorpoEmail = _templateRep.GetSingle(28).Template.Trim()
                            .Replace("#NomeFantasia#", fornecedor.Pessoa.PessoaJuridica.NomeFantasia)
                            .Replace("#UrlBoleto#", fatura.UrlBoleto),

                            Status = Status.NaoEnviado,
                            Origem = Origem.BoletoGerado,
                            Ativo = true

                        };

                        Sms sms = new Sms
                        {
                            UsuarioCriacaoId = 1,
                            DtCriacao = DateTime.Now,
                            Numero = fornecedor.Pessoa.Telefones.FirstOrDefault()?.DddCel + fornecedor.Pessoa.Telefones.FirstOrDefault()?.Celular,
                            Mensagem = "Economiza Já - Seu boleto está disponível. Segue o código de barras para pagamento " + retornoPagamento.BarCode + "",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.BoletoGerado,
                            Ativo = true

                        };

                        #endregion

                        this._faturaRep.Edit(fatura);
                        this._emailsRep.Add(emails);
                        this._smsRep.Add(sms);

                        _unitOfWork.Commit();


                    }
                }

            }
        }

        public void GerarMensalidades()
        {

            var membros = this._membroRep.GetAll().Where(x => x.Ativo).ToList();

            var data = DateTime.Now;
            var year = data.Year;
            var month = data.Month;
            var day = Parametros.MembroDiaFechamento;

            if (data.Day > day && month == 12)
            {
                year++;
                month = 1;
            }
            else if (data.Day > day)
                month++;

            var dtVencimento = new DateTime(year, month, day);
            var dtMovimentacao = dtVencimento.AddMonths(-1);

            foreach (var membro in membros)
            {
                var mensalidade = new Mensalidade
                {
                    DtCriacao = DateTime.Now,
                    UsuarioCriacaoId = 1,
                    Ativo = true,
                    MembroId = membro.Id,
                    DtVencimento = dtVencimento,
                    Status = StatusMensalidade.Gerado,
                    Descricao = $"Movimentacao referente a {dtMovimentacao:MM/YYYY}"
                };

                this._mensalidadeRep.Add(mensalidade);


                var detalhe = new MensalidadeDetalhe
                {
                    Descricao = $"Mensalidade referente a {dtMovimentacao:MM/YYYY}",
                    Valor = Parametros.MembroMensalidade,
                    Mensalidade = mensalidade,
                    DtCriacao = DateTime.Now,
                    UsuarioCriacaoId = 1,
                    Ativo = true
                };

                this._mensalidadeDetalheRep.Add(detalhe);

                if (!membro.Vip)
                {
                    var detalheVip = new MensalidadeDetalhe
                    {
                        Descricao = "Desconto VIP",
                        Valor = -Parametros.MembroMensalidade,
                        Mensalidade = mensalidade,
                        DtCriacao = DateTime.Now,
                        UsuarioCriacaoId = 1,
                        Ativo = true
                    };

                    this._mensalidadeDetalheRep.Add(detalheVip);

                }

                //Montas os itens referentes a transação
                var itens = mensalidade.Detalhes.Select(x => new Item
                {
                    Name = x.Descricao,
                    Value = int.Parse(x.Valor.ToString(CultureInfo.InvariantCulture).Replace(",", "").Replace(".", ""))
                }).ToArray();

                var metadata = new Metadata
                {
                    CustomId = mensalidade.Id.ToString(),
                    NotificationURL = $"{ConfigurationManager.AppSettings[$"{Environment.GetEnvironmentVariable("Amb_EconomizaJa")}_UrlRetorno"]}api/pagamentos/retornoMensalidade"
                };

                var transacao = new TransactionRequest
                {
                    Items = itens,
                    Metadata = metadata
                };

                _unitOfWork.Commit();

                var gerenciaNet = new GerencianetAPI();
                //Cria a nova transação no gerencianet
                var retornoTransacao = gerenciaNet.CreateTransaction(transacao);

                mensalidade.ChargerId = retornoTransacao.ChargerId;

                this._mensalidadeRep.Edit(mensalidade);

                _unitOfWork.Commit();

            }

        }

        public void ProcessarTrocasPlanosMembro()
        {
            var usuarioSistema = _usuarioRep.FirstOrDefault(x => x.Id == 1);
            var template = _templateRep.GetSingle(42).Template.Trim();
            dynamic response = new object();

            var _mensal = _mensalidadeRep.FindBy(x => x.DtEfetivarPlano != null && !x.Ativo).ToList();

            var mensalidades = _mensal.Where(x => x.DtEfetivarPlano.Value == DateTime.Now.Date).ToList();


            if (mensalidades.Count > 0)
            {
                mensalidades.ForEach(x =>
                {
                    var gerouPlano = false;
                    var tipoPagamento = x.Detalhes.FirstOrDefault().Tipo;
                    var usuarioMembro = x.Membro.Pessoa.Usuarios.FirstOrDefault(m => m.FlgMaster);


                    if (tipoPagamento == TipoMovimentacao.Credito)
                        gerouPlano = this.MensalidadeRecorrenteCartao(usuarioMembro, x.Membro, x);
                    else
                    {
                        response = this.MensalidadeRecorrenteBoleto(usuarioMembro, x.Membro, x);
                        gerouPlano = response != null;
                    }


                    if (gerouPlano)
                    {
                        x.Ativo = true;
                        var detalheMensalidade = x.Detalhes.FirstOrDefault();
                        detalheMensalidade.Ativo = true;
                        _mensalidadeRep.Edit(x);
                        _mensalidadeDetalheRep.Edit(detalheMensalidade);
                        _unitOfWork.Commit();

                        var nome = x.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica ? x.Membro.Pessoa.PessoaFisica.Nome :
                                                                                           x.Membro.Pessoa.PessoaJuridica.NomeFantasia;

                        template = template.Replace("#NomeFantasia#", nome).Replace("#UrlBoleto#", Convert.ToString(response.urlBoleto));

                        if (detalheMensalidade.Tipo == TipoMovimentacao.Boleto)
                        {
                            var email = new Emails
                            {
                                UsuarioCriacaoId = usuarioSistema.Id,
                                UsuarioCriacao = usuarioSistema,
                                DtCriacao = DateTime.Now,
                                EmailDestinatario = x.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica ? x.Membro.Pessoa.PessoaFisica.Email
                                                                                                          : x.Membro.Pessoa.PessoaJuridica.Email,
                                AssuntoEmail = $"Boleto Gerado - Seu boleto já está disponível para pagamento.",
                                CorpoEmail = template,
                                Status = Status.NaoEnviado,
                                Origem = Origem.BoletoGerado,
                                Ativo = true
                            };

                            Sms sms = new Sms
                            {
                                UsuarioCriacaoId = usuarioSistema.Id,
                                UsuarioCriacao = usuarioSistema,
                                DtCriacao = DateTime.Now,
                                Numero = x.Membro.Pessoa.Telefones.FirstOrDefault()?.DddCel + x.Membro.Pessoa.Telefones.FirstOrDefault()?.Celular,
                                Mensagem = "Economiza Já - Seu boleto está disponível. Segue o código de barras para pagamento " + response.codigoBarras,
                                Status = StatusSms.NaoEnviado,
                                OrigemSms = TipoOrigemSms.BoletoGerado,
                                Ativo = true

                            };

                            this._emailsRep.Add(email);
                            this._smsRep.Add(sms);

                            _unitOfWork.Commit();

                        }
                    }
                });
            }
        }

        public bool GerarMensalidadeMembro(Usuario usu, Membro membro, DateTime dataVencimento, TipoMovimentacao tipoPagamento)
        {
            var dtVencimentoFormatado = new DateTime(dataVencimento.Year, dataVencimento.Month, dataVencimento.Day);
            var dataFimPlano = membro.DataFimPeriodoGratuito.Value.AddMonths(membro.PlanoMensalidade.QtdMeses);
            var resultSuccessTransation = false;

            var mensalidade = new Mensalidade
            {
                DtCriacao = DateTime.Now,
                UsuarioCriacaoId = 1,
                Ativo = true,
                MembroId = membro.Id,
                DtVencimento = dtVencimentoFormatado,
                Status = StatusMensalidade.Gerado,
                PlanoMensalidadeId = membro.PlanoMensalidadeId.Value,
                Descricao = membro.PlanoMensalidade.Descricao

            };

            this._mensalidadeRep.Add(mensalidade);

            var detalhe = new MensalidadeDetalhe
            {
                Descricao = membro.PlanoMensalidade.Descricao,
                Valor = membro.PlanoMensalidade.Valor * membro.PlanoMensalidade.QtdMeses,
                Mensalidade = mensalidade,
                Tipo = tipoPagamento,
                DtCriacao = DateTime.Now,
                UsuarioCriacaoId = usu.Id,
                Ativo = true
            };

            this._mensalidadeDetalheRep.Add(detalhe);

            _unitOfWork.Commit();

            switch (tipoPagamento)
            {
                case TipoMovimentacao.Credito:

                    resultSuccessTransation = this.MensalidadeRecorrenteCartao(usu, membro, mensalidade);

                    break;

                case TipoMovimentacao.Boleto:

                    resultSuccessTransation = this.MensalidadeRecorrenteBoleto(usu, membro, mensalidade) != null;

                    break;
            }


            return resultSuccessTransation;
        }

        public DateTime TrocaPlano(Usuario usu, Membro membro, DateTime dataVencimento, TipoMovimentacao tipoPagamento)
        {
            var mensalidadePlano = new Mensalidade();
            bool esperaVencimentoPlano = false;
            var usuariosCancelados = _usuarioCanceladoRep.FirstOrDefault(x => x.Usuario.PessoaId == membro.PessoaId) == null;

            mensalidadePlano = membro.Mensalidades != null && membro.Mensalidades.Count > 0 ?
                               membro.Mensalidades.OrderByDescending(x => x.Id)
                                                  .FirstOrDefault(p => p.Status == StatusMensalidade.Recebido ||
                                                                       p.Status == StatusMensalidade.AguardandoPagamento) : null;

            var aguardandoPagamento = mensalidadePlano != null ? mensalidadePlano.Status == StatusMensalidade.AguardandoPagamento : false;

            if (mensalidadePlano != null)
            {
                // Cancela plano atual
                if (usuariosCancelados)
                    this.CancelarPlano(mensalidadePlano.Id);

                dataVencimento = aguardandoPagamento ? DateTime.Now.AddMonths(membro.PlanoMensalidade.QtdMeses) :
                                                       mensalidadePlano.DtVencimento.AddMonths(membro.PlanoMensalidade.QtdMeses);
                esperaVencimentoPlano = true;

            }
            else
            {
                mensalidadePlano = membro.Mensalidades != null && membro.Mensalidades.Count > 0 ?
                                    membro.Mensalidades.OrderByDescending(x => x.Id).FirstOrDefault() :
                                    null;

                if (mensalidadePlano != null)
                {
                    // Cancela plano atual
                    if (usuariosCancelados)
                        this.CancelarPlano(mensalidadePlano.Id);

                    dataVencimento = mensalidadePlano.DtVencimento.AddMonths(membro.PlanoMensalidade.QtdMeses);
                }
            }

            var mensalidade = new Mensalidade
            {
                DtCriacao = DateTime.Now,
                UsuarioCriacaoId = usu.Id,
                Ativo = false,
                MembroId = membro.Id,
                DtVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, dataVencimento.Day),
                Status = StatusMensalidade.Gerado,
                PlanoMensalidadeId = membro.PlanoMensalidadeId.Value,
                Descricao = membro.PlanoMensalidade.Descricao,
                DtEfetivarPlano = esperaVencimentoPlano ? mensalidadePlano.DtVencimento : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            };

            this._mensalidadeRep.Add(mensalidade);

            var detalhe = new MensalidadeDetalhe
            {
                Descricao = membro.PlanoMensalidade.Descricao,
                Valor = membro.PlanoMensalidade.Valor * membro.PlanoMensalidade.QtdMeses,
                Mensalidade = mensalidade,
                Tipo = tipoPagamento,
                DtCriacao = DateTime.Now,
                UsuarioCriacaoId = usu.Id,
                Ativo = false
            };

            this._mensalidadeDetalheRep.Add(detalhe);

            _unitOfWork.Commit();

            return mensalidade.DtEfetivarPlano.Value;
        }

        #endregion

        #region Métodos Complementares

        private bool GerarPagamentoBoleto(Usuario usu, Membro membro, Mensalidade mensalidade)
        {
            var telefone = usu.Pessoa.Telefones.FirstOrDefault(x => x.Ativo);
            var endereco = usu.Pessoa.Enderecos.FirstOrDefault(x => x.EnderecoPadrao);

            var configurations = new ConfigGerenciaNet
            {
                Fine = 200,
                Interest = 33
            };

            var itens = mensalidade.Detalhes.Select(x => new Item
            {
                Name = x.Descricao,
                Value = int.Parse(x.Valor.ToString(CultureInfo.InvariantCulture).Replace(",", "").Replace(".", ""))
            }).ToArray();

            var metadata = new Metadata
            {
                CustomId = mensalidade.Id.ToString(),
                NotificationURL = $"{ConfigurationManager.AppSettings[$"{Environment.GetEnvironmentVariable("Amb_EconomizaJa")}_UrlRetorno"]}api/pagamentos/retornoMensalidade"
            };

            var transacao = new TransactionRequest
            {
                Items = itens,
                Metadata = metadata
            };

            var gerenciaNet = new GerencianetAPI();

            //Cria a nova transação no gerencianet
            var retornoTransacao = gerenciaNet.CreateTransaction(transacao);

            mensalidade.ChargerId = retornoTransacao.ChargerId;

            this._mensalidadeRep.Edit(mensalidade);
            _unitOfWork.Commit();

            Payment payment = new Payment();
            payment.BankingBillet = new BankingBillet();
            payment.BankingBillet.ExpireAt = mensalidade.DtVencimento.ToString("yyyy-MM-dd");
            payment.BankingBillet.Customer = new Customer();
            payment.BankingBillet.Customer.Name = usu.UsuarioNome;

            payment.BankingBillet.Customer.Birth = membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                (membro.Pessoa.PessoaJuridica.DtFundacao ?? DateTime.Now).ToShortDateString() :
                DateTime.Now.ToShortDateString();

            payment.BankingBillet.Customer.Email = membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                                                   membro.Pessoa.PessoaJuridica.Email : membro.Pessoa.PessoaFisica.Email;

            payment.BankingBillet.Customer.PhoneNumber = $"{telefone?.DddTelComl}{telefone?.TelefoneComl}";

            payment.BankingBillet.Customer.Address = new Address();
            payment.BankingBillet.Customer.Address.ZipCode = endereco?.Cep;
            payment.BankingBillet.Customer.Address.City = endereco.Cidade.DescCidade;
            payment.BankingBillet.Customer.Address.Number = endereco.Numero.ToString();
            payment.BankingBillet.Customer.Address.Neighborhood = endereco.Bairro.DescBairro;
            payment.BankingBillet.Customer.Address.Complement = endereco.Complemento;
            payment.BankingBillet.Customer.Address.State = endereco.Estado.Uf;
            payment.BankingBillet.Customer.Address.Street = endereco.DescEndereco;
            payment.BankingBillet.Customer.JuridicalPerson = new JuridicalPerson();
            payment.BankingBillet.Customer.JuridicalPerson.CNPJ = membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                membro.Pessoa.PessoaJuridica.Cnpj : usu.Pessoa.PessoaFisica.Cpf;
            payment.BankingBillet.Customer.JuridicalPerson.CorporateName = membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                membro.Pessoa.PessoaJuridica.RazaoSocial : membro.Pessoa.PessoaFisica.Nome;
            payment.BankingBillet.Configurations = configurations;

            var retornoPagamento = gerenciaNet.CreateBankingBillet(retornoTransacao.ChargerId, payment);


            mensalidade.ChargerId = retornoPagamento.ChargeId;
            mensalidade.UrlPdf = retornoPagamento.Pdf != null ? retornoPagamento.Pdf.Charge : string.Empty;
            mensalidade.Status = StatusMensalidade.AguardandoPagamento;
            mensalidade.UsuarioCriacaoId = usu.Id;
            mensalidade.DtAlteracao = DateTime.Now;

            _mensalidadeRep.Edit(mensalidade);
            _unitOfWork.Commit();


            return !string.IsNullOrEmpty(retornoPagamento.Link);
        }

        private bool GerarPagamentoCartao(Usuario usu, Membro membro, Mensalidade mensalidade)
        {

            var telefone = usu.Pessoa.Telefones.FirstOrDefault(x => x.Ativo);
            var endereco = usu.Pessoa.Enderecos.FirstOrDefault();
            var cartao = _cartaoCreditoRep.FirstOrDefault(x => x.MembroId == membro.Id && x.Padrao);

            var itens = mensalidade.Detalhes.Select(x => new Item
            {
                Name = x.Descricao,
                Value = int.Parse(x.Valor.ToString(CultureInfo.InvariantCulture).Replace(",", "").Replace(".", ""))

            }).ToArray();

            var metadata = new Metadata
            {
                CustomId = mensalidade.Id.ToString(),
                NotificationURL = $"{ConfigurationManager.AppSettings[$"{Environment.GetEnvironmentVariable("Amb_EconomizaJa")}_UrlRetorno"]}api/pagamentos/retornoMensalidade"
            };

            var transacao = new TransactionRequest
            {
                Items = itens,
                Metadata = metadata
            };

            var gerenciaNet = new GerencianetAPI();

            //Cria a nova transação no gerencianet
            var retornoTransacao = gerenciaNet.CreateTransaction(transacao);

            mensalidade.ChargerId = retornoTransacao.ChargerId;

            this._mensalidadeRep.Edit(mensalidade);
            _unitOfWork.Commit();

            Payment payment = new Payment();

            payment.CreditCard = new CreditCard();
            payment.CreditCard.PaymentToken = cartao.TokenCartaoGerenciaNet;
            payment.CreditCard.Customer = new Customer();
            payment.CreditCard.Customer.PhoneNumber = $"{telefone?.DddTelComl}{telefone?.TelefoneComl}";
            payment.CreditCard.Customer.Birth = membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                (membro.Pessoa.PessoaJuridica.DtFundacao ?? DateTime.Now).ToShortDateString() :
                DateTime.Now.ToShortDateString();
            payment.CreditCard.Customer.Email = membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ?
                                                   membro.Pessoa.PessoaJuridica.Email : membro.Pessoa.PessoaFisica.Email;
            //Endereço de Entrega -> Opcional
            payment.CreditCard.Customer.Address = new Address();
            payment.CreditCard.Customer.Address.ZipCode = endereco?.Cep;
            payment.CreditCard.Customer.Address.City = endereco.Cidade.DescCidade;
            payment.CreditCard.Customer.Address.Number = endereco.Numero.ToString();
            payment.CreditCard.Customer.Address.Neighborhood = endereco.Bairro.DescBairro;
            payment.CreditCard.Customer.Address.Complement = endereco.Complemento;
            payment.CreditCard.Customer.Address.State = endereco.Estado.Uf;
            payment.CreditCard.Customer.Address.Street = endereco.DescEndereco;

            //Endereço de Cobrança -> Obrigatório
            payment.CreditCard.BillingAddress = new Address();
            payment.CreditCard.BillingAddress.ZipCode = endereco?.Cep;
            payment.CreditCard.BillingAddress.City = endereco.Cidade.DescCidade;
            payment.CreditCard.BillingAddress.Number = endereco.Numero.ToString();
            payment.CreditCard.BillingAddress.Neighborhood = endereco.Bairro.DescBairro;
            payment.CreditCard.BillingAddress.Complement = endereco.Complemento;
            payment.CreditCard.BillingAddress.State = endereco.Estado.Uf;
            payment.CreditCard.BillingAddress.Street = endereco.DescEndereco;

            if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
            {
                payment.CreditCard.Customer.JuridicalPerson = new JuridicalPerson();
                payment.CreditCard.Customer.JuridicalPerson.CNPJ = membro.Pessoa.PessoaJuridica.Cnpj;
                payment.CreditCard.Customer.JuridicalPerson.CorporateName = membro.Pessoa.PessoaJuridica.RazaoSocial;
            }
            else
            {
                payment.CreditCard.Customer.Name = membro.Pessoa.PessoaFisica.Nome;
                payment.CreditCard.Customer.CPF = membro.Pessoa.PessoaFisica.Cpf;
            }

            var retornoPagamento = gerenciaNet.CreateBankingBillet(retornoTransacao.ChargerId, payment);

            mensalidade.ChargerId = retornoPagamento.ChargeId;
            mensalidade.Status = StatusMensalidade.AguardandoPagamento;

            _mensalidadeRep.Edit(mensalidade);

            _unitOfWork.Commit();

            return mensalidade.ChargerId > 0;
        }

        private object MensalidadeRecorrenteBoleto(Usuario usu, Membro membro, Mensalidade mensalidade)
        {

            dynamic _endPoint = new Endpoints(this.ClienId, this.ClientSecret, this.Sandbox);

            var telefone = usu.Pessoa.Telefones.FirstOrDefault(x => x.Ativo);
            var endereco = usu.Pessoa.Enderecos.FirstOrDefault(x => x.EnderecoPadrao);

            try
            {
                var plan = new
                {
                    name = membro.PlanoMensalidade.Descricao,
                    interval = membro.PlanoMensalidade.QtdMeses
                };

                var itensMensalidade = mensalidade.Detalhes.Select(x => new Item
                {
                    Name = x.Descricao,
                    Value = int.Parse(x.Valor.ToString(CultureInfo.InvariantCulture).Replace(",", "").Replace(".", ""))
                }).ToArray();

                var metadataMensalidade = new
                {
                    custom_id = mensalidade.Id.ToString(),
                    notification_url = $"{ConfigurationManager.AppSettings[$"{Environment.GetEnvironmentVariable("Amb_EconomizaJa")}_UrlRetorno"]}api/pagamentos/retornoMensalidade"
                };

                var transacao = new
                {
                    items = itensMensalidade,
                    metadata = metadataMensalidade
                };

                // Criando o assinatura
                var planResponse = _endPoint.CreatePlan(null, plan);

                var param = new { id = planResponse.data.plan_id };

                // Adicionando outros dados na assinatura
                var responsePlano = _endPoint.CreateSubscription(param, transacao);

                var subscriptionParam = new
                {
                    subscriptionId = responsePlano.data.subscription_id,
                    chargerId = responsePlano.data.charges[0].charge_id
                };

                var body = this.PessoaJuricaFisicaBolelto(membro, usu, telefone, endereco, mensalidade);

                var paramPaySubscription = new { id = (int)subscriptionParam.subscriptionId };

                // Adicionando dados de pagamento na assinatura
                var responseCreatePayment = _endPoint.PaySubscription(paramPaySubscription, body);

                mensalidade.ChargerId = (int)responseCreatePayment.data.subscription_id;
                mensalidade.Status = StatusMensalidade.AguardandoPagamento;
                mensalidade.UrlPdf = Convert.ToString(responseCreatePayment.data.pdf.charge);

                _mensalidadeRep.Edit(mensalidade);
                _unitOfWork.Commit();

                var response = new
                {

                    codigoBarras = responseCreatePayment.data.barcode,
                    urlBoleto = responseCreatePayment.data.pdf.charge
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar pagamento recorrente\n", ex);
            }

        }

        private bool MensalidadeRecorrenteCartao(Usuario usu, Membro membro, Mensalidade mensalidade)
        {

            dynamic _endPoint = new Endpoints(this.ClienId, this.ClientSecret, this.Sandbox);

            var telefone = usu.Pessoa.Telefones.FirstOrDefault(x => x.Ativo);
            var endereco = usu.Pessoa.Enderecos.FirstOrDefault(x => x.EnderecoPadrao);
            var cartao = _cartaoCreditoRep.FirstOrDefault(x => x.MembroId == membro.Id && x.Padrao);

            try
            {
                var plan = new
                {
                    name = membro.PlanoMensalidade.Descricao,
                    interval = membro.PlanoMensalidade.QtdMeses
                };


                var itensMensalidade = mensalidade.Detalhes.Select(x => new Item
                {
                    Name = x.Descricao,
                    Value = int.Parse(x.Valor.ToString(CultureInfo.InvariantCulture).Replace(",", "").Replace(".", ""))
                }).ToArray();

                var metadataMensalidade = new
                {
                    custom_id = mensalidade.Id.ToString(),
                    notification_url = $"{ConfigurationManager.AppSettings[$"{Environment.GetEnvironmentVariable("Amb_EconomizaJa")}_UrlRetorno"]}api/pagamentos/retornoMensalidade"
                };

                var transacao = new
                {
                    items = itensMensalidade,
                    metadata = metadataMensalidade
                };

                // Criando o assinatura
                var planResponse = _endPoint.CreatePlan(null, plan);

                var param = new { id = planResponse.data.plan_id };

                // Adicionando outros dados na assinatura
                var responsePlano = _endPoint.CreateSubscription(param, transacao);

                var subscriptionParam = new
                {
                    subscriptionId = responsePlano.data.subscription_id,
                    chargerId = responsePlano.data.charges[0].charge_id
                };

                var body = this.PessoaJuricaFisicaCartao(membro, usu, telefone, endereco, cartao);

                var paramPaySubscription = new { id = (int)subscriptionParam.subscriptionId };

                // Adicionando dados de pagamento na assinatura
                var responseCreatePayment = _endPoint.PaySubscription(paramPaySubscription, body);

                mensalidade.ChargerId = (int)responseCreatePayment.data.subscription_id;
                mensalidade.Status = StatusMensalidade.AguardandoPagamento;

                _mensalidadeRep.Edit(mensalidade);
                _unitOfWork.Commit();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar pagamento recorrente\n", ex);
            }

        }

        public object PessoaJuricaFisicaCartao(Membro membro, Usuario usu, Telefone telefone, Endereco endereco, CartaoCredito cartao)
        {
            var body = new object();

            if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
            {
                body = new
                {
                    payment = new
                    {
                        credit_card = new
                        {
                            payment_token = cartao.TokenCartaoGerenciaNet, // see credit card flow to see how to get this
                            billing_address = new
                            {
                                street = endereco.DescEndereco,
                                number = endereco.Numero.ToString(),
                                neighborhood = endereco.Bairro.DescBairro,
                                zipcode = endereco?.Cep,
                                city = endereco.Cidade.DescCidade,
                                complement = endereco.Complemento,
                                state = endereco.Estado.Uf
                            },
                            customer = new
                            {
                                name = membro.Pessoa.PessoaJuridica.RazaoSocial,
                                email = membro.Pessoa.PessoaJuridica.Email,
                                phone_number = $"{telefone?.DddTelComl}{telefone?.TelefoneComl}",
                                birth = (membro.Pessoa.PessoaJuridica.DtFundacao ?? DateTime.Now).ToString("yyyy-MM-dd"),
                                juridical_person = new
                                {
                                    corporate_name = membro.Pessoa.PessoaJuridica.RazaoSocial,
                                    cnpj = membro.Pessoa.PessoaJuridica.Cnpj
                                }
                            }
                        }

                    }
                };
            }
            else
            {
                body = new
                {
                    payment = new
                    {
                        credit_card = new
                        {
                            payment_token = cartao.TokenCartaoGerenciaNet, // see credit card flow to see how to get this
                            billing_address = new
                            {
                                street = endereco.DescEndereco,
                                number = endereco.Numero.ToString(),
                                neighborhood = endereco.Bairro.DescBairro,
                                zipcode = endereco?.Cep,
                                city = endereco.Cidade.DescCidade,
                                complement = endereco.Complemento,
                                state = endereco.Estado.Uf
                            },
                            customer = new
                            {
                                name = membro.Pessoa.PessoaFisica.Nome,
                                email = membro.Pessoa.PessoaFisica.Email,
                                cpf = membro.Pessoa.PessoaFisica.Cpf,
                                phone_number = $"{telefone?.DddTelComl}{telefone?.TelefoneComl}",
                                birth = (membro.Pessoa.PessoaFisica.DtNascimento ?? membro.DtCriacao).ToString("yyyy-MM-dd")
                            }
                        }

                    }
                };
            }



            return body;
        }

        public object PessoaJuricaFisicaBolelto(Membro membro, Usuario usu, Telefone telefone, Endereco endereco, Mensalidade mensalidade)
        {
            var body = new object();

            if (membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica)
            {
                body = new
                {
                    payment = new
                    {
                        banking_billet = new
                        {
                            customer = new
                            {
                                name = membro.Pessoa.PessoaJuridica.RazaoSocial,
                                email = membro.Pessoa.PessoaJuridica.Email,
                                phone_number = $"{telefone?.DddTelComl}{telefone?.TelefoneComl}",
                                birth = (membro.Pessoa.PessoaJuridica.DtFundacao ?? DateTime.Now).ToString("yyyy-MM-dd"),
                                address = new
                                {
                                    street = endereco.DescEndereco,
                                    number = endereco.Numero.ToString(),
                                    neighborhood = endereco.Bairro.DescBairro,
                                    zipcode = endereco?.Cep,
                                    city = endereco.Cidade.DescCidade,
                                    complement = endereco.Complemento,
                                    state = endereco.Estado.Uf
                                },
                                juridical_person = new
                                {
                                    corporate_name = membro.Pessoa.PessoaJuridica.RazaoSocial,
                                    cnpj = membro.Pessoa.PessoaJuridica.Cnpj
                                }
                            },
                            expire_at = mensalidade.DtVencimento.ToString("yyyy-MM-dd"),
                            configurations = new
                            {
                                fine = 200,
                                interest = 33
                            }

                        }

                    }
                };
            }
            else
            {
                body = new
                {
                    payment = new
                    {
                        banking_billet = new
                        {
                            customer = new
                            {
                                name = membro.Pessoa.PessoaFisica.Nome,
                                email = membro.Pessoa.PessoaFisica.Email,
                                phone_number = $"{telefone?.DddTelComl}{telefone?.TelefoneComl}",
                                cpf = membro.Pessoa.PessoaFisica.Cpf,
                                birth = (membro.Pessoa.PessoaFisica.DtNascimento ?? membro.DtCriacao).ToString("yyyy-MM-dd"),
                                address = new
                                {
                                    street = endereco.DescEndereco,
                                    number = endereco.Numero.ToString(),
                                    neighborhood = endereco.Bairro.DescBairro,
                                    zipcode = endereco?.Cep,
                                    city = endereco.Cidade.DescCidade,
                                    complement = endereco.Complemento,
                                    state = endereco.Estado.Uf
                                }
                            },
                            expire_at = mensalidade.DtVencimento.ToString("yyyy-MM-dd"),
                            configurations = new
                            {
                                fine = 200,
                                interest = 33
                            }
                        }
                    }
                };
            }



            return body;
        }

        public bool CancelarPlano(int mensalidadeId)
        {
            dynamic _endPoint = new Endpoints(this.ClienId, this.ClientSecret, this.Sandbox);

            var mensalidade = _mensalidadeRep.FirstOrDefault(x => x.Id == mensalidadeId);

            var param = new
            {
                id = mensalidade.ChargerId
            };

            try
            {
                var response = _endPoint.CancelSubscription(param);

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cancelar assinatura.\n", ex);
            }

            return true;
        }

        public bool VerificaPlanoMembro(int idPessoa)
        {
            bool retorno = false;

            bool membroForaGratuidade = false;
            bool membroFezPedido = false;
            bool membroMensalidadePaga = false;
            var dataHoje = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            var _membro = _membroRep.FirstOrDefault(x => x.PessoaId == idPessoa);
            var _pedido = _pedidoRep.FirstOrDefault(x => x.MembroId == _membro.Id && x.StatusSistemaId > 23);


            membroForaGratuidade = _membro.DataFimPeriodoGratuito != null ? _membro.DataFimPeriodoGratuito.Value <= DateTime.Now && !_membro.Vip : !_membro.Vip;

            membroFezPedido = _pedido == null;

            membroMensalidadePaga = _membro.Mensalidades != null ?
                                    (_membro.Mensalidades
                                    .OrderByDescending(x => x.Id)
                                    .FirstOrDefault(x => x.Status == StatusMensalidade.Recebido &&
                                                         x.DtVencimento.AddMonths(_membro.PlanoMensalidade.QtdMeses) >= dataHoje) != null) :
                                    false;


            if (membroForaGratuidade)
            {
                // Período gratuito expirado e mensalidade não paga
                if (membroForaGratuidade && !membroMensalidadePaga)
                    return true;

                // Período gratuito expirado, mensalidade não foi gerada ou não paga, já fez pedido
                if (membroForaGratuidade && !membroMensalidadePaga && !membroFezPedido)
                    return true;

                // Fez pedido e mensalidade não paga
                if (!membroFezPedido && !membroMensalidadePaga)
                    return true;
            }


            return retorno;
        }

        public bool VerificaFaturasFornecedor(int idPessoa)
        {
            bool retorno = false;

            retorno = _forneceRep.GetAll()
                      .Where(x => x.PessoaId == idPessoa).SelectMany(p => p.Faturas)
                      .Any(p => (p.DtVencimento < DateTime.Now && p.Status == StatusFatura.AguardandoPagamento) || p.Status == StatusFatura.NaoPago);

            return retorno;
        }

        public NotificationsResponse ReturnStatusFatura(string notification)
        {
            dynamic _endPoint = new Endpoints(this.ClienId, this.ClientSecret, this.Sandbox);

            var param = new { token = notification };

            var response = _endPoint.GetNotification(param);

            var ultimoObjetoArray = response.data;

            var objeto = ultimoObjetoArray[ultimoObjetoArray.Count - 1];

            return JsonConvert.DeserializeObject<NotificationsResponse>(objeto.ToString());

        }

        #endregion
    }
}
