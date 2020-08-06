using System;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeUsuario;
using ECC.Servicos.Abstrato;
using ECC.EntidadePessoa;
using System.Collections.Generic;
using System.Linq;
using ECC.EntidadeAvisos;
using ECC.EntidadeCotacao;
using ECC.EntidadeEmail;
using ECC.EntidadePedido;
using ECC.EntidadeSms;
using ECC.EntidadeStatus;
using ECC.Servicos.Util;
using ECC.EntidadeParametroSistema;
using ECC.EntidadeRecebimento;
using ECC.EntidadeRobos;
using ECC.EntidadeProduto;

namespace ECC.Servicos
{
    public class NotificacoesAlertasService : INotificacoesAlertasService
    {
        #region Variaveis
        private readonly IEntidadeBaseRep<Membro> _membroRep;
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
        private readonly IEntidadeBaseRep<TipoAvisos> _tipoAvisosRep;
        private readonly IEntidadeBaseRep<Notificacao> _notificacaoRep;
        private readonly IEntidadeBaseRep<UsuarioNotificacao> _usuarioNotificacaoRep;
        private readonly IEntidadeBaseRep<Avisos> _avisosRep;
        private readonly IEntidadeBaseRep<ParametroSistema> _parametroSistemaRep;
        private readonly IEntidadeBaseRep<ExecucaoRobo> _execucaoRoboRep;
        private readonly IEntidadeBaseRep<Franquia> _franquiaRep;
        private readonly IEntidadeBaseRep<MembroCategoria> _membroCategoriaRep;
        private readonly IEntidadeBaseRep<Emails> _emailsRep;
        private readonly IEncryptionService _encryptionService;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Construtor

        public NotificacoesAlertasService() : this(new DbFactory()) { }

        public NotificacoesAlertasService(DbFactory dbFactory)
            : this(
                new EntidadeBaseRep<Membro>(dbFactory),
                new EntidadeBaseRep<MembroFornecedor>(dbFactory),
                new EntidadeBaseRep<Pedido>(dbFactory),
                new EntidadeBaseRep<HistStatusPedido>(dbFactory),
                new EntidadeBaseRep<ItemPedido>(dbFactory),
                new EntidadeBaseRep<ResultadoCotacao>(dbFactory),
                new EntidadeBaseRep<Cotacao>(dbFactory),
                new EntidadeBaseRep<HistStatusCotacao>(dbFactory),
                new EntidadeBaseRep<CotacaoPedidos>(dbFactory),
                new EntidadeBaseRep<StatusSistema>(dbFactory),
                new EntidadeBaseRep<Fornecedor>(dbFactory),
                new EntidadeBaseRep<TemplateEmail>(dbFactory),
                new EntidadeBaseRep<Emails>(dbFactory),
                new EntidadeBaseRep<Sms>(dbFactory),
                new EntidadeBaseRep<TipoAvisos>(dbFactory),
                new EntidadeBaseRep<Notificacao>(dbFactory),
                new EntidadeBaseRep<UsuarioNotificacao>(dbFactory),
                new EntidadeBaseRep<Avisos>(dbFactory),
                new EntidadeBaseRep<Usuario>(dbFactory),
                new EntidadeBaseRep<ParametroSistema>(dbFactory),
                new EntidadeBaseRep<ExecucaoRobo>(dbFactory),
                new EntidadeBaseRep<Franquia>(dbFactory),
                new EntidadeBaseRep<MembroCategoria>(dbFactory),
                new EntidadeBaseRep<Emails>(dbFactory),
                new EncryptionService(),
                new UnitOfWork(dbFactory))
        { }

        public NotificacoesAlertasService(
                IEntidadeBaseRep<Membro> membroRep,
                IEntidadeBaseRep<MembroFornecedor> membroFornecedorRep,
                IEntidadeBaseRep<Pedido> pedidoRep,
                IEntidadeBaseRep<HistStatusPedido> histStatusPedidoRep,
                IEntidadeBaseRep<ItemPedido> itemPedidoRep,
                IEntidadeBaseRep<ResultadoCotacao> resultadoCotacaoRep,
                IEntidadeBaseRep<Cotacao> cotacaoRep,
                IEntidadeBaseRep<HistStatusCotacao> histStatusCotacaoRep,
                IEntidadeBaseRep<CotacaoPedidos> cotacaoPedidosRep,
                IEntidadeBaseRep<StatusSistema> statusSistemaRep,
                IEntidadeBaseRep<Fornecedor> fornecedorRep,
                IEntidadeBaseRep<TemplateEmail> templateEmail,
                IEntidadeBaseRep<Emails> emailsNotificaoRep,
                IEntidadeBaseRep<Sms> smsRep,
                IEntidadeBaseRep<TipoAvisos> tipoAvisosRep,
                IEntidadeBaseRep<Notificacao> notificacaoRep,
                IEntidadeBaseRep<UsuarioNotificacao> usuarioNotificacaoRep,
                IEntidadeBaseRep<Avisos> avisosRep,
                IEntidadeBaseRep<Usuario> usuarioRep,
                IEntidadeBaseRep<ParametroSistema> parametroSistemaRep,
                IEntidadeBaseRep<ExecucaoRobo> execucaoRoboRep,
                IEntidadeBaseRep<Franquia> franquiaRep,
                IEntidadeBaseRep<MembroCategoria> membroCategoriaRep,
                IEntidadeBaseRep<Emails> emailsRep,
                IEncryptionService encryptionService,
                IUnitOfWork unitOfWork)
        {
            _membroRep = membroRep;
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
            _FornecedorRep = fornecedorRep;
            _templateEmail = templateEmail;
            _emailsNotificaoRep = emailsNotificaoRep;
            _smsRep = smsRep;
            _tipoAvisosRep = tipoAvisosRep;
            _notificacaoRep = notificacaoRep;
            _usuarioNotificacaoRep = usuarioNotificacaoRep;
            _avisosRep = avisosRep;
            _parametroSistemaRep = parametroSistemaRep;
            _execucaoRoboRep = execucaoRoboRep;
            _franquiaRep = franquiaRep;
            _membroCategoriaRep = membroCategoriaRep;
            _emailsRep = emailsRep;
            _unitOfWork = unitOfWork;
        }


