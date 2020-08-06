using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeProduto;
using ECC.Entidades.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class FornecedorProdutoConfig : EntidadeBaseConfig<FornecedorProduto>
    {
        public FornecedorProdutoConfig()
        {

            //HasKey(w => new { w.FornecedorId, w.ProdutoId, w.QuantidadeMinima });

            //Property(w => w.FornecedorId);
            //Property(w => w.ProdutoId);
            //Property(w => w.QuantidadeMinima);
            //Property(w => w.Valor).IsRequired();

            //HasKey(w => new { w.FornecedorId, w.ProdutoId });
            
            Property(w => w.Valor).IsRequired();
            Property(w => w.FornecedorId);
            Property(w => w.ProdutoId);

            //HasMany(t => t.ListaQuantidadeDesconto).WithRequired(t => t.FornecedorProduto).HasForeignKey(t => t.FornecedorProdutoId);

        }
    }
}
