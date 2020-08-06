using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class FabricanteConfig: EntidadeBaseConfig<Fabricante>
    {
        public FabricanteConfig() {

            Property(w => w.DescFabricante).IsRequired().HasMaxLength(100);
            HasMany(w => w.Produtos).WithOptional(t => t.Fabricante).HasForeignKey(t => t.FabricanteId);
        }

    }
}