        #endregion

        public enum TipoCliente
        {
            Membro = 1,
            Fornecedor = 2
        };

        #region Metodos Genericos

        public TipoAvisos GetTipoAviso(int avisoId)
        {
            var retAviso = _tipoAvisosRep.GetAll().FirstOrDefault(av => av.Id == avisoId);
            return retAviso;
        }

        /// <summary>
        /// Método verificar se pode enviar Email ou SMS.
        /// </summary>
        /// <param name="usuarioId">Passar Id do usuário a qual deseja verificar se quer receber email ou sms.</param>
        /// <param name="tipoAvisosId">Passar o Id do Aviso da tabela TipoAvisos que pode estar no enum TipoAvisos</param>
        /// <param name="tipoAlerta">Passar o Tipo do Alerta que se encontra no Enum TipoAlerta</param>
        /// <returns></returns>
        public bool PodeEnviarNotificacao(int usuarioId, int tipoAvisosId, TipoAlerta tipoAlerta)
        {
            var usuarioNitificacao = this._usuarioNotificacaoRep.All.Any(x => x.Ativo &&
            x.UsuarioId == usuarioId &&
            x.Notificacao.Ativo &&
            x.Notificacao.TipoAvisosId == tipoAvisosId &&
            x.Notificacao.TipoAlerta == tipoAlerta);
            return !usuarioNitificacao;
        }

        /// <summary>
        /// Pega o template que necessitar passando o ID do mesmo.
        /// </summary>
        /// <param name="idEmail">ID do template cadastrado no banco de dados.</param>
        /// <returns></returns>
        public string GetCorpoEmail(int idEmail)
        {
            return _templateEmail.GetSingle(idEmail).Template;
        }

        /// <summary>
        /// Pega os Parâmetros cadastrados no banco de dados passando o ID do mesmo.
        /// </summary>
        /// <param name="codigoParam">Passar o ID cadastrado no Banco de Dados</param>
        /// <returns></returns>
        public string GetParametroSistema(string codigoParam)
        {
            var paramRet = _parametroSistemaRep.FirstOrDefault(w => w.Codigo == codigoParam);
            return paramRet != null ? paramRet.Valor : "";
        }

        /// <summary>
        /// Remove os avisos do Fornecedor referente a Cotação.
        /// </summary>
        /// <param name="pessoaId">Passar o ID da Pessoa</param>
        /// <param name="cotacaoId">Passar o ID da Cotação</param>
        /// <param name="tipoAviso">Passar o ID de tipo do Aviso, pode estar no Enum TipoAviso ou senão terá que acessar o banco para saber o ID necessário</param>
        public void RemoverAvisoUsuarioFornecedorCotacao(int pessoaId, int cotacaoId, int tipoAviso)
        {
            var usuarios = _usuarioRep.FindBy(u => u.PessoaId == pessoaId).ToList();

            foreach (var usuario in usuarios)
            {
                var aviso = this._avisosRep.FirstOrDefault(x => x.UsuarioNotificadoId == usuario.Id && x.IdReferencia == cotacaoId && x.TipoAvisosId == tipoAviso);
                if (aviso == null) continue;

                this._avisosRep.Delete(aviso);
            }

            this._unitOfWork.Commit();
        }

        /// <summary>
        /// Remove os avisos dos Fornecedores referente a Cotação.
        /// </summary>
        /// <param name="cotacaoId">Passar o ID da Cotação</param>
        /// <param name="tipoAviso">Passar o ID de tipo do Aviso, pode estar no Enum TipoAviso ou senão terá que acessar o banco para saber o ID necessário</param>
        public void RemoverAvisosFornecedoresCotacao(int cotacaoId, int tipoAviso)
        {
            var avisos = this._avisosRep.FindBy(x => x.IdReferencia == cotacaoId && x.TipoAvisosId == tipoAviso).ToList();

            this._avisosRep.DeleteAll(avisos);

            this._unitOfWork.Commit();
        }

        /// <summary>
        /// Remove os avisos do Fornecedor referente a Cotação.
        /// </summary>
        /// <param name="pessoaId">Passar o ID da Pessoa</param>
        /// <param name="pedidoId">Passar o ID do Pedido</param>
        /// <param name="tipoAviso">Passar o ID de tipo do Aviso, pode estar no Enum TipoAviso ou senão terá que acessar o banco para saber o ID necessário</param>
        public void RemoverAvisoMembroPedido(int pessoaId, int pedidoId, int tipoAviso)
        {
            var usuarios = _usuarioRep.FindBy(u => u.PessoaId == pessoaId).ToList();

            foreach (var usuario in usuarios)
            {
                var aviso = this._avisosRep.FirstOrDefault(x => x.UsuarioNotificadoId == usuario.Id && x.IdReferencia == pedidoId && x.TipoAvisosId == tipoAviso);
                if (aviso == null) continue;

                this._avisosRep.Delete(aviso);
            }

            this._unitOfWork.Commit();
        }

