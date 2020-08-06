
using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class SubCategoriaConfig : EntidadeBaseConfig<SubCategoria>
    {

       public SubCategoriaConfig()
       {

           HasMany(s => s.Produtos).WithRequired(t => t.SubCategoria).HasForeignKey(t => t.SubCategoriaId);

           Property(s => s.DescSubCategoria).IsRequired().HasMaxLength(100);


       }

    }
}
