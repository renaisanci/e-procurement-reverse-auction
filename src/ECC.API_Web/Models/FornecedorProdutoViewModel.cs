

using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class FornecedorProdutoViewModel
    {
        public int Id { get; set; }
        public int FornecedorId { get; set; }
        public int ProdutoId { get; set; }
        public decimal Valor { get; set; }
        public string CodigoProdutoFornecedor { get; set; }
        public decimal PercentMin { get; set; }
        public decimal PercentMax { get; set; }
        public List<FornecedorProdutoQuantidadeViewModel> ListaQuantidadeDesconto { get; set; }
    }


    public class FornecedorProdutoSalvaPostViewModel
    {
        public int FornecedorProdutoId { get; set; }
        public int FornecedorId { get; set; }
        public int ProdutoId { get; set; }
        public string CodigoProdutoFornecedor { get; set; }
        public string Valor { get; set; }
        public List<FornecedorProdutoQuantidadeViewModel> ListaQuantidadeDesconto { get; set; }
    }
}