        public DateTime UltimaExecucaoRotina(int pTipoAviso, string pNomeRotina)
        {
            DateTime retornoData = DateTime.MaxValue;

            var execRobo = _execucaoRoboRep.GetAll().Where(w => w.NomeRotina == pNomeRotina
                                                             && w.TipoAviso == pTipoAviso);

            if (execRobo.Any())
            {
                retornoData = execRobo.Max(m => m.DtCriacao).Date;
            }

            return retornoData;
        }

        public void GravaExecucaoRotina(int pTipoAviso, string pNomeRotina)
        {
            Usuario usuario = _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
            ExecucaoRobo execRobo = new ExecucaoRobo();

            execRobo.Ativo = true;
            execRobo.UsuarioCriacaoId = usuario.Id;
            execRobo.DtCriacao = DateTime.Now;

            execRobo.NomeRotina = pNomeRotina;
            execRobo.TipoAviso = pTipoAviso;

            _execucaoRoboRep.Add(execRobo);
            this._unitOfWork.Commit();
        }

        public List<Usuario> EnviarEmailSmsFornecedorAceitarMembro()
        {
            var usuarios = new List<Usuario>();

            var membrosFornecedores = _membroFornecedorRep.FindBy(x => !x.Ativo).ToList();

            membrosFornecedores.ForEach(x =>
            {
                var nomeMembro = string.Empty;

                if (x.Membro.Pessoa.TipoPessoa == TipoPessoa.PessoaFisica)
                    nomeMembro = x.Membro.Pessoa.PessoaFisica.Nome;
                else
                    nomeMembro = x.Membro.Pessoa.PessoaJuridica.NomeFantasia;

                x.Fornecedor.Pessoa.Usuarios.ToList().ForEach(u =>
                {

                    if (PodeEnviarNotificacao(u.Id, (int)TipoAviso.NovoFornecedorAvisoMembro, TipoAlerta.EMAIL))
                    {

                        Emails email1 = new Emails()
                        {
                            EmailDestinatario = u.UsuarioEmail,

                            CorpoEmail = this.GetCorpoEmail(36)
                            .Replace("#NomeFornecedor#", x.Fornecedor.Pessoa.PessoaJuridica.NomeFantasia)
                            .Replace("#Membro#", nomeMembro),

                            AssuntoEmail = "Solicitação de membro pendente de aceite",
                            Status = Status.NaoEnviado,
                            Origem = Origem.LembreteFornecedorAceiteMembro,
                            DtCriacao = DateTime.Now,
                            UsuarioCriacao = u,
                            Ativo = true
                        };

                        _emailsRep.Add(email1);
                    }

                    if (PodeEnviarNotificacao(u.Id, (int)TipoAviso.NovoFornecedorAvisoMembro, TipoAlerta.SMS))
                    {
                        var sms = new Sms
                        {
                            UsuarioCriacao = u,
                            DtCriacao = DateTime.Now,
                            Numero = u.Telefones.Select(t => t.DddCel).FirstOrDefault() + u.Telefones.Select(t => t.Celular).FirstOrDefault(),
                            Mensagem = $"Economiza Já - Membro {nomeMembro} - Está querendo comprar com você. Corra para aceitar e comece a fazer novos negocios.",
                            Status = StatusSms.NaoEnviado,
                            OrigemSms = TipoOrigemSms.LembreteFornecedorAceiteMembro,
                            Ativo = true
                        };

                        _smsRep.Add(sms);
                    }

                    usuarios.Add(u);
                    _unitOfWork.Commit();

                });

            });

            return usuarios;
        }


        #endregion

        #region Metodos Pedidos Pendentes de Aceito Membro/Fornecedor

        public List<RetornoAvisos> TrataPedidoMembro(Pedido Pedido, int TipoAvisosId)
        {

            List<RetornoAvisos> listaRetorno = new List<RetornoAvisos>();
            Membro MembroPedido = Pedido.Membro;
            List<Usuario> lUsu = _usuarioRep.FindBy(u => u.PessoaId == MembroPedido.PessoaId).ToList();

            foreach (Usuario usu in lUsu)
            {
                bool retEnviarEmail = false;
                bool retEnviarSMS = false;
                bool retNovoAviso = false;
                //Pega o Registro do Aviso para validar se necessário novo envio de email, caso não tiver deve ser criado um registro
                var NovoAviso = _avisosRep.GetAll().FirstOrDefault(av => av.TipoAvisosId == TipoAvisosId
                                                                && av.UsuarioNotificadoId == usu.Id
                                                                && av.IdReferencia == Pedido.Id);
                if (NovoAviso == null)
                    retNovoAviso = true;

                //em minutos, tempo para reenviar email e SMS
                int paramTempoValidacao = 30; //default
                string strParamTempoValidacao = GetParametroSistema("REENVIO_ACEITEPEDIDO");
                paramTempoValidacao = strParamTempoValidacao == "" ? paramTempoValidacao : int.Parse(strParamTempoValidacao);

                //Valida Se Necessário enviar email
                if (PodeEnviarNotificacao(usu.Id, TipoAvisosId, TipoAlerta.EMAIL) && (!retNovoAviso && NovoAviso.DataUltimoAviso.AddMinutes(paramTempoValidacao) < DateTime.Now))
                    retEnviarEmail = true;

                //Valida se necessário enviar SMS
                if (PodeEnviarNotificacao(usu.Id, TipoAvisosId, TipoAlerta.SMS) && (!retNovoAviso && NovoAviso.DataUltimoAviso.AddMinutes(paramTempoValidacao) < DateTime.Now))
                    retEnviarSMS = true;

                //Cria lista com dados para envio.
                listaRetorno.Add(new RetornoAvisos()
                {
                    EnviarEmail = retEnviarEmail,
                    EnviarSMS = retEnviarSMS,
                    NovoAviso = retNovoAviso,
                    Usuario = usu,
                    Aviso = NovoAviso
                });
            }


            return listaRetorno;
        }

