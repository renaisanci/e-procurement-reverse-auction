 
using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class SegmentoCategoriaConfig : EntidadeBaseConfig<SegmentoCategoria>
    {
        public SegmentoCategoriaConfig()
        {

            Property(sc => sc.CategoriaId).IsRequired();
            Property(mc => mc.SegmentoId).IsRequired();


        }

    }
}
