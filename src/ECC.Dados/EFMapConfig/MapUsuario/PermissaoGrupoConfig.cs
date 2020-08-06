using ECC.EntidadeUsuario;

namespace ECC.Dados.EFMapConfig.MapUsuario
{
    public class PermissaoGrupoConfig : EntidadeBaseConfig<PermissaoGrupo>
    {


        public PermissaoGrupoConfig()
        {

            Property(e => e.PerfilId).IsRequired();
            Property(e => e.GrupoId).IsRequired();
            Property(e => e.MenuId).IsRequired();

        }


    }
}