        public void LimparTabelAvisosPorMembro(List<Pedido> ListaPedidosPendentes, int pTipoAviso)
        {
            List<int> vAvisos = new List<int>();
            // Identifica Pedidos que estão na tabela de Avisos, porém já foram respondidos
            vAvisos = _avisosRep.GetAll().Where(w => w.TipoAvisosId == pTipoAviso && w.ModuloId == 3).Select(s => s.IdReferencia).ToList();
            if (vAvisos != null && vAvisos.Any())
            {
                List<int> PedidosDeletar = vAvisos.Except(ListaPedidosPendentes.Select(s => s.Id)).ToList();
                // Consulta Avisos por Pedidos
                var AvisosDeletar = _avisosRep.GetAll().Where(w => w.TipoAvisosId == pTipoAviso && w.ModuloId == 3 && PedidosDeletar.Contains(w.IdReferencia));

                if (AvisosDeletar != null && AvisosDeletar.Any())
                {
                    _avisosRep.DeleteAll(AvisosDeletar);
                    _unitOfWork.Commit();
                }
            }
        }

        public List<Pedido> ListarPedidosPendentes(Enum pCliente)
        {
            bool CotacapoAceiteMembro = true;

            // Status pedido em cotacao
            int statusIdPend = 0;
            int statusIdPendHist = 0;
            if ((TipoCliente)pCliente == TipoCliente.Membro)
            {
                //Status pedido Aguardando Membro
                statusIdPend = _statusSistemaRep.GetAll().FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 3).Id;
                statusIdPendHist = statusIdPend;
            }
            else
            {
                //Status pedido Aguardando Fornecedor
                statusIdPend = _statusSistemaRep.GetAll().FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 4).Id;
                statusIdPendHist = _statusSistemaRep.GetAll().FirstOrDefault(x => x.WorkflowStatusId == 12 && x.Ordem == 3).Id;
            }

            List<Pedido> lPedidoPendente = _pedidoRep.FindBy(p => p.StatusSistemaId == statusIdPend).ToList();
            List<Pedido> lPedidoIdPendenteAtrasado = new List<Pedido>();
            Pedido pedidoAtrasado = null;

            foreach (Pedido item in lPedidoPendente)
            {
                var objRet = _histStatusPedidoRep.GetAll().FirstOrDefault(hp => hp.PedidoId == item.Id
                                   && hp.StatusSistemaId == statusIdPendHist
                                   );

                if (objRet != null)
                {
                    pedidoAtrasado = objRet.Pedido;
                }
                if (pedidoAtrasado != null)
                {
                    CotacapoAceiteMembro = true;

                    if ((TipoCliente)pCliente == TipoCliente.Fornecedor)
                    {
                        //Verifica se Todos os pedidos da cotação foram aceitos pelo membro, para ai sim avisar o fornecedor.
                        CotacaoPedidos lcotPed = _cotacaoPedidoRep.GetAll().FirstOrDefault(lp => lp.PedidoId == item.Id);
                        List<CotacaoPedidos> lcot = _cotacaoPedidoRep.GetAll().Where(cpr => cpr.CotacaoId == lcotPed.CotacaoId).ToList();
                        foreach (CotacaoPedidos itemCot in lcot)
                        {
                            if (itemCot.Pedido.StatusSistemaId != statusIdPend)
                            {
                                CotacapoAceiteMembro = false;
                            }
                        }
                    }

                    if (CotacapoAceiteMembro)
                        lPedidoIdPendenteAtrasado.Add(pedidoAtrasado);
                }
            }

