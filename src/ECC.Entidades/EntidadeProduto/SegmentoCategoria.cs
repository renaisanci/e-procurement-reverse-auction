using ECC.Entidades;

namespace ECC.EntidadeProduto
{
    public class SegmentoCategoria : EntidadeBase
    {
        public int CategoriaId { get; set; }
        public int SegmentoId { get; set; }
        public virtual Segmento Segmento { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}
