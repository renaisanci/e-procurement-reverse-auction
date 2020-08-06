 
using ECC.Entidades;
using ECC.EntidadeStatus;

namespace ECC.EntidadePedido
{
    public class HistStatusPedido : EntidadeBase
    {
        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }
        public int StatusSistemaId { get; set; }
        public virtual StatusSistema StatusSistema { get; set; }
        public string DescMotivoCancelamento { get; set; }
    }
}
