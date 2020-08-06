using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
   public  class SegmentoConfig : EntidadeBaseConfig<Segmento>
    {

       public SegmentoConfig()
       {

            Property(w => w.DescSegmento).IsRequired().HasMaxLength(100);
            //HasMany(w => w.Produtos).WithRequired(t => t.Marca).HasForeignKey(t => t.MarcaId);
        }

    }
}
