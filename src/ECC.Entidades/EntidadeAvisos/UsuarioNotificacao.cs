using ECC.Entidades;
using ECC.EntidadeUsuario;

namespace ECC.EntidadeAvisos
{
    public class UsuarioNotificacao : EntidadeBase
    {
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public int NotificacaoId { get; set; }

        public virtual Notificacao Notificacao { get; set; }
    }
}
