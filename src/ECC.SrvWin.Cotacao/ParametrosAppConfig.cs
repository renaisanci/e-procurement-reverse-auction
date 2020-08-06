using System.Configuration;

namespace ECC.SrvWin.FecharCotacao
{
    public class ParametrosAppConfig
    {
        public static int TimerCotacao
        {
            get
            {
                
                return int.Parse(ConfigurationManager.AppSettings["TimerCotacao"]);
            }
        }

    }
}