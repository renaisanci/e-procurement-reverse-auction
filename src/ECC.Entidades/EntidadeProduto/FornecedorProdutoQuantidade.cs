
using ECC.Entidades;
using System;

namespace ECC.EntidadeProduto
{
    public class FornecedorProdutoQuantidade : EntidadeBase
    {
        public FornecedorProdutoQuantidade()
        {

        }

        public decimal PercentualDesconto { get; set; }
        public int QuantidadeMinima { get; set; }

        public int FornecedorProdutoId { get; set; }

        public virtual FornecedorProduto FornecedorProduto { get; set; }

        public DateTime ValidadeQtdDesconto { get; set; }
    }
}
