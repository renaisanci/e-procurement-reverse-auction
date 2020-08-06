
using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeEndereco;


namespace ECC.Dados.EFMapConfig.MapEndereco
{
    public class RegiaoConfig : EntityTypeConfiguration<Regiao>
    {

        public RegiaoConfig()
        {
            
            Property(b => b.DescRegiao).IsRequired().HasMaxLength(100);
            Property(b => b.EstadoId).IsRequired();
            HasMany(e => e.Cidades).WithRequired(r => r.Regiao).HasForeignKey(r => r.RegiaoId);
        }

    }
}
