using System;
using ECC.EntidadeRecebimento;

namespace ECC.API_Web.Models
{
    public class MensalidadeViewModel
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public DateTime DtVencimento { get; set; }

        public StatusMensalidade Status { get; set; }

        public int TipoPagamentoId { get; set; }

        public string Tipo { get; set; }

        public string TipoPlano { get; set; }

        public bool HabilitarPagar { get; set; }

        public String UrlBoleto { get; set; }

        public decimal Total { get; set; }

        public bool Ativo { get; set; }

    }
}