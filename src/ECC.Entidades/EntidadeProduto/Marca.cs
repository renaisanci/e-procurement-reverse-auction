using System.Collections.Generic;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeProduto
{
    public class Marca : EntidadeBase
    {
        public string DescMarca{ get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
