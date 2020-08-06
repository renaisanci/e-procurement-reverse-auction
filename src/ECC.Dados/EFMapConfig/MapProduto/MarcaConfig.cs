using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class MarcaConfig : EntidadeBaseConfig<Marca>
    {
        public MarcaConfig()
        {

            Property(w => w.DescMarca).IsRequired().HasMaxLength(100);
            HasMany(w => w.Produtos).WithRequired(t => t.Marca).HasForeignKey(t => t.MarcaId);
        }

    }
}
