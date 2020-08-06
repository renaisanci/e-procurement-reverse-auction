using System;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeStatus;
using ECC.EntidadeUsuario;
using ECC.EntidadeCotacao;
using ECC.EntidadePedido;
using ECC.EntidadeEmail;
using ECC.Servicos.Abstrato;
using ECC.EntidadePessoa;
using ECC.EntidadeSms;
using System.Collections.Generic;
using System.Linq;
using ECC.Servicos.Util;
using ECC.EntidadeAvisos;
using System.Threading.Tasks;

namespace ECC.Servicos
{
    public class CotacaoService : ICotacaoService
    {
        #region Variaveis
        private readonly IEntidadeBaseRep<MembroFornecedor> _membroFornecedorRep;
        private readonly IEntidadeBaseRep<Fornecedor> _FornecedorRep;
        private readonly IEntidadeBaseRep<Pedido> _pedidoRep;
        private readonly IEntidadeBaseRep<HistStatusPedido> _histStatusPedidoRep;
        private readonly IEntidadeBaseRep<ItemPedido> _itemPedidoRep;
        private readonly IEntidadeBaseRep<Cotacao> _cotacaoRep;
        private readonly IEntidadeBaseRep<HistStatusCotacao> _histStatusCotacaoRep;
        private readonly IEntidadeBaseRep<CotacaoPedidos> _cotacaoPedidoRep;
        private readonly IEntidadeBaseRep<ResultadoCotacao> _resultadoCotacaoRep;
        private readonly IEntidadeBaseRep<Usuario> _usuarioRep;
        private readonly IEntidadeBaseRep<StatusSistema> _statusSistemaRep;
        private readonly IEntidadeBaseRep<TemplateEmail> _templateEmail;
        private readonly IEntidadeBaseRep<Emails> _emailsNotificaoRep;
        private readonly IEntidadeBaseRep<Sms> _smsRep;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;
        private readonly IEntidadeBaseRep<Membro> _membroRep;
        private readonly IEncryptionService _encryptionService;
        private readonly INotificacoesAlertasService _notificacoesAlertasService;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Construtor

        public CotacaoService() : this(new DbFactory()) { }

        public CotacaoService(DbFactory dbFactory)
            : this(new EntidadeBaseRep<MembroFornecedor>(dbFactory), new EntidadeBaseRep<Pedido>(dbFactory),
                new EntidadeBaseRep<HistStatusPedido>(dbFactory), new EntidadeBaseRep<ItemPedido>(dbFactory),
                new EntidadeBaseRep<ResultadoCotacao>(dbFactory), new EntidadeBaseRep<Cotacao>(dbFactory),
                new EntidadeBaseRep<HistStatusCotacao>(dbFactory), new EntidadeBaseRep<CotacaoPedidos>(dbFactory),
                new EntidadeBaseRep<StatusSistema>(dbFactory),
                new EntidadeBaseRep<Fornecedor>(dbFactory),
                new EntidadeBaseRep<TemplateEmail>(dbFactory),
                new EntidadeBaseRep<Emails>(dbFactory),
                new EntidadeBaseRep<Sms>(dbFactory),
                new EntidadeBaseRep<Avisos>(dbFactory),
                new EntidadeBaseRep<Membro>(dbFactory),
                new EntidadeBaseRep<Usuario>(dbFactory), new EncryptionService(), new NotificacoesAlertasService(), new UnitOfWork(dbFactory))
        { }

