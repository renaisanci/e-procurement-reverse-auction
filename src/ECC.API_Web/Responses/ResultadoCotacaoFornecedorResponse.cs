using System.Collections.Generic;
using ECC.API_Web.Models;

namespace ECC.API_Web.Responses
{
    public class ResultadoCotacaoFornecedorResponse
    {
        public int IdFornecedor { get; set; }
        public int IdProduto { get; set; }
        public string DescProduto { get; set; }
        public string Marca { get; set; }
        public decimal PrecoNegociadoUnit { get; set; }
        public int Quantidade { get; set; }
        public int MeuPreco { get; set; }
    }
}