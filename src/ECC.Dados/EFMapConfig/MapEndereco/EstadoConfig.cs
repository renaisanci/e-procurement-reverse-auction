 
using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeEndereco;
 

namespace ECC.Dados.EFMapConfig.MapEndereco
{
    public class EstadoConfig :EntityTypeConfiguration<Estado>
    {

        public EstadoConfig()
        {
           
            Property(e => e.DescEstado).IsRequired().HasMaxLength(100);
            Property(e => e.Uf).IsRequired().HasMaxLength(2);
            HasMany(e => e.Cidades).WithRequired(r => r.Estado).HasForeignKey(r => r.EstadoId);
            HasMany(e => e.Regioes).WithRequired(r => r.Estado).HasForeignKey(r => r.EstadoId);  
      
        }
    }
}
