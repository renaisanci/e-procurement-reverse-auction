using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.EntidadeComum
{
    public class ResultadoPrecoCotacaoFornecedor
    {
        public bool indProdutoIndosponivel { get; set; }
        public bool indIndisponivelPermanente { get; set; }
        public bool indPrecoIgual { get; set; }
        public bool indMaisPrecoIgual { get; set; }
        public DateTime? dtProdutoIndisponivel { get; set; }
        public string DescProduto { get; set; }
        public string DescMarca { get; set; }
        public string Especificacao { get; set; }
        public string Observacao { get; set; }
        public int ProdutoId { get; set; }
        public string PrecoNegociadoUnit { get; set; }
        public decimal? menorPreco { get; set; }
        public string PrecoMedio { get; set; }


        public string PrecoMedioMercado { get; set; }
        public bool? FlgOutraMarca { get; set; }
        public int qtd { get; set; }
    }
}
