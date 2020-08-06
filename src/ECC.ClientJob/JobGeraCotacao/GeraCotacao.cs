using System;
using System.Diagnostics;
using ECC.Servicos;
using ECC.Servicos.ModelService;
using Microsoft.AspNet.SignalR.Client;
using Quartz;

namespace ECC.ClientJob.JobGeraCotacao
{
    public class GeraCotacao : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS07";
        private const string LogName = "ECJ_WS07Log";
        private EventLog eventLog2 = new EventLog();
        private string Url = IdentificaAmbiente();
        private string _ambiente;
        private HubConnection Connection { get; set; }
        public static string Ambiente
        {
            get
            {
                return Environment.GetEnvironmentVariable("Amb_EconomizaJa");
            }
           
        }
        private IHubProxy Proxy { get; set; }

        #endregion


        #region Execute

        public override void Execute(IJobExecutionContext context)
        {

            if (!EventLog.SourceExists(LogSource))
            {
                EventLog.CreateEventSource(LogSource, LogName);
            }

            eventLog2.Source = LogSource;
            eventLog2.Log = LogName;


            try
            {
                eventLog2.WriteEntry($"Serviço para Geração de Cotações.", EventLogEntryType.Information, 1);

                GerarCotacoes();

            }
            catch (Exception ex)
            {
                eventLog2.WriteEntry("ECJ_WS05 - Erro ao chamar método para gerar cotação: " + ex.Message, EventLogEntryType.Error, 156);
            }
        }


        #endregion


        #region Métodos

        private void GerarCotacoes()
        {

            try
            {
                Connection = new HubConnection(Url);
                Proxy = Connection.CreateHubProxy("Notificacoes");
                Connection.Start();

                CriarCotacaoService service = new CriarCotacaoService();
                eventLog2.WriteEntry($"Gerando Cotações.", EventLogEntryType.Information, 1);
                var objetoCotcao = service.GeraCotacaoPedidos();

                if (objetoCotcao.CotacaoId > 0)
                    EnviaNotificacaoHub(objetoCotcao);
                else
                    eventLog2.WriteEntry($"Não existem pedidos para gerar cotação!", EventLogEntryType.Information, 1);
            }
            catch (Exception ex)
            {
                eventLog2.WriteEntry($"Erro ao Gerar Cotações.\n\n{ex}", EventLogEntryType.Error, 1);
            }

        }

        private void EnviaNotificacaoHub(GeraCotacaoViewModel cota)
        {
            try
            {
                Proxy.Invoke("NotificaNovaCotacao", cota).Wait();
                eventLog2.WriteEntry($"Notificação enviada com sucesso!", EventLogEntryType.Information, 1);

            }
            catch (Exception ex)
            {
                eventLog2.WriteEntry($"Erro ao enviar notificação para fornecedores.\n\n{ex}", EventLogEntryType.Error, 1);
            }
        }
        
        public static string IdentificaAmbiente()
        {
            var amb = string.Empty;

            if (Ambiente == "Dev")
            {
                amb = "http://localhost:1310/signalr";
               
            }else if (Ambiente == "Hom")
            {
                amb = "http://homadm.economizaja.com.br/signalr";
            }
            else
            {
                amb = "http://adm.economizaja.com.br/signalr";
            }

            return amb;
        }

        #endregion

    }
}