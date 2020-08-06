
using System.Security.Principal;
using ECC.EntidadeUsuario;


namespace ECC.Servicos.Util
{
    public class MembershipContext
    {
        public IPrincipal Principal { get; set; }
        public Usuario Usuario { get; set; }
        public bool IsValid()
        {
            return Principal != null;
        }
    }
}
