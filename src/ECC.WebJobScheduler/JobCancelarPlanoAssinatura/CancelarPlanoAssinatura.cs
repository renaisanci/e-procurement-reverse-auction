using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ECC.Entidades.EntidadeComum;
using ECC.EntidadeUsuario;
using ECC.Servicos;
using ECC.Servicos.Abstrato;
using ECC.Servicos.ModelService;
using Microsoft.AspNet.SignalR.Client;
using Quartz;


namespace ECC.WebJobScheduler.JobCancelarPlanoAssinatura
{
    public class CancelarPlanoAssinatura : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS10";
        private const string LogName = "ECJ_WS10Log";
        private EventLog eventLog10 = new EventLog();

        #endregion

        #region Execute

        public override void Execute(IJobExecutionContext context)
        {

            if (!EventLog.SourceExists(LogSource))
            {
                EventLog.CreateEventSource(LogSource, LogName);
            }

            eventLog10.Source = LogSource;
            eventLog10.Log = LogName;

            try
            {
                eventLog10.WriteEntry($"Serviço de Cancelamento de Plano do Membro e Acesso do Fornecedor.", EventLogEntryType.Information, 1);
                var _cancelarPlanoAssinaturaService = new CancelarPlanoAssinaturaService();
                _cancelarPlanoAssinaturaService.CancelarPlanoAssinatura();

            }
            catch (Exception ex)
            {
                eventLog10.WriteEntry("ECJ_WS10 - Erro ao chamar método para cancelar plano do membro e acesso do fornecedor: " + ex.Message, EventLogEntryType.Error, 159);
            }
        }

        #endregion       

    }
}