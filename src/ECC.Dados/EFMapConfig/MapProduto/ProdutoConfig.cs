using ECC.Entidades.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class ProdutoConfig : EntidadeBaseConfig<Produto>
    {
        public ProdutoConfig()
        {
            Property(p => p.SubCategoriaId).IsRequired();
            Property(p => p.ProdutoPromocionalId).IsOptional();
            Property(p => p.MarcaId).IsRequired();
            Property(p => p.FabricanteId).IsOptional();
            Property(p => p.UnidadeMedidaId).IsRequired();
            Property(p => p.Especificacao).HasMaxLength(6500);
            Property(p => p.DescProduto).HasMaxLength(500);

            HasMany(t => t.Imagens).WithRequired(t => t.Produto).HasForeignKey(t => t.ProdutoId);
            HasMany(t => t.Rankings).WithRequired(t => t.Produto).HasForeignKey(t => t.ProdutoId);
            HasMany(t => t.Fornecedores).WithRequired(t => t.Produto).HasForeignKey(t => t.ProdutoId);
            HasMany(t => t.Franquias).WithRequired(t => t.Produto).HasForeignKey(t => t.ProdutoId);
        }
    }
}
