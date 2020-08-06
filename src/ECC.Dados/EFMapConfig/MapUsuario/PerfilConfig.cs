
using ECC.EntidadeUsuario;

namespace ECC.Dados.EFMapConfig.MapUsuario
{
    public class PerfilConfig : EntidadeBaseConfig<Perfil>
    {


        public PerfilConfig()
        {

            Property(p => p.DescPerfil).IsRequired().HasMaxLength(50);
            HasMany(e => e.Usuarios).WithRequired(r => r.Perfil).HasForeignKey(r => r.PerfilId);
        }
    }
}
