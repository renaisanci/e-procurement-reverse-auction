﻿using System;
using System.Configuration;
using System.Diagnostics;
using ECC.EntidadeEmail;
using ECC.EntidadeSms;
using ECC.Servicos;
using ECC.SMS;
using Quartz;

namespace ECC.ClientJob.JobEmail
{
    public class EnvioEmailSms : BaseJob
    {
        
        #region Propriedades

        private EventLog eventLog1 = new EventLog();

        #endregion


        #region Execute

        public override void Execute(IJobExecutionContext context)
        {

            if (!EventLog.SourceExists("ECJ_WS05"))
            {
                EventLog.CreateEventSource(
                    "ECJ_WS05", "ECJ_WS05Log");
            }

            eventLog1.Source = "ECJ_WS05";
            eventLog1.Log = "ECJ_WS05Log";


            try
            {
                eventLog1.WriteEntry("ECJ_WS05 - Chamando classe Envia Email SMS ", EventLogEntryType.Information, 1);

                this.ProcessarSmsNaoEnviado();
                this.ProcessarSmsPendente();
                this.ProcessarEmailNaoEnviado();
                this.ProcessarEmailPendente();

            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("ECJ_WS05 - Erro Chamando classe Envia Email SMS " + ex.Message, EventLogEntryType.Error, 156);
            }
        }


        #endregion
        

        #region Métodos


        private void ProcessarSmsNaoEnviado()
        {
            eventLog1.WriteEntry("ECJ_WS05 - Processa sms nao enviado", EventLogEntryType.Information, 2);

            SmsService smsService = new SmsService();

            smsService.LimpaSmsEnviado();
            eventLog1.WriteEntry("ECJ_WS05 - Deleta sms enviado", EventLogEntryType.Information, 200);

            var tww = new TWW();

            var listaSmsNaoEnviado = smsService.BuscarNaoEnviados();

            if (listaSmsNaoEnviado.Count == 0)
                return;

            foreach (var sms in listaSmsNaoEnviado)
            {
                try
                {
                    sms.DataInicioProcesso = DateTime.Now;
                    sms.Status = StatusSms.Processando;
                    smsService.Salvar(sms);

                    var response = tww.Send(sms.Id, sms.Numero, sms.Mensagem);
                    //emailService.EnviaEmail(ParametrosAppConfig.EmailSms, sms.Mensagem + "@" + idMsg, sms.Numero);
                    if (response != "OK")
                    {
                        LogarErroSms(sms.Id, StatusSms.TentarNovamente, $"créditos: {tww.Creditos()}, validade: {tww.ValidadeCreditos()}");
                        continue;
                    }

                    sms.Status = StatusSms.Enviado;
                    smsService.Salvar(sms);
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry("ECJ_WS05 - Processa sms nao enviado" + ex.Message, EventLogEntryType.Error, 1);
                    LogarErroSms(sms.Id, StatusSms.Erro, ex.Message);
                }
            }

            eventLog1.WriteEntry("ECJ_WS05 - Fim Processa sms nao enviado", EventLogEntryType.Information, 3);
        }

        private void ProcessarSmsPendente()
        {
            eventLog1.WriteEntry("ECJ_WS05 - Processar Sms Pendente", EventLogEntryType.Information, 4);
            var smsService = new SmsService();
            //var emailService = new EmailService();
            var tww = new TWW();

            var listaSmsPendente = smsService.BuscarPendentes();

            if (listaSmsPendente.Count == 0)
                return;

            foreach (var sms in listaSmsPendente)
            {
                try
                {
                    sms.DataInicioProcesso = DateTime.Now;
                    sms.Status = StatusSms.Processando;
                    smsService.Salvar(sms);

                    var response = tww.Send(sms.Id, sms.Numero, sms.Mensagem);
                    //emailService.EnviaEmail(ParametrosAppConfig.EmailSms, sms.Mensagem + "@" + idMsg, sms.Numero);
                    if (response != "OK")
                    {
                        LogarErroSms(sms.Id, StatusSms.TentarNovamente, $"créditos: {tww.Creditos()}, validade: {tww.ValidadeCreditos()}");
                        continue;
                    }

                    sms.Status = StatusSms.Enviado;
                    smsService.Salvar(sms);
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry("ECJ_WS05 - Processar Sms Pendente" + ex.Message, EventLogEntryType.Error, 2);
                    LogarErroSms(sms.Id, StatusSms.Erro, ex.Message);
                }
            }

            eventLog1.WriteEntry("ECJ_WS05 - Fim Processar Sms Pendente", EventLogEntryType.Information, 5);
        }

        private void ProcessarEmailNaoEnviado()
        {
            eventLog1.WriteEntry("ECJ_WS05 - Processar Email Nao Enviado", EventLogEntryType.Information, 6);

            var emailService = new EmailService();


            emailService.LimpaEmailEnviado();
            eventLog1.WriteEntry("ECJ_WS05 - Deleta email enviado", EventLogEntryType.Information, 200);

            var listaEmailNaoEnviado =
                emailService.BuscarNaoEnviados(int.Parse(ConfigurationManager.AppSettings["QtdeSmsEnviadosPorVez"]));

            if (listaEmailNaoEnviado.Count == 0)
                return;

            foreach (var email in listaEmailNaoEnviado)
            {
                try
                {
                    email.DataInicioProcesso = DateTime.Now;
                    emailService.EnviaEmail(email.EmailDestinatario, "", email.CorpoEmail, email.AssuntoEmail);
                    email.Status = Status.Enviado;
                    emailService.Salvar(email);
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry("ECJ_WS05 - Processar Email Nao Enviado" + ex.Message, EventLogEntryType.Error, 3);
                    LogarErroEmail(email.Id, Status.TentarNovamente, ex.Message);
                }
            }

            eventLog1.WriteEntry("ECJ_WS05 - Fim Processar Email Nao Enviado", EventLogEntryType.Information, 7);
        }

        private void ProcessarEmailPendente()
        {
            eventLog1.WriteEntry("ECJ_WS05 -Processar Email Pendente", EventLogEntryType.Information, 8);

            var emailService = new EmailService();

            var listaEmailPendente = emailService.BuscarPendentes(int.Parse(ConfigurationManager.AppSettings["QtdeSmsEnviadosPorVez"]));

            if (listaEmailPendente.Count == 0)
                return;

            foreach (var email in listaEmailPendente)
            {
                try
                {
                    email.DataInicioProcesso = DateTime.Now;
                    emailService.EnviaEmail(email.EmailDestinatario, "", email.CorpoEmail, email.AssuntoEmail);
                    email.Status = Status.Enviado;
                    emailService.Salvar(email);
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry("ECJ_WS05 -Processar Email Pendente" + ex.Message, EventLogEntryType.Error, 4);
                    LogarErroEmail(email.Id, Status.TentarNovamente, ex.Message);
                }
            }
        }

        private void LogarErroEmail(int idEmail, Status status, string error = null)
        {
            var emailService = new EmailService();
            var emailAtualizavel = emailService.BuscarPor(idEmail);

            emailAtualizavel.Status = status;
            emailAtualizavel.Erro = error;

            emailService.Salvar(emailAtualizavel);
        }

        private void LogarErroSms(int idSms, StatusSms status, string error = null)
        {
            var smsService = new SmsService();
            var smsAtualizavel = smsService.BuscarPor(idSms);

            smsAtualizavel.Status = status;
            smsAtualizavel.Erro = error;

            smsService.Salvar(smsAtualizavel);
        }

        #endregion
        

    }
}