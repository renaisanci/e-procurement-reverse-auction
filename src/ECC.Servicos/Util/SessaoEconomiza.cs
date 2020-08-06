using Microsoft.AspNet.Identity;
using System.Web;

namespace ECC.Servicos.Util
{
    public class SessaoEconomiza
    {
        private static int _usuarioId;

        public static int UsuarioId
        {
            get
            {
                if (contextoHttpIndisponivel())
                    return _usuarioId;

                return int.Parse(HttpContext.Current.User.Identity.GetUserId());
            }
            set
            {
                if (contextoHttpIndisponivel())
                {
                    _usuarioId = value;
                }
            }
        }

        public static string UsuarioNome
        {
            get
            {
                return HttpContext.Current.User.Identity.GetUserName();
            }
        }

        private static bool contextoHttpIndisponivel()
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null || HttpContext.Current.User.Identity == null)
                return true;

            return false;
        }
    }
}
