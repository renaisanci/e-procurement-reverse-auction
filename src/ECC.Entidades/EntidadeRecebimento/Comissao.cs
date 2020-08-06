using ECC.EntidadePedido;
using ECC.Entidades;

namespace ECC.EntidadeRecebimento
{
    public class Comissao : EntidadeBase
    {
        public Comissao()
        {
            
        }
        
        public decimal Valor { get; set; }

        public int FaturaId { get; set; }

        public virtual Fatura Fatura { get; set; }

        public int PedidoId { get; set; }

        public virtual Pedido Pedido { get; set; }

        public decimal PedidoTotal { get; set; }
    }
}
