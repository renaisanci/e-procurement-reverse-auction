using System;

namespace ECC.API_Web.Models
{
    public class ComissaoViewModel
    {
        public int Id { get; set; }

        public decimal Valor { get; set; }

        public int PedidoId { get; set; }

        public decimal PedidoTotal { get; set; }

        public DateTime DtCriacao { get; set; }
    }
}