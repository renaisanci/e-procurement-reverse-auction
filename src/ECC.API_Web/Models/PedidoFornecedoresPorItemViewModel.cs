using ECC.EntidadePessoa;
using System;
using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class PedidoFornecedoresPorItemViewModel
    {
        public int PedidoId { get; set; }
        public DateTime DtPedido { get; set; }
             
        public int StatusId { get; set; }
        public string Status { get; set; }
        public int OrdemStatus { get; set; }
        public int QtdItem { get; set; }
		public decimal? ValorTotal { get; set; }

        public int PrazoEntrega { get; set; }
		public int CotacaoId { get; set; }

        public Boolean PedidoPromocional { get; set; }


        public List<ItemPedidosFornecedoresViewModel> ItemPedidosFornecedor { get; set; }

	}

    public class ItemPedidosFornecedoresViewModel {
        public ItemPedidoViewModel ItemPedidos { get; set; }

        public List<FornecedorLanceViewModel> FornecedorLance { get; set; }
    }

    public class FornecedorLanceViewModel
    {

        public FornecedorViewModel FornecedorLance { get; set; }

        public decimal valorLance { get; set; }
        public string obs { get; set; }
        public int qtd { get; set; }
    }

}