using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;

namespace ECC.SrvWin.GeraCotacao
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration()
                {
                    EnableDetailedErrors = true,
                    EnableJSONP = true
                };

                map.RunSignalR(hubConfiguration);
            });
        }
    }
}