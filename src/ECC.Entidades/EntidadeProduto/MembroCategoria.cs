using ECC.EntidadePessoa;
using ECC.Entidades;

namespace ECC.EntidadeProduto
{
    public class MembroCategoria : EntidadeBase
    {
        public int CategoriaId { get; set; }
        public int MembroId { get; set; }
        public virtual Membro Membro { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}
