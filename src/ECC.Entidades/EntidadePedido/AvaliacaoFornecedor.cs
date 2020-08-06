using ECC.EntidadePessoa;
using ECC.Entidades;

namespace ECC.EntidadePedido
{
    public class AvaliacaoFornecedor : EntidadeBase
    {
        public AvaliacaoFornecedor()
        {

        }

        public int PedidoId { get; set; }

        public int FornecedorId { get; set; }

        public int NotaQualidadeProdutos { get; set; }

        public int NotaTempoEntrega { get; set; }

        public int NotaAtendimento { get; set; }

        public string ObsQualidade { get; set; }

        public string ObsEntrega { get; set; }

        public string ObsAtendimento { get; set; }

        public virtual Fornecedor Fornecedor { get; set; }

        public virtual Pedido Pedido { get; set; }

    }
}
