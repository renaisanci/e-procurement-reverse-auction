using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECC.API_Web.Models
{
    public class TrocaItemFornecedorViewModel
    {

        public int ItemId { get; set; }
        public int FornecedorId { get; set; }
        public decimal ValorItemFornecedor { get; set; }
        public string Observacao { get; set; }

        public int FornecedorPagtoIdChosen { get; set; }


    }
}