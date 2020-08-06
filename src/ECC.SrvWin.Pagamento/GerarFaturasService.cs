using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq.Expressions;
using System.ServiceProcess;
using System.Timers;
using ECC.Servicos;

namespace ECC.SrvWin.Pagamento
{
    partial class GerarFaturasService : ServiceBase
    {
        #region Private Variables

        private const string LogSource = "ECJ_WS07";
        private const string LogName = "ECJ_WS07Log";
        private System.Timers.Timer _timer;

        #endregion Private Variables

        #region Servico e Timer Operacoes

        public GerarFaturasService()
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

            eventLog1.WriteEntry($"Serviço de Geração de Faturas Startado.", EventLogEntryType.Information, 1);

#if (!DEBUG)
            this._timer.Elapsed += this._timerGerarFaturasService_Elapsed;
#else
            this.FctGerarFaturas();
#endif

        }

        private void FctGerarFaturas()
        {
            var date = DateTime.Now;
            var foraFimSemana =  date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
            
            if (date.Hour > 8 && date.Hour < 10 && foraFimSemana)
            {
                try
                {
                    eventLog1.WriteEntry($"Gerando Faturas.", EventLogEntryType.Information, 1);
                    var service = new PagamentoService();
                    service.GerarFaturas();
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry($"Erro ao Gerar Faturas.\n\n{ex}", EventLogEntryType.Error, 1);
                }
            }

        }

        private void _timerGerarFaturasService_Elapsed(object sender, ElapsedEventArgs args)
        {
            try
            {
                this.FctGerarFaturas();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Faturas.\n\n{ex}", EventLogEntryType.Error, 1);
            }

            //try
            //{
            //    this._timer?.Stop();
            //    var service = new PagamentoService();
            //    service.GerarFaturas();
            //    eventLog1.WriteEntry($"Gerando Faturas.", EventLogEntryType.Information, 1);
            //}
            //catch (Exception ex)
            //{
            //    eventLog1.WriteEntry($"Erro ao Gerar Faturas.\n\n{ex}", EventLogEntryType.Error, 1);
            //}
            //finally
            //{
            //    this._timer?.Start();
            //}
        }

        protected override void OnStop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            eventLog1.WriteEntry($"Serviço de Geração de Faturas Parado.", EventLogEntryType.Information, 1);

        }

        #endregion Metodos
    }
}
