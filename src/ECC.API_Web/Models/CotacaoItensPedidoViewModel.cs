using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class CotacaoItensPedidoViewModel
	{
	    public CotacaoItensPedidoViewModel(int cotacaoId)
	    {
			CotacaoId = cotacaoId;
			Pedidos = new List<PedidoViewModel>();
		}
        public int CotacaoId { get; set; }

		public List<PedidoViewModel> Pedidos { get; set; }
	}
}