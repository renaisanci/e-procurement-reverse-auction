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


namespace ECC.WebJobScheduler.JobPrecoCotacaoFornecedor
{
    public class LembreteFornecedor : BaseJob
    {

        #region Propriedades

        private const string LogSource = "ECJ_WS09";
        private const string LogName = "ECJ_WS09Log";
        private EventLog eventLog9 = new EventLog();
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

            eventLog9.Source = LogSource;
            eventLog9.Log = LogName;

            try
            {
                eventLog9.WriteEntry($"Serviço Lembrete de Aceite de solicitação para Membro .", EventLogEntryType.Information, 1);
                LembreteFornecedorAceiteMembro();

            }
            catch (Exception ex)
            {
                eventLog9.WriteEntry("ECJ_WS09 - Erro ao chamar método para lembrar fornecedor para aceitar membro: " + ex.Message, EventLogEntryType.Error, 156);
            }
        }

        #endregion


        #region Métodos

        private void LembreteFornecedorAceiteMembro()
        {

            try
            {
                var dataHoje = DateTime.Now;

                if (dataHoje.DayOfWeek != DayOfWeek.Saturday && dataHoje.DayOfWeek != DayOfWeek.Sunday)
                {
                    Connection = new HubConnection(Url);
                    Proxy = Connection.CreateHubProxy("Notificacoes");
                    Connection.Start();

                    NotificacoesAlertasService service = new NotificacoesAlertasService();
                    eventLog9.WriteEntry($"Lembrando fornecedor de aceitar mebro.", EventLogEntryType.Information, 1);
                    List<Usuario> usuarios = service.EnviarEmailSmsFornecedorAceitarMembro();

                    if (usuarios.Count > 0)
                        Proxy.Invoke("LembreteFornecedorAceiteMembros", usuarios.Select(c => c.TokenSignalR).ToList());

                    eventLog9.WriteEntry($"Lembrete de aceite para fornecedor executada com sucesso!", EventLogEntryType.Information, 1);
                }

            }
            catch (Exception ex)
            {
                eventLog9.WriteEntry($"Erro ao lembrar fornecedor para aceitar membro.\n\n{ex}", EventLogEntryType.Error, 1);
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