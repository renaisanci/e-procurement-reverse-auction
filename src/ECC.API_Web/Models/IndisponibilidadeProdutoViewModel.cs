using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECC.API_Web.Models
{
    public class IndisponibilidadeProdutoViewModel
    {

        public int DisponibilidadeId { get; set; }
        public int FornecedorId { get; set; }
        public int ProdutoId { get; set; }
        public Boolean IndisponivelPermanente {get; set;}
        public DateTime InicioIndisponibilidade { get; set; }
        public DateTime FimIndisponibilidade { get; set; }

    }
}