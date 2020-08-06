using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECC.API_Web.Models
{
    public class TrocaItemFornecedorResponseViewModel
    {

        public string DescFormaPagto { get; set; }
        public decimal? Desconto { get; set; }
        public int QtdParcelas { get; set; }
        public int FormaPagtoId { get; set; }
        public decimal ValorMinParcela { get; set; }
        public decimal ValorMinParcelaPedido { get; set; }
        public bool PodeSelecionar { get; set; }

    }
}