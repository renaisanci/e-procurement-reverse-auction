
using ECC.EntidadeProduto;
using ECC.Entidades.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
   public  class MembroDemandaConfig : EntidadeBaseConfig<MembroDemanda>
    {


       public MembroDemandaConfig()
       {
            Property(p => p.MembroId).IsRequired();
            Property(p => p.SubCategoriaId).IsRequired();
            Property(p => p.PeriodicidadeId).IsRequired();
            Property(p => p.Quantidade).IsRequired();
            Property(p => p.UnidadeMedidaId).IsRequired();
            Property(p => p.Observacao).IsOptional().HasMaxLength(300);
       }
    }
}
