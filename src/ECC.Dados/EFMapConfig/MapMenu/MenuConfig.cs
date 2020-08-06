using ECC.EntidadeMenu;

namespace ECC.Dados.EFMapConfig.MapMenu
{
    public class MenuConfig : EntidadeBaseConfig<Menu>
    {

        public MenuConfig()
        {

            Property(e => e.ModuloId).IsRequired();

            HasMany(e => e.MenuPai)
                .WithOptional(e => e.MenuFilho)
                .HasForeignKey(e => e.MenuPaiId);

            HasMany(e => e.PermissaoGrupo)
            .WithRequired(e => e.Menu)
            .HasForeignKey(e => e.MenuId);

        }


    }
}
