using System.Data.Entity.ModelConfiguration;
using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class FranquiaProdutoConfig : EntidadeBaseConfig<FranquiaProduto>  
    {
        public FranquiaProdutoConfig()
        {
            HasKey(w => new { w.FranquiaId, w.ProdutoId });

            Property(w => w.FranquiaId);
            Property(w => w.ProdutoId);
        }
    }
}
