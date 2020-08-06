using System;
using System.Diagnostics;
using ECC.Servicos;
using Quartz;

namespace ECC.ClientJob.JobPagamentos
{
    public class GeraFatura : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS07";
        private const string LogName = "ECJ_WS07Log";
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
                eventLog.WriteEntry($"Serviço para gerar faturas.", EventLogEntryType.Information, 1);

                // Chamar método para gerar faturas
                FctGerarFaturas();

            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Erro ao chamar método para gerar faturas: " + ex.Message, EventLogEntryType.Error, 156);
            }
        }


        #endregion
        

        #region Métodos

        private void FctGerarFaturas()
        {
            var date = DateTime.Now;
            var foraFimSemana = date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

            if (date.Hour > 8 && date.Hour < 10 && foraFimSemana)
            {
                try
                {
                    eventLog.WriteEntry($"Gerando Faturas.", EventLogEntryType.Information, 1);
                    var service = new PagamentoService();
                    service.GerarFaturas();
                }
                catch (Exception ex)
                {
                    eventLog.WriteEntry($"Erro ao Gerar Faturas.\n\n{ex}", EventLogEntryType.Error, 1);
                }
            }

        }

        #endregion

    }
}