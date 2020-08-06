using System;
using System.Collections.Generic;
using ECC.EntidadeEndereco;
using ECC.EntidadePessoa;
using ECC.Entidades;
using ECC.EntidadeStatus;
using ECC.EntidadeRecebimento;

namespace ECC.EntidadePedido
{
    public class Pedido : EntidadeBase
    {
        public Pedido()
        {
            ItemPedidos = new List<ItemPedido>();
            Comissoes = new List<Comissao>();
        }

        public DateTime DtPedido { get; set; }
        public bool FlgCotado { get; set; }
        public int StatusSistemaId { get; set; }
		
		public virtual StatusSistema StatusSistema { get; set; }
		public int MembroId { get; set; }
        public virtual Membro Membro { get; set; }

        public int EnderecoId { get; set; }

        public DateTime DtCotacao { get; set; }

        public virtual Endereco Endereco { get; set; }
		
		public virtual ICollection<ItemPedido> ItemPedidos { get; set; }
        
        public virtual ICollection<Comissao> Comissoes { get; set; }
    }
}
