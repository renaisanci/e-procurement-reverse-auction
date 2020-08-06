using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using System;
using System.Collections.Generic;
using System.Linq;
using Gerencianet.SDK;
using ECC.EntidadePedido;
using ECC.EntidadeParametroSistema;
using ECC.EntidadeSms;
using ECC.EntidadeEmail;

namespace ECC.Servicos
{
    public class CancelarPedidoMembroService : ICancelarPedidoMembroService
    {

        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<HistStatusPedido> _histStatusPedidoRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<Emails> _emailRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmailRep;
        private readonly IEntidadeBaseRep<Sms> _smsRep;

        private readonly IUnitOfWork _unitOfWork;

        #region Construtor
        public CancelarPedidoMembroService() : this(new DbFactory()) { }

        public CancelarPedidoMembroService(DbFactory dbFactory)

            : this(new EntidadeBaseRep<Pedido>(dbFactory),
                   new EntidadeBaseRep<HistStatusPedido>(dbFactory),
                   new EntidadeBaseRep<ParametroSistema>(dbFactory),
                   new EntidadeBaseRep<Usuario>(dbFactory),
                   new EntidadeBaseRep<Emails>(dbFactory),
                   new EntidadeBaseRep<TemplateEmail>(dbFactory),
                   new EntidadeBaseRep<Sms>(dbFactory),
                   new UnitOfWork(dbFactory))
        { }

        public CancelarPedidoMembroService(
                                IEntidadeBaseRep<Pedido> pedidoRep,
                                IEntidadeBaseRep<HistStatusPedido> histStatusPedidoRep,
                                IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
                                IEntidadeBaseRep<Usuario> usuarioRep,
                                IEntidadeBaseRep<Emails> emailRep,
                                IEntidadeBaseRep<TemplateEmail> templateEmailRep,
                                IEntidadeBaseRep<Sms> smsRep,
                                IUnitOfWork unitOfWork)
        {
            _pedidoRep = pedidoRep;
            _histStatusPedidoRep = histStatusPedidoRep;
            _parametroSistemaRep = parametroSistemaRep;
            _usuarioRep = usuarioRep;
            _emailRep = emailRep;
            _templateEmailRep = templateEmailRep;
            _smsRep = smsRep;
            _unitOfWork = unitOfWork;
        }

        public void CancelarPedidoMembro()
        {
            var usuarioSistema = _usuarioRep.FirstOrDefault(x => x.Id == 1);
            var diasParametroFechamentoPedido = Convert.ToInt64(_parametroSistemaRep.FindBy(x => x.Codigo.Equals("FECHAMENTO_PEDIDO")).FirstOrDefault().Valor);
            var pedidos = _pedidoRep.FindBy(x => x.StatusSistemaId == 23).ToList();

            if (pedidos.Count > 0)
            {
                pedidos.ForEach(x =>
                {

                    var nomeMembro = x.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaJuridica ? x.Membro.Pessoa.PessoaJuridica.NomeFantasia
                                                                                             : x.Membro.Pessoa.PessoaFisica.Nome;

                    var diasAguardandoAprovacao = (DateTime.Now - x.DtAlteracao.Value).Days;

                    var templateEmail = _templateEmailRep.GetSingle(44).Template
                                        .Replace("#NomeMembro#", nomeMembro)
                                        .Replace("#Id#", x.Id.ToString())
                                        .Replace("#DiasCancelamento#", diasParametroFechamentoPedido.ToString());

                if (diasAguardandoAprovacao >= diasParametroFechamentoPedido)
                {
                    x.StatusSistemaId = 36;   //Status do Pedido Cancelado
                    _pedidoRep.Edit(x);

                    var pedidoHistorico = new HistStatusPedido
                    {
                        UsuarioCriacaoId = usuarioSistema.Id,
                        UsuarioCriacao = usuarioSistema,
                        DtCriacao = DateTime.Now,
                        PedidoId = x.Id,
                        StatusSistemaId = 36,   //Status do Pedido Cancelado
                        DescMotivoCancelamento = $"Cancelamento efetuado pelo sistema, pois passou o prazo de {diasParametroFechamentoPedido} dias para aprovação.",
                        Ativo = true,
                    };

                    _histStatusPedidoRep.Add(pedidoHistorico);


                    Emails emails = new Emails
                    {
                        UsuarioCriacao = usuarioSistema,
                        DtCriacao = DateTime.Now,
                        AssuntoEmail = $"Pedido {x.Id} cancelado por falta de aprovação.",

                        //pega o email do usuario q criou o pedido para notifica-lo entidade UsuarioCriacao
                        EmailDestinatario =  x.UsuarioCriacao.UsuarioEmail,
                        CorpoEmail = templateEmail.Trim(),
                        Status = Status.NaoEnviado,
                        Origem = Origem.CancelamentoPedidoNaoAprovadoPeloMembro,
                        Ativo = true
                    };

                    _emailRep.Add(emails);

                        //pega o celular do usuario q criou o pedido, se caso no futuro uma empresa tiver mais de um usuario
                        // manda para o usuario q criou o pedido
                        var Cel = x.UsuarioCriacao.Telefones.Select(c => new { celular=c.DddCel + c.Celular }).FirstOrDefault();

                        Sms sms = new Sms
                        {
                            UsuarioCriacao = usuarioSistema,
                            DtCriacao = DateTime.Now,
                            Numero =  Cel.celular.ToString(),
                            Mensagem = $"Pedido {x.Id} cancelado por falta de aprovação.",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.CancelamentoPedidoNaoAprovadoPeloMembro,
                            Ativo = true
                        };
                        _smsRep.Add(sms);

                        _unitOfWork.Commit();

                    }

                });
            }

        }

        #endregion

        #region Métodos



        #endregion


    }
}
