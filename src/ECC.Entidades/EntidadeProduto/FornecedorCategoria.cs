using ECC.EntidadePessoa;
using ECC.Entidades;

namespace ECC.EntidadeProduto
{
    public class FornecedorCategoria : EntidadeBase
    {
        public int CategoriaId { get; set; }
        public int FornecedorId { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}
