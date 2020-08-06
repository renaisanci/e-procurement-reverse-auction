using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class CartaoBandeiraConfig : EntidadeBaseConfig<CartaoBandeira>
    {
        public CartaoBandeiraConfig()
        {
            Property(b => b.Descricao).IsRequired();
            Property(b => b.Ativo).IsRequired();
        }
    }
}
