using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using ECC.Servicos;
using Quartz;

namespace ECC.ClientJob.JobPagamentos
{
    public class GeraMensalidade : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS06";
        private const string LogName = "ECJ_WS06Log";
        private EventLog eventLog = new EventLog();

        #endregion


        #region Execute

        public override void Execute(IJobExecutionContext context)
        {

            if (!EventLog.SourceExists(LogSource))
            {
                EventLog.CreateEventSource(LogSource, LogName);
            }

            eventLog.Source = LogSource;
            eventLog.Log = LogName;


            try
            {
                eventLog.WriteEntry($"Serviço para gerar mensalidades.", EventLogEntryType.Information, 1);

                // Chamar método para gerar mensalidades
                FctGerarMensalidades();

            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Erro ao chamar método para gerar mensalidades: " + ex.Message, EventLogEntryType.Error, 156);
            }
        }


        #endregion


        #region Métodos

        private void FctGerarMensalidades()
        {
            var date = DateTime.Now;
            var foraFimSemana = date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

            if (date.Hour > 8 && date.Hour < 10 && foraFimSemana)
            {
                try
                {
                    eventLog.WriteEntry($"Gerando mensalidades.", EventLogEntryType.Information, 1);
                    var service = new PagamentoService();
                    service.GerarMensalidades();
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry($"Erro ao gerar mensalidades.\n\n{ex}", EventLogEntryType.Error, 1);
                }
            }

        }

        #endregion

    }
}