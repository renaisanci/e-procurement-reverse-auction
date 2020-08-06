using System.Configuration;

namespace ECC.SrvWin.EmailSms
{
    public class ParametrosAppConfig
    {
        public static int TimerSms => int.Parse(ConfigurationManager.AppSettings["TimerSms"]);

        public static int QtdeSmsEnviadosPorVez => int.Parse(ConfigurationManager.AppSettings["QtdeSmsEnviadosPorVez"]);

        public static int QtdeSmsConfirmarPorVez => int.Parse(ConfigurationManager.AppSettings["QtdeSmsConfirmarPorVez"]);

        public static string EmailSms => ConfigurationManager.AppSettings["EmailSms"];
    }
}
