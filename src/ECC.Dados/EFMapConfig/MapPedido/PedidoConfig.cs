
using ECC.EntidadePedido;

namespace ECC.Dados.EFMapConfig.MapPedido
{
    public class PedidoConfig : EntidadeBaseConfig<Pedido>
    {
        public PedidoConfig()
        {            
            Property(b => b.MembroId).IsRequired();
            Property(b => b.EnderecoId).IsRequired();
            Property(b => b.StatusSistemaId).IsRequired();
            HasMany(e => e.ItemPedidos).WithRequired(r => r.Pedido).HasForeignKey(r => r.PedidoId);
            HasMany(e => e.Comissoes).WithRequired(r => r.Pedido).HasForeignKey(r => r.PedidoId);
        }
    }
}
