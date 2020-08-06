using System;

namespace ECC.API_Web.Models
{
    public class ItemPedidoViewModel
    {

		//public int ProdutoId { get; set; }
		//public int Quantidade { get; set; }
		public int Id { get; set; }
		public int sku { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public bool flgOutraMarca { get; set; }
        public bool flgDespacho { get; set; }
        public decimal? PrecoMedioUnit { get; set; }
        public decimal? PrecoNegociadoUnit { get; set; }
        public string Observacao { get; set; }
        public decimal? SubTotal { get; set; }
        public int? FornecedorId { get; set; }
		public int? FormaPagtoId { get; set; }
		public int PedidoId { get; set; }
		public int? CotacaoId { get; set; }
		public int? EntregaId { get; set; }
        public decimal? Desconto { get; set; }
        public decimal? TaxaEntrega { get; set; }
        public bool AprovacaoMembro { get; set; }
        public bool AprovacaoFornecedor { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataEntregaMembro { get; set; }
        public DateTime DataEntregaFornecedor { get; set; }
        public EntregaViewModel Entrega { get; set; }
		public FornecedorViewModel Fornecedor { get; set; }
        public int qtdForn { get; set; }
        
    }

    public class RemFornPedCotViewModel
    {
        public int forn { get; set; }
        public int prd { get; set; }
        public bool viaSistema { get; set; }

    }

}