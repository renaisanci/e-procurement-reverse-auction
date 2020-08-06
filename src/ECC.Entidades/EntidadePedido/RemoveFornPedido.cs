using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECC.EntidadePessoa;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadePedido
{
   public  class RemoveFornPedido : EntidadeBase
    {

        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }
        public int ItemPedidoId { get; set; }
        public virtual ItemPedido ItemPedido { get; set; }
        public int FonecedorId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

    }
}
