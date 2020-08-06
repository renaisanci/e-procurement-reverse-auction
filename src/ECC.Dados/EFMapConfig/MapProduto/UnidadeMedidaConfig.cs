
using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class UnidadeMedidaConfig :EntidadeBaseConfig<UnidadeMedida>
    {
        public UnidadeMedidaConfig()
        {

            HasMany(t => t.Produtos).WithRequired(t => t.UnidadeMedida).HasForeignKey(t => t.UnidadeMedidaId);

        }
    }
}
