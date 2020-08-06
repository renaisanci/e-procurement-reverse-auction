using ECC.EntidadePessoa;
using ECC.EntidadeProduto;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeCotacao
{
    public class ResultadoCotacao : EntidadeBase
    {


        public int CotacaoId { get; set; }
        public virtual Cotacao Cotacao { get; set; }
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }
        public int FornecedorId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public int Qtd { get; set; }
        public decimal PrecoNegociadoUnit { get; set; }
        public string Observacao { get; set; }
        public bool FlgOutraMarca { get; set; }

    }
}
