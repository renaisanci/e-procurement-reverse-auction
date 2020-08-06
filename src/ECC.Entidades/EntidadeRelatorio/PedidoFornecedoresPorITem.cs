using System;
using System.Collections.Generic;
using ECC.EntidadeEndereco;
using ECC.EntidadePessoa;
using ECC.Entidades;
using ECC.EntidadeStatus;
using ECC.EntidadePedido;

namespace ECC.EntidadeRelatorio
{
    public class PedidoFornecedoresPorITem : EntidadeBase
    {
        public PedidoFornecedoresPorITem()
        {
            ItemPedidosFornecedor = new List<ItemPedidosFornecedores>();
        }

        
        public DateTime DtPedido { get; set; }
        public bool FlgCotado { get; set; }
        public int StatusSistemaId { get; set; }

        public virtual StatusSistema StatusSistema { get; set; }
        public int MembroId { get; set; }
        public virtual Membro Membro { get; set; }

        public int EnderecoId { get; set; }
        public virtual Endereco Endereco { get; set; }

        public virtual ICollection<ItemPedidosFornecedores> ItemPedidosFornecedor { get; set; }

    }

    public class ItemPedidosFornecedores{
        public virtual ItemPedido ItemPedidos {get;set;}

        public List<FornecedorLance> ListFornecedorLance { get; set; }
    }

    public class FornecedorLance {

        public virtual Fornecedor ItemFornecedorLance { get; set; }

        public decimal valorLance { get; set; }
        public string obs { get; set; }
        public int qtd { get; set; }
        
    }

}
