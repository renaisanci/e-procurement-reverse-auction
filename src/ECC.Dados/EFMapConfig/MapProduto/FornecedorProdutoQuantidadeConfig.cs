using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class FornecedorProdutoQuantidadeConfig : EntidadeBaseConfig<FornecedorProdutoQuantidade>
    {
        public FornecedorProdutoQuantidadeConfig()
        {
            Property(p => p.PercentualDesconto).IsRequired();
            Property(p => p.QuantidadeMinima).IsRequired();
            Property(p => p.FornecedorProdutoId).IsRequired();
        }
    }
}
