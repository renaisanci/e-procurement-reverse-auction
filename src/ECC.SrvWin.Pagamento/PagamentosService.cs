using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using ECC.Servicos;

namespace ECC.SrvWin.Pagamento
{
    partial class PagamentosService : ServiceBase
    {

        #region Private Variables

        private const string LogSource = "ECJ_WS06";
        private const string LogName = "ECJ_WS06Log";
        private System.Timers.Timer _timer;

        #endregion Private Variables


        #region Construtor

        public PagamentosService()
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

        #endregion


        #region Servico e Timer Operacoes

        protected override void OnStart(string[] args)
        {
            this._timer =
     new System.Timers.Timer(TimeSpan.FromHours(int.Parse(ConfigurationManager.AppSettings["Timer"])).TotalMilliseconds)
     {
         Enabled = true
     };
            eventLog1.WriteEntry($"Serviço de Geração de Pagamentos.", EventLogEntryType.Information, 1);


#if (!DEBUG)
            this._timer.Elapsed += this._timerGerarPagamentosService_Elapsed;
#else
            this.GerarPagamentos();
#endif
        }

        protected override void OnStop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            eventLog1.WriteEntry($"Serviço de Geração de Pagamentos Parado.", EventLogEntryType.Information, 1);
        }

        #endregion


        #region Métodos

        private void _timerGerarPagamentosService_Elapsed(object sender, ElapsedEventArgs args)
        {
            try
            {
                GerarPagamentos();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Pagamentos.\n\n{ex}", EventLogEntryType.Error, 1);
            }

        }

        public void GerarPagamentos()
        {
            //FctGerarComissoes();
            //FctGerarFaturas();
            //FctGerarMensalidades();
            FctGerarPreficificacaoFornecedor();
        }

        public void FctGerarComissoes()
        {
            try
            {
                eventLog1.WriteEntry($"Gerando Comissoes.", EventLogEntryType.Information, 1);
                var service = new PagamentoService();
                service.GerarComissoes();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Comissoes.\n\n{ex}", EventLogEntryType.Error, 1);
            }
        }

        public void FctGerarFaturas()
        {
            var date = DateTime.Now;
            var foraFimSemana = date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

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

        public void FctGerarMensalidades()
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


        public void FctGerarPreficificacaoFornecedor()
        {
            try
            {
                eventLog1.WriteEntry($"Gerando Precificação Cotação Fornecedores.", EventLogEntryType.Information, 1);
                var service = new PrecoCotacaoFornecedorService();
                service.PrecificarCotacaoFornecedor();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Mensalidades.\n\n{ex}", EventLogEntryType.Error, 1);
            }
        }

        #endregion
    }
}
