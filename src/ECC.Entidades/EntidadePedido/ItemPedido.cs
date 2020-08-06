using System;
using ECC.EntidadePessoa;
using ECC.Entidades;
using System.Collections.Generic;
using ECC.EntidadeEntrega;
using ECC.EntidadeFormaPagto;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeUsuario;

namespace ECC.EntidadePedido
{
	public class ItemPedido : EntidadeBase
	{
		public int PedidoId { get; set; }
		public virtual Pedido Pedido { get; set; }
		public int ProdutoId { get; set; }
		public virtual Produto Produto { get; set; }
        public int? FornecedorId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public int? FornecedorUsuarioId { get; set; }
        public virtual Usuario FornecedorUsuario { get; set; }
        public int? FormaPagtoId { get; set; }
		public virtual FormaPagto FormaPagto { get; set; }
		public int Quantidade { get; set; }
        public decimal? PrecoMedioUnit { get; set; }
		public decimal? PrecoNegociadoUnit { get; set; }
		public string Observacao { get; set; }
		public bool AprovacaoFornecedor { get; set; }
        public bool AprovacaoMembro { get; set; }
        public decimal? Desconto { get; set; }
       public decimal? TaxaEntrega { get; set; }

        /// <summary>
        /// Valor Inicial: false.
        /// quando true o cliente comprador aceita a possibilidade ser de outra marca
        /// </summary>
        public bool FlgOutraMarca { get; set; }

        /// <summary>
        /// Valor Inicial: false.
        /// quando true o fornecedor despachou os itens para entrega
        /// </summary>
        public bool FlgDespacho { get; set; }
        /// <summary>
		/// Valor Inicial: 1.
		/// Aguardando Entrega: Indica que o produto ainda não foi entregue.
		/// </summary>
		public int? EntregaId { get; set; }
		public virtual Entrega Entrega { get; set; }
        public DateTime? DataEntregaMembro { get; set; }
        public DateTime? DataEntregaFornecedor { get; set; }
        /// <summary>
        /// 1 - Valor Coerente com pedido Mínimo do Fornecedor
        /// 2 - Valor Menor que o Valor de Pedido Mínimo do fornecedor, deve ser alterada quantidade ou depende do Fornecedor poder entregar
        /// 3 - Item com quantidade alterada, deve avisar o Fornecedor que neste item teve alteração no momento da aprovação.
        /// </summary>
	}

	public class ItemPedidoComparer : IEqualityComparer<ItemPedido>
	{
		public bool Equals(ItemPedido x, ItemPedido y)
		{
			//Check whether the objects are the same object. 
			if (Object.ReferenceEquals(x, y)) return true;

			//Check whether the products' properties are equal. 
			return x != null && y != null && x.PedidoId.Equals(y.PedidoId);
		}

		public int GetHashCode(ItemPedido obj)
		{
			//Get hash code for the Code field. 
			return obj.PedidoId.GetHashCode();
		}
	}
}
