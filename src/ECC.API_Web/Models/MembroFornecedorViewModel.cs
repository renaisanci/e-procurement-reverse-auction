using System;

namespace ECC.API_Web.Models
{
    public class MembroFornecedorViewModel
    {
        public int MembroId { get; set; }
        public int FornecedorId { get; set; }    
        public DateTime DataCriado { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public string FormaPagtoString { get; set; }
 
        public bool FlgSomenteAvista{ get; set; }
        public string Cnpj { get; set; }        
        public bool Ativo { get; set; }
    }


    public class MembroFornecedoresAdm
    {
        public int MembroId { get; set; }
        public int[] FornecedoresAdd { get; set; }
        public int[] FornecedoresDel { get; set; }
    }
}