using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class CotacaoItensPedidoPFViewModel
	{
	    public CotacaoItensPedidoPFViewModel(int cotacaoId)
	    {
			CotacaoId = cotacaoId;
			Pedidos = new List<PedidoPFViewModel>();
		}
        public int CotacaoId { get; set; }

		public List<PedidoPFViewModel> Pedidos { get; set; }
	}
}