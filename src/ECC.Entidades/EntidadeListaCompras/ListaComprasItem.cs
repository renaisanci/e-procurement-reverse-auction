using System;
using ECC.EntidadePessoa;
using ECC.Entidades;
using System.Collections.Generic;
using ECC.EntidadeEntrega;
using ECC.EntidadeFormaPagto;
using ECC.Entidades.EntidadeProduto;
using ECC.EntidadeUsuario;
using ECC.EntidadeListaCompras;

namespace ECC.EntidadeListaCompras
{
	public class ListaComprasItem : EntidadeBase
	{

        public int ListaComprasId { get; set; }
        public virtual ListaCompras ListaCompras { get; set; }

        public int ProdutoId { get; set; }
		public virtual Produto Produto { get; set; }    
		public int Quantidade { get; set; }
        public bool FlgOutraMarca { get; set; }
        public int QtdForne { get; set; }
        
    }

 
}
