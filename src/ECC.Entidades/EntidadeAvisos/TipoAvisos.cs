using System.Collections.Generic;
using ECC.EntidadeMenu;
using ECC.Entidades;

namespace ECC.EntidadeAvisos
{
    public class TipoAvisos : EntidadeBase
    {
        public TipoAvisos() {
            this.Notificacoes = new List<Notificacao>();
        }

        public int Ordem { get; set; }

        public int ModuloId { get; set; }
        public string Descricao { get; set; }
        public string Nome { get; set; }
        public string Url { get; set; }
        public string FontIcon { get; set; }
        public string Cor { get; set; }
        public string Feature1 { get; set; }
        public string Feature2 { get; set; }

        public virtual Modulo Modulo { get; set; }

        public virtual ICollection<Notificacao> Notificacoes { get; set; }
    }

}