        public CotacaoService(IEntidadeBaseRep<MembroFornecedor> membroFornecedorRep, IEntidadeBaseRep<Pedido> pedidoRep,
                IEntidadeBaseRep<HistStatusPedido> histStatusPedidoRep, IEntidadeBaseRep<ItemPedido> itemPedidoRep,
                IEntidadeBaseRep<ResultadoCotacao> resultadoCotacaoRep, IEntidadeBaseRep<Cotacao> cotacaoRep,
                IEntidadeBaseRep<HistStatusCotacao> histStatusCotacaoRep, IEntidadeBaseRep<CotacaoPedidos> cotacaoPedidosRep,
                IEntidadeBaseRep<StatusSistema> statusSistemaRep,
                IEntidadeBaseRep<Fornecedor> fornecedorRep,
                IEntidadeBaseRep<TemplateEmail> templateEmail,
                IEntidadeBaseRep<Emails> emailsNotificaoRep,
                IEntidadeBaseRep<Sms> smsRep,
                IEntidadeBaseRep<Avisos> avisosRep,
                IEntidadeBaseRep<Membro> membroRep,
                IEntidadeBaseRep<Usuario> usuarioRep, IEncryptionService encryptionService,  INotificacoesAlertasService notificacoesAlertasService, IUnitOfWork unitOfWork)
        {
            _membroFornecedorRep = membroFornecedorRep;
            _pedidoRep = pedidoRep;
            _histStatusPedidoRep = histStatusPedidoRep;
            _itemPedidoRep = itemPedidoRep;
            _resultadoCotacaoRep = resultadoCotacaoRep;
            _cotacaoRep = cotacaoRep;
            _histStatusCotacaoRep = histStatusCotacaoRep;
            _cotacaoPedidoRep = cotacaoPedidosRep;
            _statusSistemaRep = statusSistemaRep;
            _usuarioRep = usuarioRep;
            _encryptionService = encryptionService;
            _notificacoesAlertasService = notificacoesAlertasService;
            _FornecedorRep = fornecedorRep;
            _templateEmail = templateEmail;
            _emailsNotificaoRep = emailsNotificaoRep;
            _smsRep = smsRep;
            _avisosRep = avisosRep;
            _membroRep = membroRep;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public Cotacao BuscarCotacao(int cotacaoID)
        {
            return _cotacaoRep.FindBy(cr => cr.Id == cotacaoID).FirstOrDefault();
        }
        
        public List<CotacaoPedidos> ListarCotacaoPedidos()
        {
            // Status pedido em cotacao
            
            var statusId = _statusSistemaRep.GetAll().FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 2).Id;
        
            DateTime dataConsulta = DateTime.Now;

            List<CotacaoPedidos> listaCotacaoPedido = _cotacaoPedidoRep.FindBy(cpr => cpr.Ativo
                && cpr.Cotacao.DtFechamento < dataConsulta
                && cpr.Pedido.FlgCotado
                && cpr.Pedido.StatusSistemaId == statusId
                ).ToList();

            return listaCotacaoPedido;
        }

        public List<Pedido> ListarPedidos(int intOrdem)
        {
            var pedidos = _pedidoRep.FindBy(pr => pr.StatusSistema.Ordem == intOrdem).ToList();
            return pedidos;
        }

        public List<ResultadoCotacao> ListarResultadoCotacao(int cotacaoID)
        {
            List<ResultadoCotacao> lResultCotacao = _resultadoCotacaoRep.FindBy(rc => rc.CotacaoId == cotacaoID).ToList();
            return lResultCotacao;
        }

        public void SalvarPedido(Pedido pedidoSalvar)
        {
            Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);

            pedidoSalvar.UsuarioAlteracao = usuario;
            pedidoSalvar.DtAlteracao = DateTime.Now;

            _pedidoRep.Edit(pedidoSalvar);
            _unitOfWork.Commit();
        }


        public void AtualizarCotacao(Cotacao cotacao)
        {
            //Atualiza Cotação
            Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);

            cotacao.UsuarioAlteracao = usuario;
            cotacao.DtAlteracao = DateTime.Now;

            _cotacaoRep.Edit(cotacao);

            //Limpa Avisos para fornecedores pendentes dessa cotação
            var listaAviso = this._avisosRep.GetAll().Where(x => x.IdReferencia == cotacao.Id && x.TipoAvisosId == (int)TipoAviso.NovaCotacao);
            foreach (var aviso in listaAviso)
            {
                this._avisosRep.Delete(aviso);
            }


