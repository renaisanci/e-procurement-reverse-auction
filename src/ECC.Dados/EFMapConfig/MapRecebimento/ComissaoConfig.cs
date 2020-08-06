using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class ComissaoConfig : EntidadeBaseConfig<Comissao>
    {
        public ComissaoConfig()
        {
            Property(b => b.Valor).IsRequired().HasColumnType("money");
            Property(b => b.FaturaId).IsOptional();
            Property(b => b.PedidoTotal).IsOptional().HasColumnType("money"); 

            HasOptional(x => x.Fatura).WithMany().HasForeignKey(x => x.FaturaId);
            HasOptional(x => x.Pedido).WithMany().HasForeignKey(x => x.PedidoId);
        }
    }
}
