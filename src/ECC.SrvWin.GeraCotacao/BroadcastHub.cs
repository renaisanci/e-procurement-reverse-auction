using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ECC.SrvWin.GeraCotacao
{
    [HubName("BroadcastHub")]
    public class BroadcastHub : Hub
    {
        private readonly IHubContext _hubContext;

        public BroadcastHub()
        {
            this._hubContext = GlobalHost.ConnectionManager.GetHubContext<BroadcastHub>();
        }

        public void NotificaFornecedorNovaCotacao(int cotacaoId, List<string> tokenUsuarios)
        {
            //Clients.All.notificaFornecedorNovaCotacao(cotacaoId);
            this._hubContext.Clients.Clients(tokenUsuarios).notificaNovaCotacao(cotacaoId);
        }

    }
}