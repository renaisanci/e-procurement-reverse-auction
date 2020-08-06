

using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeEndereco;

namespace ECC.Dados.EFMapConfig.MapEndereco
{
    public class CidadeConfig : EntityTypeConfiguration<Cidade>
    {


        public CidadeConfig()
        {
       
            Property(e => e.DescCidade).IsRequired().HasMaxLength(100);
            Property(e => e.EstadoId).IsRequired();
            Property(e => e.RegiaoId).IsRequired();
            Property(e => e.CodigoIBGE).IsOptional();
            HasMany(e => e.Bairros).WithRequired(r => r.Cidade).HasForeignKey(r => r.CidadeId);  

        }

    }
}
