using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class HorasEntregaMembroViewModel
    {
        public int PeriodoId { get; set; }

        public List<PeriodoEntregaViewModel> Periodo { get; set; }

        public int EnderecoId { get; set; }

        public List<EnderecoViewModel> Endereco { get; set; }

        public string DescHorarioEntrega { get; set; }

    }
}