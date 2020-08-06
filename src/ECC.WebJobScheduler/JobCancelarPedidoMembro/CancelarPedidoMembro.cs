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


namespace ECC.WebJobScheduler.JobCancelarPedidoMembro
{
    public class CancelarPedidoMembro : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS11";
        private const string LogName = "ECJ_WS11Log";
        private EventLog eventLog11 = new EventLog();

        #endregion

        #region Execute

        public override void Execute(IJobExecutionContext context)
        {

            if (!EventLog.SourceExists(LogSource))
            {
                EventLog.CreateEventSource(LogSource, LogName);
            }

            eventLog11.Source = LogSource;
            eventLog11.Log = LogName;

            try
            {
                eventLog11.WriteEntry($"Serviço de Cancelamento de Pedidos Membro sem Aprovação.", EventLogEntryType.Information, 1);
                var _cancelarPedidoMembroService = new CancelarPedidoMembroService();
                _cancelarPedidoMembroService.CancelarPedidoMembro();

            }
            catch (Exception ex)
            {
                eventLog11.WriteEntry("ECJ_WS11 - Erro ao chamar método para cancelar pedidos do membro sem aprovação: " + ex.Message, EventLogEntryType.Error, 159);
            }
        }

        #endregion       

    }
}