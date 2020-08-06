 
using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeEndereco;

namespace ECC.Dados.EFMapConfig.MapEndereco
{
   public  class BairroConfig :EntityTypeConfiguration<Bairro>
   {
       public BairroConfig()
       {
          
           Property(b => b.DescBairro).IsRequired().HasMaxLength(100);
           Property(b => b.CidadeId).IsRequired();
    
       }

   }
}
