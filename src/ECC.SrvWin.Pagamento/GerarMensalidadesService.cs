using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using ECC.Servicos;

namespace ECC.SrvWin.Pagamento
{
    partial class GerarMensalidadesService : ServiceBase
    {
        #region Private Variables

        private const string LogSource = "ECJ_WS06";
        private const string LogName = "ECJ_WS06Log";
        private System.Timers.Timer _timer;

        #endregion Private Variables

        #region Servico e Timer Operacoes

        public GerarMensalidadesService()
        {
            InitializeComponent();

            if (!EventLog.SourceExists(LogSource))
            {
                EventLog.CreateEventSource(LogSource, LogName);
            }

            eventLog1.Source = LogSource;
            eventLog1.Log = LogName;

#if (DEBUG)
            OnStart(null);
#endif
        }

        #endregion Servico e Timer Operacoes

        #region Metodos

        protected override void OnStart(string[] args)
        {
            this._timer = new Timer(TimeSpan.FromHours(int.Parse(ConfigurationManager.AppSettings["Timer"])).TotalMilliseconds)
            {
                Enabled = true
            };
            eventLog1.WriteEntry($"Serviço de Geração de Mensalidades Startado.", EventLogEntryType.Information, 1);

#if (!DEBUG)
            this._timer.Elapsed += this._timerGerarMensalidadesService_Elapsed;
#else
            this.FctGerarMensalidades();
#endif

        }

        private void FctGerarMensalidades()
        {
            var date = DateTime.Now;
            var foraFimSemana = date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

            if (date.Hour > 8 && date.Hour < 10 && foraFimSemana)
            {
                try
                {
                    eventLog1.WriteEntry($"Gerando Mensalidades.", EventLogEntryType.Information, 1);
                    var service = new PagamentoService();
                    service.GerarMensalidades();
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry($"Erro ao Gerar Mensalidades.\n\n{ex}", EventLogEntryType.Error, 1);
                }
            }

        }

        private void _timerGerarMensalidadesService_Elapsed(object sender, ElapsedEventArgs args)
        {

            try
            {
                this.FctGerarMensalidades();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Mensalidades.\n\n{ex}", EventLogEntryType.Error, 1);
            }          
        }

        protected override void OnStop()
        {

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            eventLog1.WriteEntry($"Serviço de Geração de Mensalidades Parado.", EventLogEntryType.Information, 1);

        }

        #endregion Metodos
    }
}
