using ECC.Entidades;
using System.Collections.Generic;


namespace ECC.EntidadeUsuario
{
    /// <summary>
    /// Perfil de Usuários
    /// </summary>
    public class Perfil : EntidadeBase
    {
        public string DescPerfil { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
