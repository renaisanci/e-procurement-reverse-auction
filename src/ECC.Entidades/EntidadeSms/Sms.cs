using System;
using ECC.Entidades;

namespace ECC.EntidadeSms
{
    public class Sms : EntidadeBase
    {       
        public string Numero { get; set; }
        public string Mensagem { get; set; }
        public DateTime? DataEnvio { get; set; }//robo
        public DateTime? DataConfirmacaoEnvio { get; set; }//robo
        public StatusSms Status { get; set; }
        public string Erro { get; set; }//robo
        public DateTime? DataInicioProcesso { get; set; }//robo
        public string IdMessageSmsGateway { get; set; }//robo
        public TipoOrigemSms? OrigemSms { get; set; }
        public int? IdEntidadeOrigem { get; set; }//robo
    }
}
