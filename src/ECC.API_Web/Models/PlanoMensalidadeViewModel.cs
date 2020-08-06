using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class PlanoMensalidadeViewModel
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public int QtdMeses { get; set; }

        public decimal Valor { get; set; }

        public bool Ativo { get; set; }
    }
}