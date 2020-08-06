using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class CategoriaConfig : EntidadeBaseConfig<Categoria>
    {
        public CategoriaConfig()
        {
            HasMany(s => s.SubCategorias).WithRequired(t => t.Categoria).HasForeignKey(t => t.CategoriaId);
            Property(s => s.DescCategoria).IsRequired().HasMaxLength(100);
        }
    }
}
