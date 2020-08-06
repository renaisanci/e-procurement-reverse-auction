using ECC.EntidadeAvisos;

namespace ECC.API_Web.Models
{
    public class NotificacaoViewModel
    {
        public int Id { get; set; }
        
        public TipoAlerta Alerta { get; set; }

        public bool Ativo { get; set; }

        public bool Checked { get; set; }
    }
}