using ECC.EntidadePessoa;
using ECC.EntidadePedido;
using ECC.Entidades;

namespace ECC.EntidadeFrete
{
    public class FretePedidoFornecedor : EntidadeBase
    {
        public FretePedidoFornecedor()
        {

        }

        public int FornecedorId { get; set; }
        public int PedidoId { get; set; }
        public int TransportadoraId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public virtual Pedido Predido { get; set; }
        public virtual Transportadora Transportadora { get; set; }

        public bool MelhorFrete { get; set; }
        public decimal Peso { get; set; }
        public decimal AdValorem { get; set; }
        public decimal Pedagio { get; set; }
        public decimal ICMS { get; set; }
        public decimal ValorTotal { get; set; }

        public int PrazoEntragaDias { get; set; }
        public int CondicaoPagamentoDias { get; set; }
    }
}
