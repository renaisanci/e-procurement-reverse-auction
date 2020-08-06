using System;
using ECC.Entidades;

namespace ECC.EntidadePedido
{
    public class CalendarioFeriado : EntidadeBase
    {

        public DateTime DtEvento { get; set; }
        public string NomeFeriado { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public int TipoFeriado { get; set; }


    }
}
