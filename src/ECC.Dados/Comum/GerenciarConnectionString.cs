using System;
using System.Configuration;
using System.Web;

namespace ECC.Dados.Comum
{
    public class GerenciarConnectionString
    {
        public static string GetCompleteConnectionString(string abrevConnectionString)
        {
            string strConnectionString = string.Empty;

            try
            {
                // é necessário: using System.Configuration;
                strConnectionString = ConfigurationManager.ConnectionStrings[GetAmbiente("ECC")].ConnectionString;
            }
            catch
            {
                strConnectionString = string.Empty;
            }

            if (string.IsNullOrEmpty(strConnectionString))
                throw new Exception("Erro 1923044123 encontrado! Por Favor entrar em contato com o suporte! Em breve o sistema será reestabelecido."); // Número de erro qualquer encontrado.

            return strConnectionString;

        }

        private static string GetAmbiente(string abrevConnectionString)
        {
            var ambiente = "PRD";
            try
            {
                var absoluteUri = HttpContext.Current.Request.UrlReferrer.AbsoluteUri.ToString().ToUpper();

                if (absoluteUri.Contains("://LOCALHOST"))
                    ambiente = "LOCALHOST";

                else if (absoluteUri.Contains("://DEV"))
                    ambiente = "DEV";

                else if (absoluteUri.Contains("://HOM"))
                    ambiente = "HOM";

                else
                    ambiente = "PRD";
            
            }
            catch (Exception)
            {

                ambiente = "DEV";
            }

            
            return string.Format("{0}_{1}", abrevConnectionString, ambiente);
        }
    }
}
