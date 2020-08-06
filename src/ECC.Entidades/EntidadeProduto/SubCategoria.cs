using ECC.Entidades;
using System.Collections.Generic;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeProduto
{
    public class SubCategoria : EntidadeBase
    {
        public SubCategoria()
        {
            Produtos = new List<Produto>();
        }

        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
        public string DescSubCategoria { get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
