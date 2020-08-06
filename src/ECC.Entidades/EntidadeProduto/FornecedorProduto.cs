using System.Collections.Generic;
using ECC.EntidadePessoa;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeProduto
{
    public class FornecedorProduto  : EntidadeBase
    {
        public FornecedorProduto()
        {
            this.ListaQuantidadeDesconto = new List<FornecedorProdutoQuantidade>();
        }

        public int FornecedorId { get; set; }
        public int ProdutoId { get; set; }
        public decimal Valor { get; set; }
        public string CodigoProdutoFornecedor { get; set; }

        public virtual Fornecedor Fornecedor { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual ICollection<FornecedorProdutoQuantidade> ListaQuantidadeDesconto { get; set; }
    }
}
