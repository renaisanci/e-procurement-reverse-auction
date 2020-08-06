using System;
using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class PedidoViewModel
    {
        public int PedidoId { get; set; }
        public DateTime DtPedido { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public int OrdemStatus { get; set; }
        public int QtdItem { get; set; }
        public decimal? ValorTotal { get; set; }
        public int PrazoEntrega { get; set; }
        public List<FornecedorPrazoSemanalViewModel> FornecedorPrazoSemanal { get; set; }
        public int CotacaoId { get; set; }
        public bool FlgCotado { get; set; }
        public DateTime DtCotacao { get; set; }
        public Boolean PedidoPromocional { get; set; }
        public EnderecoViewModel Endereco { get; set; }
        public List<ItemPedidoViewModel> Itens { get; set; }
        public CotacaoViewModel Cotacao { get; set; }
        public MembroViewModel Membro { get; set; }
        public decimal? ValorPedidoMinimo { get; set; }
        public string DescMotivoCancelamento { get; set; }
        public object ListaHistStatusPedido { get; set; }
    }



}