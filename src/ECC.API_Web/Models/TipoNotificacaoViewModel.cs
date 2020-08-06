using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class TipoNotificacaoViewModel
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public List<NotificacaoViewModel> Notificacoes { get; set; }
    }
}