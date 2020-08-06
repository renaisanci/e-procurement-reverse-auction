using ECC.EntidadeUsuario;

namespace ECC.Dados.EFMapConfig.MapUsuario
{
    public class UsuarioGrupoConfig : EntidadeBaseConfig<UsuarioGrupo>
    {

        public UsuarioGrupoConfig()
        {

            Property(ur => ur.UsuarioId).IsRequired();
            Property(ur => ur.GrupoId).IsRequired();

        }

    }
}