            return lPedidoIdPendenteAtrasado;
        }

        public void LimparTabelAvisosPorFornecedor(List<Pedido> ListaPedidosPendentes, int pTipoAviso)
        {
            List<int> vAvisos = new List<int>();
            // Identifica Pedidos que estão na tabela de Avisos, porém já foram respondidos
            vAvisos = _avisosRep.GetAll().Where(w => w.TipoAvisosId == pTipoAviso && w.ModuloId == 4).Select(s => s.IdReferencia).ToList();
            if (vAvisos != null && vAvisos.Any())
            {
                var PedidosDeletar = vAvisos.Except(ListaPedidosPendentes.Select(s => s.Id)).ToList();
                // Consulta Avisos por Pedidos
                var AvisosDeletar = _avisosRep.GetAll().Where(w => w.TipoAvisosId == pTipoAviso && w.ModuloId == 4 && PedidosDeletar.Contains(w.IdReferencia));
                if (AvisosDeletar != null && AvisosDeletar.Any())
                {
                    _avisosRep.DeleteAll(AvisosDeletar);
                    _unitOfWork.Commit();
                }
            }
        }

        public void LimparAvisoPedidoFornecedor(Pedido pedido, int pTipoAviso, int usuarioId)
        {

            // Identifica Pedido que está na tabela de Avisos
            var vAviso = _avisosRep
                .FirstOrDefault(w => w.IdReferencia == pedido.Id &&
                w.UsuarioNotificadoId == usuarioId &&
                w.TipoAvisosId == pTipoAviso &&
                w.ModuloId == 4);

            if (vAviso != null)
            {
                _avisosRep.Delete(vAviso);
                _unitOfWork.Commit();
            }
        }

        public Usuario UsuarioRobo()
        {
            return _usuarioRep.GetSingle(SessaoEconomiza.UsuarioId);
        }

        public void AddAvisos(Avisos pAviso)
        {
            pAviso.UsuarioCriacao = UsuarioRobo();
            pAviso.UsuarioCriacaoId = pAviso.UsuarioCriacao.Id;
            pAviso.DtCriacao = DateTime.Now;

            _avisosRep.Add(pAviso);
            _unitOfWork.Commit();
        }


        public void AtualizaDataAviso(Avisos avisos, int usuarioId)
        {
            Avisos AtuAviso = _avisosRep.GetSingle(avisos.Id);
            AtuAviso.DataUltimoAviso = DateTime.Now;
            AtuAviso.UsuarioAlteracaoId = usuarioId;
            AtuAviso.DtAlteracao = DateTime.Now;
            _unitOfWork.Commit();

        }
        #endregion Metodos

        #region Metodos Aceite Solicitacao de Trabalho Fornecedor/Membro

        public List<Fornecedor> ListaFornecedorPedido(Pedido Pedido)
        {
            List<Fornecedor> ret = new List<Fornecedor>();

            foreach (var itemPedi in Pedido.ItemPedidos)
            {

                if (null != itemPedi.Fornecedor)
                    ret.Add(itemPedi.Fornecedor);
            }
            return ret;
        }

        public List<RetornoAvisos> TrataPedidoFornecedor(Pedido pPedido, Fornecedor pFornecedor, int TipoAvisosId)
        {
            List<RetornoAvisos> listaRetorno = new List<RetornoAvisos>();

            List<Usuario> lUsu = _usuarioRep.FindBy(u => u.PessoaId == pFornecedor.Pessoa.Id).ToList();

            foreach (Usuario usu in lUsu)
            {
                bool retEnviarEmail = false;
                bool retEnviarSMS = false;
                bool retNovoAviso = false;
                //Pega o Registro do Aviso para validar se necessário novo envio de email, caso não tiver deve ser criado um registro
                var NovoAviso = _avisosRep.GetAll().FirstOrDefault(av => av.TipoAvisosId == TipoAvisosId
                                                                && av.UsuarioNotificadoId == usu.Id
                                                                && av.IdReferencia == pPedido.Id);

                if (NovoAviso == null)
                    retNovoAviso = true;

                //em minutos, tempo para reenviar email e SMS
                int paramTempoValidacao = 30; //default
                string strParamTempoValidacao = GetParametroSistema("REENVIO_ACEITEPEDIDO");
                paramTempoValidacao = strParamTempoValidacao == "" ? paramTempoValidacao : int.Parse(strParamTempoValidacao);

                // valida se necessário enviar email
                if (PodeEnviarNotificacao(usu.Id, TipoAvisosId, TipoAlerta.EMAIL) && (!retNovoAviso && NovoAviso.DataUltimoAviso.AddMinutes(paramTempoValidacao) < DateTime.Now))
                    retEnviarEmail = true;

                //Valida se necessário enviar SMS
                if (PodeEnviarNotificacao(usu.Id, TipoAvisosId, TipoAlerta.SMS) && (!retNovoAviso && NovoAviso.DataUltimoAviso.AddMinutes(paramTempoValidacao) < DateTime.Now))
                    retEnviarSMS = true;

                //Cria lista com dados para envio.
                listaRetorno.Add(new RetornoAvisos()
                {
                    EnviarEmail = retEnviarEmail,
                    EnviarSMS = retEnviarSMS,
                    NovoAviso = retNovoAviso,
                    Usuario = usu,
                    Aviso = NovoAviso
                });
            }


            return listaRetorno;
        }

        public List<RetornoAvisos> VerificaNovoFornecedorCadastrado(int tipoAvisosId, DateTime pDataPesquisa)
        {
            List<RetornoAvisos> listaRetorno = new List<RetornoAvisos>();

            List<int> lCat = new List<int>();
            List<int> lFCid = new List<int>();


            var lFornCat = _FornecedorRep.GetAll().Where(w => w.DtCriacao >= pDataPesquisa).Select(s => s.FornecedorCategorias).ToList();
            var lFornReg = _FornecedorRep.GetAll().Where(w => w.DtCriacao >= pDataPesquisa).Select(s => s.FornecedorRegiao).ToList();

            foreach (var itemFornCat in lFornCat)
            {
                foreach (var itemCat in itemFornCat)
                {
                    lCat.Add(itemCat.Categoria.Id);
                }
            }

            foreach (var itemFornReg in lFornReg)
            {
                foreach (var itemReg in itemFornReg)
                {
                    lFCid.Add(itemReg.Cidade.Id);
                }
            }

            var lmembro = _membroRep.GetAll().Where(w => w.MembroCategorias.Where(ww => lCat.Contains(ww.CategoriaId)).Any()
                                                   && w.Pessoa.Enderecos.Where(we => lFCid.Contains(we.CidadeId)).Any()).ToList();

            List<int> lPessoaMembroForaFranquia = lmembro.Where(w => w.FranquiaId == null || w.FranquiaId == 0).Select(s => s.PessoaId).ToList();
            //Membro de Franquias
            var lFranquia = lmembro.Where(w => w.FranquiaId != null && w.FranquiaId > 0).Select(s => s.FranquiaId).ToList();
            List<int> lPessoaFranquia = new List<int>();
            if (lFranquia.Any())
            {
                lPessoaFranquia = _franquiaRep.GetAll().Where(w => lFranquia.Contains(w.Id)).Select(s => s.PessoaId).ToList();
            }


            var lUsuForaFranquia = _usuarioRep.GetAll().Where(u => lPessoaMembroForaFranquia.Contains(u.PessoaId)).ToList();

            foreach (var usu in lUsuForaFranquia)
            {
                var retEnviarEmail = false;
                var retEnviarSMS = false;
                var retNovoAviso = false;

                //Pega o Registro do Aviso para validar se necessário novo envio de email, caso não tiver deve ser criado um registro
                var novoAviso = _avisosRep.GetAll().FirstOrDefault(av => av.TipoAvisosId == tipoAvisosId
                                                                && av.UsuarioNotificadoId == usu.Id
                                                                && av.IdReferencia == usu.PessoaId);
                if (novoAviso == null)
                    retNovoAviso = true;

                //Valida Se Necessário enviar email
                if (PodeEnviarNotificacao(usu.Id, tipoAvisosId, TipoAlerta.EMAIL) && !retNovoAviso && novoAviso.DataUltimoAviso.AddDays(1) < DateTime.Now)
                    retEnviarEmail = true;

                //Valida se necessário enviar SMS
                if (PodeEnviarNotificacao(usu.Id, tipoAvisosId, TipoAlerta.SMS) && !retNovoAviso && novoAviso.DataUltimoAviso.AddDays(1) < DateTime.Now)
                    retEnviarSMS = true;

                //Cria lista com dados para envio.
                listaRetorno.Add(new RetornoAvisos
                {
                    EnviarEmail = retEnviarEmail,
                    EnviarSMS = retEnviarSMS,
                    NovoAviso = retNovoAviso,
                    Usuario = usu,
                    Aviso = novoAviso
                });
            }

            // Verifica se tem novos para Franquia
            if (lPessoaFranquia.Any())
            {
                var lUsuFranquia = _usuarioRep.GetAll().Where(u => lPessoaFranquia.Contains(u.PessoaId)).ToList();
                foreach (var usu in lUsuFranquia)
                {
                    var retEnviarEmail = false;
                    var retEnviarSMS = false;
                    var retNovoAviso = false;

                    //Pega o Registro do Aviso para validar se necessário novo envio de email, caso não tiver deve ser criado um registro
                    var novoAviso = _avisosRep.GetAll().FirstOrDefault(av => av.TipoAvisosId == tipoAvisosId
                                                                    && av.UsuarioNotificadoId == usu.Id
                                                                    && av.IdReferencia == usu.PessoaId);
                    if (novoAviso == null)
                        retNovoAviso = true;

                    //Valida Se Necessário enviar email
                    if (PodeEnviarNotificacao(usu.Id, tipoAvisosId, TipoAlerta.EMAIL) && !retNovoAviso && novoAviso.DataUltimoAviso.AddDays(1) < DateTime.Now)
                        retEnviarEmail = true;

                    //Valida se necessário enviar SMS
                    if (PodeEnviarNotificacao(usu.Id, tipoAvisosId, TipoAlerta.SMS) && !retNovoAviso && novoAviso.DataUltimoAviso.AddDays(1) < DateTime.Now)
                        retEnviarSMS = true;

                    //Cria lista com dados para envio.
                    listaRetorno.Add(new RetornoAvisos
                    {
                        EnviarEmail = retEnviarEmail,
                        EnviarSMS = retEnviarSMS,
                        NovoAviso = retNovoAviso,
                        Usuario = usu,
                        Aviso = novoAviso
                    });
                }
            }

            //retorna lista
            return listaRetorno;
        }

        public List<MembroFornecedor> ListaPendenciasAceiteFornecedorParaMembro()
        {
            List<MembroFornecedor> ret = new List<MembroFornecedor>();
            ret = _membroFornecedorRep.GetAll().Where(fo => fo.Ativo == false).ToList();

            return ret;
        }

        public List<RetornoAvisos> TrataPendenciasAceiteFornecedorParaMembro(MembroFornecedor itemFornecedor, int tipoAvisosId)
        {
            var listaRetorno = new List<RetornoAvisos>();
            var lUsu = _usuarioRep.FindBy(u => u.PessoaId == itemFornecedor.Fornecedor.PessoaId).ToList();

            foreach (var usu in lUsu)
            {
                var retEnviarEmail = false;
                var retEnviarSMS = false;
                var retNovoAviso = false;

                //Pega o Registro do Aviso para validar se necessário novo envio de email, caso não tiver deve ser criado um registro
                var novoAviso = _avisosRep.GetAll().FirstOrDefault(av => av.TipoAvisosId == tipoAvisosId
                                                                && av.UsuarioNotificadoId == usu.Id
                                                                && av.IdReferencia == itemFornecedor.MembroId);
                if (novoAviso == null)
                    retNovoAviso = true;

                //Valida Se Necessário enviar email
                if (PodeEnviarNotificacao(usu.Id, tipoAvisosId, TipoAlerta.EMAIL) && !retNovoAviso && novoAviso.DataUltimoAviso.AddDays(1) < DateTime.Now)
                    retEnviarEmail = true;

                //Valida se necessário enviar SMS
                if (PodeEnviarNotificacao(usu.Id, tipoAvisosId, TipoAlerta.SMS) && !retNovoAviso && novoAviso.DataUltimoAviso.AddDays(1) < DateTime.Now)
                    retEnviarSMS = true;

                //Cria lista com dados para envio.
                listaRetorno.Add(new RetornoAvisos
                {
                    EnviarEmail = retEnviarEmail,
                    EnviarSMS = retEnviarSMS,
                    NovoAviso = retNovoAviso,
                    Usuario = usu,
                    Aviso = novoAviso
                });
            }
            return listaRetorno;
        }


        public void LimparTabelAvisosPorSolicitacaoMembroFornecedor(List<MembroFornecedor> ListaPedidosPendentes, int pTipoAviso)
        {
            List<int> vAvisos = new List<int>();
            // Identifica Pedidos que estão na tabela de Avisos, porém já foram respondidos
            vAvisos = _avisosRep.GetAll().Where(w => w.TipoAvisosId == pTipoAviso && w.ModuloId == 4).Select(s => s.IdReferencia).ToList();
            if (vAvisos != null && vAvisos.Any())
            {
                List<int> PedidosDeletar = vAvisos.Except(ListaPedidosPendentes.Select(s => s.MembroId)).ToList();
                // Consulta Avisos por Pedidos
                var AvisosDeletar = _avisosRep.GetAll().Where(w => w.TipoAvisosId == pTipoAviso && w.ModuloId == 4 && PedidosDeletar.Contains(w.IdReferencia));

                if (AvisosDeletar != null && AvisosDeletar.Any())
                {
                    _avisosRep.DeleteAll(AvisosDeletar);
                    _unitOfWork.Commit();
                }
            }
        }


        #endregion

        #region Metodos Para Identificar novas cotações ou cotações cujo Fornecedor não tenha dado lance

        public List<Cotacao> ListarCotacoesPendentes()
        {
            var retorno = _cotacaoRep.GetAll().Where(w => w.StatusSistemaId == 25 || w.StatusSistemaId == 26).ToList();

            return retorno;
        }


        public List<RetornoAvisos> TrataNovaCotacaoFornecedor(Cotacao pCotacao, int TipoAvisosId)
        {
            var listaRetorno = new List<RetornoAvisos>();
            var cotacaoesPedidos = _cotacaoPedidoRep.GetAll().Where(w => w.CotacaoId == pCotacao.Id).ToList();


            //resgatar todos fornecedores que deram lance na cotação
            var fornecedoresDeramLance = _resultadoCotacaoRep.FindBy(rc => rc.CotacaoId == pCotacao.Id).Select(s => s.Fornecedor).ToList();

            //pegar todos fornecedores que podem dar o lance na cotação
            var todosMembrosCotacao = cotacaoesPedidos.Select(s => s.Pedido.MembroId).ToList();
            var todosFornecedores = _membroFornecedorRep.GetAll().Where(w => todosMembrosCotacao.Contains(w.MembroId)).Select(s => s.Fornecedor).ToList();

            //fazer distinção dos fornecedores que deram o lance e que podiam dar o lance, para separar os que ainda não deram
            var fornecedoresNaoDeramLance = todosFornecedores.Except(fornecedoresDeramLance).ToList();


            //Pegar todos Usuarios dos fornecedores que não deram lance
            var pFornecedores = fornecedoresNaoDeramLance.Select(s => s.Pessoa.Id).ToList();
            var lUsu = _usuarioRep.GetAll().Where(u => pFornecedores.Contains(u.PessoaId)).ToList();


            foreach (var usu in lUsu)
            {
                var retEnviarEmail = false;
                var retEnviarSMS = false;
                var retNovoAviso = false;
                //Pega o Registro do Aviso para validar se necessário novo envio de email, caso não tiver deve ser criado um registro
                var novoAviso = _avisosRep.GetAll().FirstOrDefault(av => av.TipoAvisosId == TipoAvisosId
                                                                && av.UsuarioNotificadoId == usu.Id
                                                                && av.IdReferencia == pCotacao.Id);
                if (novoAviso == null)
                    retNovoAviso = true;

                //Valida Se Necessário enviar email
                if (PodeEnviarNotificacao(usu.Id, TipoAvisosId, TipoAlerta.EMAIL) && (!retNovoAviso && novoAviso.DataUltimoAviso.AddMinutes(30) < DateTime.Now))
                    retEnviarEmail = true;

                //Valida se necessário enviar SMS
                if (PodeEnviarNotificacao(usu.Id, TipoAvisosId, TipoAlerta.SMS) && (!retNovoAviso && novoAviso.DataUltimoAviso.AddMinutes(30) < DateTime.Now))
                    retEnviarSMS = true;

                //Cria lista com dados para envio.
                listaRetorno.Add(new RetornoAvisos
                {
                    EnviarEmail = retEnviarEmail,
                    EnviarSMS = retEnviarSMS,
                    NovoAviso = retNovoAviso,
                    Usuario = usu,
                    Aviso = novoAviso
                });
            }


            return listaRetorno;
        }


        public List<Usuario> UsuariosFornecedoresResponderCotacao(Cotacao pCotacao, int qtdHoraEnvioNotificacao)
        {

            var listaRetorno = new List<Usuario>();
            var cotacaoesPedidos = _cotacaoPedidoRep.GetAll().Where(w => w.CotacaoId == pCotacao.Id).ToList();


            //resgatar todos fornecedores que deram lance na cotação
            var fornecedoresDeramLance = _resultadoCotacaoRep.FindBy(rc => rc.CotacaoId == pCotacao.Id).Select(s => s.Fornecedor).ToList();

            //pegar todos fornecedores que podem dar o lance na cotação
            var todosMembrosCotacao = cotacaoesPedidos.Select(s => s.Pedido.MembroId).ToList();
            var todosFornecedores = _membroFornecedorRep.GetAll().Where(w => todosMembrosCotacao.Contains(w.MembroId)).Select(s => s.Fornecedor).ToList();

            //fazer distinção dos fornecedores que deram o lance e que podiam dar o lance, para separar os que ainda não deram
            var fornecedoresNaoDeramLance = todosFornecedores.Except(fornecedoresDeramLance).ToList();

            //Pegar todos Usuarios dos fornecedores que não deram lance
            var pFornecedores = fornecedoresNaoDeramLance.Select(s => s.Pessoa.Id).ToList();
            var listaUsuariosFornecedoresRespCotacao = _usuarioRep.GetAll().Where(u => pFornecedores.Contains(u.PessoaId)).ToList();

            var horaFechamentoCotcao = _cotacaoRep.FirstOrDefault(x => x.Id == pCotacao.Id).DtFechamento;
            var dataHoraParametro = new TimeSpan(qtdHoraEnvioNotificacao, 0, 0).Subtract(new TimeSpan(0, 30, 0));

            listaUsuariosFornecedoresRespCotacao.ForEach(x =>
            {
                var enviaNotificacao = false;
                var aviso = _avisosRep.FirstOrDefault(a => a.UsuarioNotificado.Id == x.Id && a.TipoAviso.Id == (int)TipoAviso.NovaCotacao);

                if (aviso == null)
                {
                    enviaNotificacao = (horaFechamentoCotcao - DateTime.Now) <= dataHoraParametro ? true : false;

                    if (enviaNotificacao)
                    {
                        var _aviso = new Avisos
                        {
                            UsuarioCriacaoId = 1,
                            DtCriacao = DateTime.Now,
                            Ativo = true,
                            IdReferencia = pCotacao.Id,
                            DataUltimoAviso = DateTime.Now,
                            ExibeNaTelaAvisos = true,
                            TipoAvisosId = (int)TipoAviso.NovaCotacao, //Id TipoAviso para esse aviso
                            URLPaginaDestino = "/#/cotacoes",
                            TituloAviso = "Cotação Pendente de Resposta",
                            ToolTip = "Cotação Pendente de Resposta",
                            DescricaoAviso = "Cotação " + pCotacao.Id + " Pendente de Resposta",
                            ModuloId = 4, //Modulo Fornecedor
                            UsuarioNotificadoId = x.Id
                        };

                        _avisosRep.Add(_aviso);
                        _unitOfWork.Commit();
                    }

                }
                else
                {
                    enviaNotificacao = (horaFechamentoCotcao - DateTime.Now) <= dataHoraParametro &&
                       (DateTime.Now - aviso.DataUltimoAviso) >= dataHoraParametro ? true : false;
                }

                if (enviaNotificacao)
                {
                    listaRetorno.Add(x);
                    if (aviso != null)
                        AtualizaDataAviso(aviso, x.Id);
                }

            });

            return listaRetorno;
        }


        public void LimparTabelAvisosPorNovasCotacoesFornecedor(List<Cotacao> listaPedidoMembro, int pTipoAviso)
        {
            //TODO cotacao
            List<int> vAvisos = new List<int>();
            // Identifica Pedidos que estão na tabela de Avisos, porém já foram respondidos
            vAvisos = _avisosRep.GetAll().Where(w => w.TipoAvisosId == pTipoAviso && w.ModuloId == 4).Select(s => s.IdReferencia).ToList();
            if (vAvisos != null && vAvisos.Any())
            {
                List<int> PedidosDeletar = vAvisos.Except(listaPedidoMembro.Select(s => s.Id)).ToList();
                // Consulta Avisos por Pedidos
                var AvisosDeletar = _avisosRep.GetAll().Where(w => w.TipoAvisosId == pTipoAviso && w.ModuloId == 4 && PedidosDeletar.Contains(w.IdReferencia));

                if (AvisosDeletar != null && AvisosDeletar.Any())
                {
                    _avisosRep.DeleteAll(AvisosDeletar);
                    _unitOfWork.Commit();
                }
            }
        }

        #endregion

        #region Métodos para Noticação de Antecipação do Pedido do Membro

        public List<KeyValuePair<int, Usuario>> AntecipacaoPedidoMembro()
        {

            var listPedUsuario = new List<KeyValuePair<int, Usuario>>();
            var pedidos = _pedidoRep.FindBy(x => x.StatusSistemaId == 21 && (x.DtCotacao.Date - DateTime.Now.Date).Days == 2).ToList();

            if (pedidos.Count > 0)
            {
                pedidos.ForEach(x =>
                {
                    x.DtCotacao = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, x.DtCotacao.Hour, x.DtCotacao.Minute, DateTime.Now.Second);
                    _pedidoRep.Edit(x);
                    _unitOfWork.Commit();

                    var pedUsuario = new KeyValuePair<int, Usuario>(x.Id, x.UsuarioCriacao);
                    listPedUsuario.Add(pedUsuario);

                });
            }

            return listPedUsuario;
        }


        #endregion
    }
}
