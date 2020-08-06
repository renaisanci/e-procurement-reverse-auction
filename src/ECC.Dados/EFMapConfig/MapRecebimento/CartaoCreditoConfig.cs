using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class CartaoCreditoConfig : EntidadeBaseConfig<CartaoCredito>
    {
        public CartaoCreditoConfig()
        {
            Property(b => b.Nome).IsRequired();
            Property(b => b.Numero).IsRequired();
            Property(b => b.DataVencimento).IsRequired();
            Property(b => b.Cvc).IsRequired().HasMaxLength(250);
            Property(b => b.TokenCartaoGerenciaNet).IsRequired().HasMaxLength(300);
            Property(b => b.MembroId).IsRequired();
            Property(b => b.CartaoBandeiraId).IsRequired();
            Property(b => b.Padrao).IsRequired();
            Property(b => b.Ativo).IsRequired();
            HasRequired(x => x.Membro).WithMany().HasForeignKey(x => x.MembroId);
            HasRequired(x => x.CartaoBandeira).WithMany().HasForeignKey(x => x.CartaoBandeiraId);

        }
    }
}
