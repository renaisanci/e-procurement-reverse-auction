using System.Collections.Generic;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeProduto
{
    public class Fabricante : EntidadeBase
    {
        public string DescFabricante { get; set; }
        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
