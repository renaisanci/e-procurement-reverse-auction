using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeEndereco;

namespace ECC.Dados.EFMapConfig.MapEndereco
{
    public class PeriodoEntregaConfig : EntityTypeConfiguration<PeriodoEntrega>
    {
        public PeriodoEntregaConfig()
        {
            
            Property(b => b.DescPeriodoEntrega).IsRequired().HasMaxLength(20);
          
        }
    }
}
