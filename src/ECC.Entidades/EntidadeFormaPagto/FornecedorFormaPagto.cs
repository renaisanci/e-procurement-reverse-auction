using ECC.EntidadePessoa;
using ECC.Entidades;

namespace ECC.EntidadeFormaPagto
{
    public class FornecedorFormaPagto : EntidadeBase
    {
        public int FornecedorId { get; set; }
        public int FormaPagtoId { get; set; }
        public decimal Desconto { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public virtual FormaPagto FormaPagto { get; set; }
        public decimal? ValorMinParcela { get; set; }
        public decimal? ValorMinParcelaPedido { get; set; }
        public decimal? ValorPedido { get; set; }
        public bool Ativo { get; set; }

    }
}
