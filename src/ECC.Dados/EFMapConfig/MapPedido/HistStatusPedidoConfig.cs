

using ECC.EntidadePedido;

namespace ECC.Dados.EFMapConfig.MapPedido
{
    public class HistStatusPedidoConfig : EntidadeBaseConfig<HistStatusPedido>
    {

        public HistStatusPedidoConfig()
        {
            Property(b => b.PedidoId).IsRequired();
            Property(b => b.StatusSistemaId).IsRequired();
            Property(b => b.DescMotivoCancelamento).IsOptional().HasMaxLength(200);
        }
    }


}
