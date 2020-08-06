using System.Configuration;

namespace ECC.SrvWin.NotificacoesAlertas
{
    public class ParametrosAppConfig
    {
        public static int TimerNotificacoesAlertas
        {
            get
            {

                return int.Parse(ConfigurationManager.AppSettings["TimerAlertas"]);
            }
        }

    }
}