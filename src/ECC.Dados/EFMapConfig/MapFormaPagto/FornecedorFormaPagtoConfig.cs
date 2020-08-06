using ECC.EntidadeFormaPagto;

namespace ECC.Dados.EFMapConfig.MapFormaPagto
{
    public class FornecedorFormaPagtoConfig : EntidadeBaseConfig<FornecedorFormaPagto>
    {

        public FornecedorFormaPagtoConfig()
        {
            Property(ff => ff.FornecedorId).IsRequired();
            Property(ff => ff.FormaPagtoId).IsRequired();
            Property(ff => ff.Desconto).IsRequired().HasPrecision(18,1);
            Property(ff => ff.ValorMinParcela).IsOptional().HasPrecision(18,2);
            Property(ff => ff.ValorMinParcelaPedido).IsOptional().HasPrecision(18,2);
            Property(ff => ff.ValorPedido).IsRequired().HasPrecision(18,2);
        }
    }
}
