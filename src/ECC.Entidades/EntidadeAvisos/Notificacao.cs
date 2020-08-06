using System.Collections.Generic;
using ECC.Entidades;

namespace ECC.EntidadeAvisos
{
    public class Notificacao : EntidadeBase
    {
        public int TipoAvisosId { get; set; }

        public TipoAlerta TipoAlerta { get; set; }

        public virtual TipoAvisos TipoAvisos { get; set; }

        public virtual ICollection<UsuarioNotificacao> UsuariosNotificacoes { get; set; }
    }
}
