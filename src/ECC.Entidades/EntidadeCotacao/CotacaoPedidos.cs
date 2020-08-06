
using System;
using System.Collections.Generic;
using ECC.EntidadePedido;
using ECC.Entidades;

namespace ECC.EntidadeCotacao
{
    public class CotacaoPedidos : EntidadeBase
    {

      
        public int CotacaoId { get; set; }
        public virtual Cotacao Cotacao { get; set; }
        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

       
    }

	public class CotacaoPedidosByCotacaoComparer : IEqualityComparer<CotacaoPedidos>
	{
		public bool Equals(CotacaoPedidos x, CotacaoPedidos y)
		{
			//Check whether the objects are the same object. 
			if (Object.ReferenceEquals(x, y)) return true;

			//Check whether the products' properties are equal. 
			return x != null && y != null && x.CotacaoId.Equals(y.CotacaoId);
		}

		public int GetHashCode(CotacaoPedidos obj)
		{
			//Get hash code for the Code field. 
			return obj.CotacaoId.GetHashCode();
		}
	}
}
