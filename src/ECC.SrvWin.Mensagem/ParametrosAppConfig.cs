using System.Configuration;

namespace ECC.SrvWin.Mensagem
{
    public class ParametrosAppConfig
    {
        public static int TimerSms
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["TimerSms"]);
            }
        }

        public static int QtdeSmsEnviadosPorVez
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["QtdeSmsEnviadosPorVez"]);
            }
        }

        public static int QtdeSmsConfirmarPorVez
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["QtdeSmsConfirmarPorVez"]);
            }
        }

        public static string SmsGatewayUser
        {
            get
            {
                return ConfigurationManager.AppSettings["SmsGatewayUser"];
            }
        }

        public static string SmsGatewayPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["SmsGatewayPassword"];
            }
        }
    }
}