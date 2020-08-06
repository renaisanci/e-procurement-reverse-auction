 
using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class MembroCategoriaConfig : EntidadeBaseConfig<MembroCategoria>
    {
        public MembroCategoriaConfig()
        {

            Property(mc => mc.MembroId).IsRequired();
            Property(mc => mc.CategoriaId).IsRequired();


        }

    }
}
