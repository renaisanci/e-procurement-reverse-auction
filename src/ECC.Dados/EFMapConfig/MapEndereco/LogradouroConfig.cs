 
using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeEndereco;

namespace ECC.Dados.EFMapConfig.MapEndereco
{
    public class LogradouroConfig : EntityTypeConfiguration<Logradouro>
    {


        public LogradouroConfig() 
        {
 
            Property(b => b.DescLogradouro).IsRequired().HasMaxLength(100);

        }
    }
}
