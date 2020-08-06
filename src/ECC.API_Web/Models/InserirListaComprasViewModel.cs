using System.Collections.Generic;
 

namespace ECC.API_Web.Models
{
    public class InserirListaComprasViewModel
    {
    

        public string NomeLista { get; set; }

        public List<ItemPedidoViewModel> ListaCompras { get; set; }

        public List<ListaComprasRemFornViewModel> RemFornPedCot { get; set; }
    }
}