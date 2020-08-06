using ECC.EntidadeAvisos;

namespace ECC.Dados.EFMapConfig.MapAvisos
{
    public class TipoAvisosConfig : EntidadeBaseConfig<TipoAvisos>
    {

        public TipoAvisosConfig()
        {
            Property(x => x.ModuloId).IsRequired();
            Property(x => x.Descricao).IsRequired().HasMaxLength(255);

            HasRequired(x => x.Modulo).WithMany().HasForeignKey(x => x.ModuloId);
            HasMany(x => x.Notificacoes).WithRequired(x => x.TipoAvisos).HasForeignKey(x => x.TipoAvisosId);
        }
    }
}
