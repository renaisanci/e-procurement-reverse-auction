using System;
using System.Configuration;
using System.Diagnostics;
using ECC.Servicos;
using System.ServiceProcess;

namespace ECC.SrvWin.Pagamento
{
    partial class GerarComissoesService : ServiceBase
    {
        #region Private Variables

        private const string LogSource = "ECJ_WS08";
        private const string LogName = "ECJ_WS08Log";
        private System.Timers.Timer _timer;

        #endregion Private Variables

        #region Servico e Timer Operacoes

        public GerarComissoesService()
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
            this._timer =
                new System.Timers.Timer(TimeSpan.FromHours(int.Parse(ConfigurationManager.AppSettings["Timer"])).TotalMilliseconds)
                {
                    Enabled = true
                };
            eventLog1.WriteEntry($"Serviço de Geração de Comissões Startado.", EventLogEntryType.Information, 1);


#if (!DEBUG)
            this._timer.Elapsed += this._timerGerarComissoesService_Elapsed;
#else
            this.FctGerarComissoes();
#endif


        }

        private void FctGerarComissoes()
        {
            try
            {
                eventLog1.WriteEntry($"Gerando Comissões.", EventLogEntryType.Information, 1);
                var service = new PagamentoService();
                service.GerarComissoes();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Comissões.\n\n{ex}", EventLogEntryType.Error, 1);
            }
        }

        private void _timerGerarComissoesService_Elapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                this.FctGerarComissoes();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Comissões.\n\n{ex}", EventLogEntryType.Error, 1);
            }

            //try
            //{
            //    this._timer?.Stop();
            //    var service = new PagamentoService();
            //    service.GerarComissoes();
            //    eventLog1.WriteEntry($"Gerando Comissões.", EventLogEntryType.Information, 1);
            //}
            //catch (Exception ex)
            //{
            //    eventLog1.WriteEntry($"Erro ao Gerar Comissões.\n\n{ex}", EventLogEntryType.Error, 1);
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
            eventLog1.WriteEntry($"Serviço de Geração de Comissões Parado.", EventLogEntryType.Information, 1);
        }

        #endregion Metodos
    }
}
