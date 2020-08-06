namespace ECC.API_Web.Models
{
    public class CotacaoPedidosViewModel
	{
        public int CotacaoId { get; set; }

		public int PedidoId { get; set; }

		public CotacaoViewModel Endereco { get; set; }
		public PedidoViewModel Cotacao { get; set; }
		

	}
}