using System;
using System.Collections.Generic;
using System.Diagnostics;
using ECC.Entidades.EntidadeComum;
using ECC.Servicos;
using ECC.Servicos.Abstrato;
using ECC.Servicos.ModelService;
using Microsoft.AspNet.SignalR.Client;
using Quartz;

namespace ECC.WebJobScheduler.JobPrecoCotacaoFornecedor
{
    public class PrecoCotacaoFornecedor : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS02";
        private const string LogName = "ECJ_WS02Log";
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
                eventLog2.WriteEntry($"Serviço de Precificação Cotação Fornecedores.", EventLogEntryType.Information, 1);

                PrecificarCotacoesFornecedor();

            }
            catch (Exception ex)
            {
                eventLog2.WriteEntry("ECJ_WS02 - Erro ao chamar método para preficar cotações de fornecedores: " + ex.Message, EventLogEntryType.Error, 156);
            }
        }


        #endregion


        #region Métodos

        private void PrecificarCotacoesFornecedor()
        {

            try
            {               

                Connection = new HubConnection(Url);
                Proxy = Connection.CreateHubProxy("Notificacoes");
                Connection.Start();

                PrecoCotacaoFornecedorService service = new PrecoCotacaoFornecedorService();
                eventLog2.WriteEntry($"Precificando Cotação Fornecedor.", EventLogEntryType.Information, 1);
                var resultado = service.PrecificarCotacaoFornecedor();
                
                resultado.ForEach(x => {

                    Proxy.Invoke("CotacaoNovoPreco", x.TokenUsuario, x.CotacaoId, x.DataFechamentoCotacao, x.CotacaoGrupo, DateTime.Now);

                });
              
                eventLog2.WriteEntry($"Precificação de Cotações para Fornecedores executada com sucesso!", EventLogEntryType.Information, 1);
            }
            catch (Exception ex)
            {
                eventLog2.WriteEntry($"Erro ao Precificar Cotações para Fornecedores.\n\n{ex}", EventLogEntryType.Error, 1);
            }

        }

        public static string IdentificaAmbiente()
        {
            var amb = string.Empty;

            if (Ambiente.ToUpper() == "DEV")
            {
                amb = "http://localhost:1310/signalr";

            }
            else if (Ambiente.ToUpper() == "HOM")
            {
                amb = "http://homadm.economizaja.com.br/signalr";
            }
            else
            {
                amb = "https://adm.economizaja.com.br/signalr";
            }

            return amb;
        }

        #endregion

    }
}