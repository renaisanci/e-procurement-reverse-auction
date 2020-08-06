using System;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceProcess;
using System.Timers;
using System.Xml;
using ECC.Servicos;

namespace ECC.SrvWin.GeraCotacao
{
    partial class GeraCotacaoService : ServiceBase
    {

        #region Private Variables

        private const string LogSource = "ECJ_WS07";
        private const string LogName = "ECJ_WS07Log";
        private System.Timers.Timer _timer;

        #endregion Private Variables


        #region Construtor

        public GeraCotacaoService()
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
            this._timer = new Timer(TimeSpan.FromHours(int.Parse(ConfigurationManager.AppSettings["Timer"])).TotalMilliseconds)
            {
                Enabled = true
            };
            eventLog1.WriteEntry($"Serviço de Geração de Cotações.", EventLogEntryType.Information, 1);


#if (!DEBUG)
            this._timer.Elapsed += this._timerGerarCotacoesService_Elapsed;
#else
           this.GerarCotacoes();
#endif
        }

        protected override void OnStop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
            eventLog1.WriteEntry($"Serviço de Geração de Cotações Parado.", EventLogEntryType.Information, 1);
        }

        #endregion


        #region Métodos

        private void _timerGerarCotacoesService_Elapsed(object sender, ElapsedEventArgs args)
        {
            try
            {
                GerarCotacoes();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Cotações.\n\n{ex}", EventLogEntryType.Error, 1);
            }

        }

        private void GerarCotacoes()
        {

            try
            {
                var service = new CriarCotacaoService();
                var dataHoje = DateTime.Now.TimeOfDay;
                if (service.HoraCotacao.Hours == dataHoje.Hours && service.HoraCotacao.Minutes == dataHoje.Minutes)
                {
                    eventLog1.WriteEntry($"Gerando Cotações.", EventLogEntryType.Information, 1);
                    var objetoCotcao = service.GeraCotacaoPedidos();
                    new BroadcastHub().NotificaFornecedorNovaCotacao(objetoCotcao.CotacaoId, objetoCotcao.ListaTokenUsuarios);

                }

            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry($"Erro ao Gerar Cotações.\n\n{ex}", EventLogEntryType.Error, 1);
            }

        }

        #endregion

    }
}
