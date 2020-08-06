using ECC.Entidades;
using ECC.EntidadeStatus;

namespace ECC.EntidadeCotacao
{
    public class HistStatusCotacao : EntidadeBase
    {

        public int CotacaoId { get; set; }
        public virtual Cotacao Cotacao { get; set; }
        public int StatusSistemaId { get; set; }
        public virtual StatusSistema StatusSistema { get; set; }
    }
}
