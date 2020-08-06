using ECC.Entidades;
using System.Collections.Generic;

namespace ECC.EntidadeMenu
{
    public class Modulo : EntidadeBase
    {


        public Modulo()
        {
            Menus = new List<Menu>();


        }
        public string DescModulo { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
    }
}
