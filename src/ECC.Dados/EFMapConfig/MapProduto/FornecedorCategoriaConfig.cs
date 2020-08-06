using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class FornecedorCategoriaConfig : EntidadeBaseConfig<FornecedorCategoria>
    {


        public FornecedorCategoriaConfig()
        {

            Property(fc => fc.FornecedorId).IsRequired();
            Property(fc => fc.CategoriaId).IsRequired();
            
        }
    }
}
