using System.Data.Entity.Core;
using Microsoft.AspNet.SignalR.Client;

namespace ECC.SrvWin.GeraCotacao
{
    public class BroadcastGenerator
    {
        public static void Run()
        {
            const string url = @"http://localhost:8080";
            var connection = new HubConnection(url);
            var hub = connection.CreateHubProxy("BroadcastHub");
            connection.Start().Wait();
            string teste = connection.ConnectionId;


            //NotificaFornecedorNovaCotacao
            hub.Invoke("NotificaFornecedorNovaCotacao", 13, null).Wait();

        }



    }
}