 
using ECC.EntidadeUsuario;

namespace ECC.Dados.EFMapConfig.MapUsuario
{
    public class GrupoConfig : EntidadeBaseConfig<Grupo>
    {

        public GrupoConfig()
        {


            HasMany(e => e.PermissoesGrupos).WithRequired(r => r.Grupo).HasForeignKey(r => r.GrupoId);

        }

    }
}
