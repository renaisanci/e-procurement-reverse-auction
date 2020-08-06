using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECC.API_Web.Models
{
    public class CalendarioFeriadoViewModel
    {
        public string NomeCidade { get; set; }
        public string Estado { get; set; }
        public DateTime DtEvento { get; set; }
        public string NomeFeriado { get; set; }
        public int TipoFeriado { get; set; }
    }
}