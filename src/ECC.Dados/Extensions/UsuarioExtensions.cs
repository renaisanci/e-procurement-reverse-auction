using System.Linq;

using ECC.Dados.Repositorio;

using ECC.EntidadeUsuario;

namespace ECC.Dados.Extensions
{
    public static class UsuarioExtensions
    {
        public static Usuario GetSingleByEmail(this IEntidadeBaseRep<Usuario> usuarioRep, string email, int perfilModulo)
        {
            return usuarioRep.GetAll().FirstOrDefault(x => x.UsuarioEmail == email && x.Perfil.Id == perfilModulo);
        }
    }
}
