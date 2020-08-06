using System.Collections.Generic;
using ECC.Entidades;
using ECC.EntidadeUsuario;

namespace ECC.EntidadeMenu
{
    public class Menu : EntidadeBase
    {
        public Menu()
        {
            MenuPai = new List<Menu>();

            PermissaoGrupo = new List<PermissaoGrupo>();
        }

        public int? MenuPaiId { get; set; }
        public virtual ICollection<Menu> MenuPai { get; set; }

        public virtual Menu MenuFilho { get; set; }
    
        public int ModuloId { get; set; }
        public virtual Modulo Modulo { get; set; }
        public string DescMenu { get; set; }

        public int Nivel { get; set; }

        public int Ordem { get; set; }

        public string Url { get; set; }

        public string FontIcon { get; set; }

        public string Feature1 { get; set; }

        public string Feature2 { get; set; }

        public virtual ICollection<PermissaoGrupo> PermissaoGrupo { get; set; }
    }
}
