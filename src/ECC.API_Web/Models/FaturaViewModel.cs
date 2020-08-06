using System;
using System.Collections.Generic;
using System.Linq;

namespace ECC.API_Web.Models
{
    public class FaturaViewModel
    {
        public FaturaViewModel()
        {
            Comissoes = new List<ComissaoViewModel>();
        }

        public int Id { get; set; }

        public DateTime DtVencimento { get; set; }
        public DateTime DtFechamento { get; set; }

        public decimal TotalComissoes { get { return this.Comissoes.Sum(x => x.Valor); } }

        public decimal TotalVendas { get { return this.Comissoes.Sum(x => x.PedidoTotal); } }

        public string Status { get; set; }

        public int StatusRecebimento { get; set; }

        public string Token { get; set; }

        public string UrlBoleto { get; set; }

        public List<ComissaoViewModel> Comissoes { get; set; }
    }
}