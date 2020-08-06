using System;

namespace ECC.API_Web.Models
{
    public class SolicitacaoMembroFornecedorViewModel
    {
        public int MembroId { get; set; }

        public int FornecedorId { get; set; }

        public DateTime DataCriado { get; set; }

        public DateTime DataAlteracao { get; set; }

        public string NomeFantasia { get; set; }

        public string RazaoSocial { get; set; }

        public string Cnpj { get; set; }

        public string Observacao { get; set; }

        public string ObservacaoFormaPagto { get; set; }

        public string ObservacaoEntrega { get; set; }

        public string Descricao { get; set; }

        public string PalavrasChaves { get; set; }

         
        public bool Ativo { get; set; }
    }
}