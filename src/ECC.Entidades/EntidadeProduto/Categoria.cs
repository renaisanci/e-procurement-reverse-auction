using System.Collections.Generic;
using ECC.Entidades;

namespace ECC.EntidadeProduto
{
    public class Categoria : EntidadeBase    
    {
        public Categoria()
        {           
            SubCategorias = new List<SubCategoria>();
        }

        public string DescCategoria { get; set; }

        public virtual ICollection<SubCategoria> SubCategorias { get; set; }
        public virtual ICollection<SegmentoCategoria> SegmentosCategoria { get; set; }
    }
}
