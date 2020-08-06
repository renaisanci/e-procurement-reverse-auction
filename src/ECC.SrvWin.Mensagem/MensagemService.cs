using ECC.Servicos;
using ECC.Servicos.Util;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Timers;
using ECC.EntidadeSms;

namespace ECC.SrvWin.Mensagem
{
    public partial class MensagemService : ServiceBase
    {
        #region Private Variables

        private Timer _timerSms;
        private List<Sms> _listaSmsNaoEnviado;
        private List<Sms> _listaSmsPendente;

        #endregion Private Variables

        #region Service Operations

        public MensagemService()
        {
            InitializeComponent();
            OnStart(null);
        }

        protected override void OnStop()
        {
            if (_timerSms != null)
            {
                _timerSms.Stop();
                _timerSms.Dispose();
            }
        }

        protected override void OnStart(string[] args)
        {
            _timerSms = new Timer(TimeSpan.FromSeconds(Convert.ToInt32(ParametrosAppConfig.TimerSms)).TotalMilliseconds);
            _timerSms.Enabled = true;

#if (!DEBUG)
            _timerSms.Elapsed += _timerSms_Elapsed;
            _timerSms.Start();
#else
            this.Sms();
#endif
        }

        #endregion Service Operations

        #region TimerSmsElapsed

        private void _timerSms_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                this.Sms();
            }
            catch (Exception ex)
            {
                //Logar Erro
            }
        }

        private void Sms()
        {
            if (_timerSms.Enabled == false)
                return;

            try
            {
                _timerSms.Enabled = false;

                SessaoEconomiza.UsuarioId = 1;
                this.ProcessarSmsNaoEnviado();
                this.ProcessarSmsPendente();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _timerSms.Enabled = true;
            }
        }

        private SmsGateway CreateSmsGateway()
        {
            string smsUser = ParametrosAppConfig.SmsGatewayUser;
            string smsPassword = ParametrosAppConfig.SmsGatewayPassword;

            return new SmsGateway(smsUser, smsPassword);
        }

        private void ProcessarSmsNaoEnviado()
        {            
            SmsService smsService = new SmsService();

            this._listaSmsNaoEnviado = smsService.BuscarNaoEnviados(ParametrosAppConfig.QtdeSmsEnviadosPorVez);

            if (_listaSmsNaoEnviado.Count == 0)
                return;

            foreach (var sms in _listaSmsNaoEnviado)
            {
                sms.DataInicioProcesso = DateTime.Now;
                sms.Status = StatusSms.Processando;
                smsService.Salvar(sms);
            }

            SmsGateway smsGateway = CreateSmsGateway();

            smsGateway.GetDevices(
            success: (devices) =>
            {
                foreach (var sms in _listaSmsNaoEnviado)
                {
                    try
                    {
                        string device = devices[0];

                        smsGateway.SendMessage(device, sms.Numero, sms.Mensagem,
                        success: (result) =>
                        {
                            var smsServiceThreadSafe = new SmsService();
                            var smsAtualizavel = smsServiceThreadSafe.BuscarPor(sms.Id);

                            smsAtualizavel.DataEnvio = DateTime.Now;
                            smsAtualizavel.Erro = null;

                            if (result[0]["status"].ToString() == "pending")
                                smsAtualizavel.Status = StatusSms.Pendente;

                            smsAtualizavel.IdMessageSmsGateway = result[0]["id"].ToString();

                            smsServiceThreadSafe.Salvar(smsAtualizavel);
                        },
                        error: (erro) =>
                        {
                            LogarErroSms(sms.Id, StatusSms.TentarNovamente, erro);
                        });

                        var newIndex = devices.IndexOf(device) + 1;

                        if (newIndex == devices.Count)
                            device = devices[0];
                        else
                            device = devices[newIndex];
                    }
                    catch (Exception ex)
                    {
                        LogarErroSms(sms.Id, StatusSms.TentarNovamente, ex.Message);
                    }
                }
            },
            error: (erro) =>
            {
                foreach (var sms in _listaSmsNaoEnviado)
                {
                    LogarErroSms(sms.Id, StatusSms.TentarNovamente, erro);
                }
            });
        }       

        private void ProcessarSmsPendente()
        {
            SmsService smsService = new SmsService();

            this._listaSmsPendente = smsService.BuscarPendentes(ParametrosAppConfig.QtdeSmsEnviadosPorVez);

            if (_listaSmsPendente.Count == 0)
                return;

            SmsGateway smsGateway = CreateSmsGateway();

            foreach (var sms in _listaSmsPendente)
            {
                if (!string.IsNullOrWhiteSpace(sms.IdMessageSmsGateway))
                {
                    smsGateway.GetMessage(
                        sms.IdMessageSmsGateway,
                        success: (result) =>
                        {
                            var status = result["status"].ToString();

                            if (status == "sent" || status == "manual send")
                            {
                                var smsServiceThreadSafe = new SmsService();
                                var smsAtualizavel = smsServiceThreadSafe.BuscarPor(sms.Id);

                                smsAtualizavel.Status = StatusSms.Enviado;
                                smsAtualizavel.DataConfirmacaoEnvio = DateTime.Now;

                                smsServiceThreadSafe.Salvar(smsAtualizavel);
                            }
                            else if (status == "pending" && sms.DataEnvio < DateTime.Now.AddMinutes(-5))
                            {                                
                                var smsServiceThreadSafe = new SmsService();
                                var smsAtualizavel = smsServiceThreadSafe.BuscarPor(sms.Id);

                                smsAtualizavel.Status = StatusSms.NaoEnviado;
                                smsAtualizavel.DataEnvio = null;
                                smsAtualizavel.DataInicioProcesso = null;

                                smsServiceThreadSafe.Salvar(smsAtualizavel);
                            }
                            else if (status == "failed")
                            {
                                LogarErroSms(sms.Id, StatusSms.Erro, (result["error"] ?? string.Empty).ToString());
                            }
                        },
                        error: (erro) =>
                        {
                            LogarErroSms(sms.Id, StatusSms.TentarNovamente, erro);
                        });
                }
            }
        }

        private void LogarErroSms(int idSms, StatusSms status, string error = null)
        {
            SmsService SmsService = new SmsService();
            var smsAtualizavel = SmsService.BuscarPor(idSms);

            smsAtualizavel.Status = status;
            smsAtualizavel.Erro = error;

            SmsService.Salvar(smsAtualizavel);
        }

        #endregion TimerSmsElapsed
    }
}
