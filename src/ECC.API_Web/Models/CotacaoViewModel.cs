using System;

namespace ECC.API_Web.Models
{
    public class CotacaoViewModel
    {


        public int CotacaoId { get; set; }
        public DateTime DtCotacao { get; set; }

        public DateTime DtFechamento{ get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public int QtdItem { get; set; }        
        public int OrdemStatus { get; set; }
   


    }
}