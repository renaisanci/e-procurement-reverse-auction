 
using ECC.EntidadeMenu;

namespace ECC.Dados.EFMapConfig.MapMenu
{
    public class ModuloConfig : EntidadeBaseConfig<Modulo>
    {




        public ModuloConfig()
        {

            HasMany(e => e.Menus).WithRequired(r => r.Modulo).HasForeignKey(r => r.ModuloId);

        }


    }
}
