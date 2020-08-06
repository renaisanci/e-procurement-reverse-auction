using ECC.EntidadeFormaPagto;

namespace ECC.Dados.EFMapConfig.MapFormaPagto
{
    public class PromocaoFormaPagtoConfig : EntidadeBaseConfig<PromocaoFormaPagto>
    {

        public PromocaoFormaPagtoConfig()
        {
            Property(f => f.ProdutoPromocionalId).IsRequired();
            Property(f => f.FormaPagtoId).IsRequired();
        }
    }
}
