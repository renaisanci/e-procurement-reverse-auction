using ECC.Entidades;
using System.Collections.Generic;

namespace ECC.EntidadeListaCompras
{
   public class ListaCompras : EntidadeBase
    {


        public ListaCompras()
        {
            ListaComprasItens = new List<ListaComprasItem>();
            ListaComprasRemoveFornecedores = new List<ListaComprasRemoveForn>();
        }


        public string NomeLista { get; set; }
        public virtual ICollection<ListaComprasItem> ListaComprasItens { get; set; }
        public virtual ICollection<ListaComprasRemoveForn> ListaComprasRemoveFornecedores { get; set; }
    }
}
