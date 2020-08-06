using ECC.Entidades;

namespace ECC.EntidadeUsuario
{
    public class UsuarioGrupo : EntidadeBase
    {
        public int UsuarioId { get; set; }

        public int GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }
    }
}
