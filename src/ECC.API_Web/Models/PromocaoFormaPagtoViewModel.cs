using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECC.API_Web.Models
{
    public class PromocaoFormaPagtoViewModel
    {
        public int ProdutoPromocionalId { get; set; }
        public int FormaPagtoId { get; set; }
        public string DescFormaPagamento { get; set; }
    }
}