            //commit
            _unitOfWork.Commit();
        }

        public void InserirHistoricoCotacao(HistStatusCotacao histCotacao)
        {
            Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
            histCotacao.UsuarioCriacao = usuario;
            histCotacao.UsuarioCriacaoId = usuario.Id;

            _histStatusCotacaoRep.Add(histCotacao);
            _unitOfWork.Commit();
        }

        public void InserirHistoricoPedido(HistStatusPedido histPedido)
        {
            Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
            histPedido.UsuarioCriacao = usuario;

            _histStatusPedidoRep.Add(histPedido);
            _unitOfWork.Commit();
        }

        public void SalvarItemPedido(ItemPedido itemPedidoSalvar)
        {
            Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);

            itemPedidoSalvar.UsuarioAlteracao = usuario;
            itemPedidoSalvar.DtAlteracao = DateTime.Now;

            _itemPedidoRep.Edit(itemPedidoSalvar);
            _unitOfWork.Commit();
        }

        public List<Fornecedor> ListaFornecedoresPorMembro(int MembroID)
        {
            List<Fornecedor> retFornecedor = new List<Fornecedor>();
            var mfor = _membroFornecedorRep.FindBy(mf => mf.MembroId == MembroID);
            retFornecedor = mfor.Select(mf => mf.Fornecedor).ToList();
            return retFornecedor;
        }

        public Fornecedor BuscaFornecedorById(int FornedecorId)
        {
            Fornecedor retFornecedor = new Fornecedor();
            var fornec = _FornecedorRep.FindBy(f => f.Id == FornedecorId);
            retFornecedor = fornec.FirstOrDefault();
            return retFornecedor;
        }

        public StatusSistema BuscaStatusSistema(int pWorkflowStatusId, int pOrdem)
        {
            StatusSistema retStatusSis;
            retStatusSis = _statusSistemaRep.FindBy(ss => ss.WorkflowStatusId == pWorkflowStatusId && ss.Ordem == pOrdem).FirstOrDefault();
            return retStatusSis;
        }

        public void EmailNotificacaoFornecedor(List<Usuario> fornecedores, int CotacaoId)
        {
            try
            {
                Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
                var corpoEmail = _templateEmail.FindBy(e => e.Id == 16).Select(e => e.Template).FirstOrDefault();

                if (fornecedores.Count > 0)
                {
                    if (corpoEmail != null)
                    {
                        foreach (var fornecedor in fornecedores)
                        {
                            string corpoEmailFornecedor = corpoEmail.Replace("#IDCotacao#", CotacaoId.ToString());

                            if (_notificacoesAlertasService.PodeEnviarNotificacao(fornecedor.Id, 16, TipoAlerta.EMAIL))
                            {

                            Emails emails = new Emails
                            {
                                Ativo = true,
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                AssuntoEmail = "Cotação - Você teve o melhor preco na cotação " + CotacaoId + "",
                                EmailDestinatario = fornecedor.UsuarioEmail,
                                CorpoEmail = corpoEmailFornecedor.Trim(),
                                Status = Status.NaoEnviado,
                                Origem = Origem.MembroSolicitaFornecedor

                                };

                                //Envia EMAIL para fornecedor
                                _emailsNotificaoRep.Add(emails);

                                //Commit
                                _unitOfWork.Commit();
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SmsNotificacaoFornecedor(List<Usuario> fornecedores, int CotacaoId)
        {
            try
            {
                Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
                if (fornecedores.Count > 0)
                {
                    foreach (var userFornecedor in fornecedores)
                    {

                        //Verifica os numeros de celulares cadastrados para o usuario.
                        var NumTel = userFornecedor.Pessoa.Telefones.Where(t => t.UsuarioTelId == userFornecedor.Id && !string.IsNullOrEmpty(t.Celular.Trim()));
                        //valida se tem permissão de enviar SMS para usuario
                        if (_notificacoesAlertasService.PodeEnviarNotificacao(userFornecedor.Id, 16, TipoAlerta.SMS) && NumTel != null)
                        {
                            foreach (var itemNumTel in NumTel)
                            {
                                Sms sms = new Sms
                                {
                                    UsuarioCriacao = usuario,
                                    DtCriacao = DateTime.Now,
                                    Numero = itemNumTel.DddCel + itemNumTel.Celular,
                                    Mensagem = "Economiza Já - Cotação. Você teve o melhor preço na cotação " + CotacaoId + ".",
                                    Status = StatusSms.NaoEnviado,
                                    OrigemSms = TipoOrigemSms.NovaCotacao,
                                    Ativo = true
                                };

                                //Envia SMS para fonecedor
                                _smsRep.Add(sms);
                                _unitOfWork.Commit();
                            }
                            
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public void EmailNotificacaoMembro(List<Pedido> pedidos)
        {
            try
            {
                Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
                var corpoEmailComFornecedor = _templateEmail.FirstOrDefault(e => e.Id == 17).Template;
                var corpoEmailSemFornecedor = _templateEmail.FirstOrDefault(e => e.Id == 33).Template;

                int statusPedidoSemFornecedor = BuscaStatusSistema(12, 9).Id;

                if (pedidos.Count > 0)
                {
                    if (corpoEmailComFornecedor != null && corpoEmailSemFornecedor != null)
                    {
                        foreach (var pedido in pedidos)
                        {

                            List<Usuario> usuarios = _usuarioRep.GetAll().Where(u => u.PessoaId == pedido.Membro.PessoaId).ToList();

                            //Aviso e Email para Membro Pedido sem Fornecedor (Todos os itens sem valor)
                            if (pedido.StatusSistemaId == statusPedidoSemFornecedor)
                            {
                                foreach (var usu in usuarios)
                                {
                                    Emails emails = new Emails
                                    {
                                        Ativo = true,
                                        UsuarioCriacao = usuario,
                                        DtCriacao = DateTime.Now,
                                        AssuntoEmail = "Pedido " + pedido.Id + " com itens sem estoque.",
                                        EmailDestinatario = usu.UsuarioEmail,
                                        CorpoEmail = corpoEmailSemFornecedor.Trim(),
                                        Status = Status.NaoEnviado,
                                        Origem = Origem.NovaCotacao

                                    };
                                    //Envia EMAIL para fornecedor
                                    _emailsNotificaoRep.Add(emails);
                                }
                            }
                            //Aviso e Email para Membro Pedido Com Fornecedor
                            else
                            {
                                foreach (var usu in usuarios)
                                {
                                    var membro = _membroRep.FirstOrDefault(x => x.PessoaId == usu.PessoaId);

                                    if (_notificacoesAlertasService.PodeEnviarNotificacao(usu.Id, 1, TipoAlerta.EMAIL))
                                    {
                                        Emails emails = new Emails
                                        {
                                            UsuarioCriacao = usuario,
                                            DtCriacao = DateTime.Now,
                                          //  AssuntoEmail = "Aprovação Pedido - Corra e aprove seu pedido " + pedido.Id + ".",

                                            AssuntoEmail = "Corra aprove seu pedido " + pedido.Id + " valores validos por 2 dias, evite o cancelamento",
                                            EmailDestinatario = usu.UsuarioEmail,
                                            CorpoEmail = corpoEmailComFornecedor.Replace("#NomeFantasia#", membro.Pessoa.TipoPessoa==TipoPessoa.PessoaJuridica ? membro.Pessoa.PessoaJuridica.NomeFantasia: membro.Pessoa.PessoaFisica.Nome).Trim(),
                                            Status = Status.NaoEnviado,
                                            Origem = Origem.NovaCotacao

                                        };
                                        //Envia EMAIL para fornecedor
                                        _emailsNotificaoRep.Add(emails);
                                    }

                                    // Inserir Alerta Para Membro
                                    Avisos locAviso = new Avisos();
                                    locAviso.UsuarioCriacao = usuario;
                                    locAviso.DtCriacao = DateTime.Now;
                                    locAviso.Ativo = true;
                                    //Pedido = item,
                                    locAviso.IdReferencia = pedido.Id;
                                    locAviso.DataUltimoAviso = DateTime.Now;
                                    locAviso.ExibeNaTelaAvisos = true;
                                    locAviso.TipoAvisosId = (int)TipoAviso.PedidoPendentedeAceiteMembro;
                                    locAviso.URLPaginaDestino = "/#/meusPedidos";
                                    locAviso.TituloAviso = "Aceite Pendente";
                                    locAviso.ToolTip = "";
                                    locAviso.DescricaoAviso = "Aceite Pendente do pedido " + pedido.Id;
                                    locAviso.ModuloId = 3; //Modulo Membro
                                    locAviso.UsuarioNotificadoId = usu.Id;

                                    _avisosRep.Add(locAviso);
                                }
                            }

                            //Commit
                            _unitOfWork.Commit();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SmsNotificacaoMembro(List<Pedido> pedidos)
        {
            try
            {
                Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
                Sms sms = null;
                int statusPedidoSemFornecedor = BuscaStatusSistema(12, 9).Id;

                if (pedidos.Count > 0)
                {
                    foreach (var pedido in pedidos)
                    {
                        //SMS para Membro Pedido sem Fornecedor (Todos os itens sem valor)
                        if (pedido.StatusSistemaId == statusPedidoSemFornecedor)
                        {
                            sms = new Sms
                            {
                                Ativo = true,
                                UsuarioCriacao = usuario,
                                DtCriacao = DateTime.Now,
                                Numero = pedido.Membro.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + pedido.Membro.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                                Mensagem = "Economiza Já - Pedido "+ pedido.Id + " sem estoque.",
                                Status = StatusSms.NaoEnviado,
                                OrigemSms = TipoOrigemSms.NovaCotacao
                            };
                        }
                        else //SMS para Membro Pedido Com Fornecedor
                        {
                            //Verifica os numeros de celulares cadastrados para o usuario.
                            var NumTel = pedido.Membro.Pessoa.Telefones.Where(t => t.UsuarioTelId == pedido.Membro.Id && !string.IsNullOrEmpty(t.Celular.Trim()));
                            //valida se tem permissão de enviar SMS para usuario
                            if (_notificacoesAlertasService.PodeEnviarNotificacao(pedido.UsuarioCriacao.Id, 1, TipoAlerta.SMS))
                            {
                                sms = new Sms
                                {
                                    UsuarioCriacao = usuario,
                                    DtCriacao = DateTime.Now,
                                    Numero = pedido.Membro.Pessoa.Telefones.Select(t => t.DddCel).FirstOrDefault() + pedido.Membro.Pessoa.Telefones.Select(t => t.Celular).FirstOrDefault(),
                                   // Mensagem = "Economiza Já - Aprovação Pedido. Corra e aprove seu pedido "+ pedido.Id + ".",
                                    Mensagem = "Corra aprove seu pedido " + pedido.Id + " valores validos por 2 dias, evite o cancelamento",
                                    Status = StatusSms.NaoEnviado,
                                    OrigemSms = TipoOrigemSms.NovaCotacao,
                                    Ativo = true
                                };
                            }
                        }
                        //Envia SMS para fonecedor
                        _smsRep.Add(sms);
                        _unitOfWork.Commit();
                        
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
