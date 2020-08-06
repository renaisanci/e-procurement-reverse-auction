using System;
using ECC.Entidades;


namespace ECC.EntidadeEmail
{
    public class Emails : EntidadeBase
    {

        public DateTime? DataEnvio { get; set; }//robo
        public DateTime? DataConfirmacaoEnvio { get; set; }//robo
        public DateTime? DataInicioProcesso { get; set; }//robo
        public string AssuntoEmail { get; set; }
        public string EmailDestinatario { get; set; }
        public string CorpoEmail { get; set; }
        public Status Status { get; set; }
        public Origem Origem { get; set; }

        public string Erro { get; set; }//robo
    }
}
