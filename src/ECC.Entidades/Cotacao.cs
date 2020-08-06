
using System;
using System.Collections.Generic;
using ECC.Entidades.EntidadeStatus;

namespace ECC.Entidades.EntidadeCotacao
{
    public class Cotacao : EntidadeBase 
    {


        public Cotacao()
        {
            CotacaoPedidos = new List<CotacaoPedidos>();
        }
        public int StatusSistemaId { get; set; }
        public virtual StatusSistema StatusSistema { get; set; }
        public virtual ICollection<CotacaoPedidos> CotacaoPedidos { get; set; }

    }

	public class CotacaoComparer : IEqualityComparer<Cotacao>
	{
		public bool Equals(Cotacao x, Cotacao y)
		{
			//Check whether the objects are the same object. 
			if (Object.ReferenceEquals(x, y)) return true;

			//Check whether the products' properties are equal. 
			return x != null && y != null && x.Id.Equals(y.Id);
		}

		public int GetHashCode(Cotacao obj)
		{
			//Get hash code for the Code field. 
			return obj.Id.GetHashCode();
		}
	}
}
