using ECC.EntidadeMenu;
using ECC.Entidades;

namespace ECC.EntidadeUsuario
{
    public class PermissaoGrupo : EntidadeBase
    {
        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        public int PerfilId { get; set; }
        public virtual Perfil Perfil { get; set; }

        public int GrupoId { get; set; }
        public virtual Grupo Grupo { get; set; }

        public bool FlgVisualizarMenu { get; set; }
        public bool FlgAlterar { get; set; }
        public bool FlgInserir { get; set; }
        public bool FlgExcluir { get; set; }
        public bool FlgConsultar { get; set; }
    }
}
