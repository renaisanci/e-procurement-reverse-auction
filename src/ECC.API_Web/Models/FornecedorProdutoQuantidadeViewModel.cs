
using System;

namespace ECC.API_Web.Models
{
    public class FornecedorProdutoQuantidadeViewModel
    {
        public string PercentualDesconto { get; set; }
        public int QuantidadeMinima { get; set; }
        public int FornecedorProdutoId { get; set; }

        public DateTime ValidadeQtdDesconto { get; set; }
    }
}