using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeFormaPagto
{
    public class PromocaoFormaPagto : EntidadeBase
    {
        public int ProdutoPromocionalId { get; set; }
        public int FormaPagtoId { get; set; }
        public virtual ProdutoPromocional Fornecedor { get; set; }
        public virtual FormaPagto FormaPagto { get; set; }
    }
}
