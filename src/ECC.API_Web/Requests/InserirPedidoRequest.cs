using System.Collections.Generic;
using ECC.API_Web.Models;

namespace ECC.API_Web.Requests
{
    public class InserirPedidoRequest
    {
        public int EnderecoId { get; set; }

        public string DtCotacao { get; set; }

        public List<ItemPedidoViewModel> Items { get; set; }

        public List<RemFornPedCotViewModel> RemFornPedCot { get; set; }
    }
}