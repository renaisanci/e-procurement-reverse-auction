using ECC.EntidadeProduto;
using ECC.Entidades;

namespace ECC.EntidadeRecebimento
{
    public class PlanoSegmento : EntidadeBase
    {
        public virtual Segmento Segmento { get; set; }

        public int SegmentoId { get; set; }

        public PlanoMensalidade PlanoMensalidade { get; set; }

        public int PlanoMensalidadeId { get; set; }
    }
}
