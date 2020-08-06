using System.Collections.Generic;
using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeProduto
{
    public class UnidadeMedida : EntidadeBase
    {
        public UnidadeMedida()
        {          
            Produtos = new List<Produto>();
        }

        public string DescUnidadeMedida { get; set; }

        public virtual ICollection<Produto> Produtos { get; set; }
    }
}
