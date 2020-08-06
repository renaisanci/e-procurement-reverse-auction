using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECC.API_Web.Models
{
    public class FranquiaViewModel
    {
        public int FranquiaId { get; set; }
        public int PessoaId { get; set; }
        public string Responsavel { get; set; }
        public string Descricao { get; set; }
        public bool DataCotacao { get; set; }
        public string Cnpj { get; set; }
        public string NomeFantasia { get; set; }
        public string RazaoSocial { get; set; }
        public DateTime DtFundacao { get; set; }
        public string Email { get; set; }
        public string InscEstadual { get; set; }
        public string DddTelComl { get; set; }
        public string TelefoneComl { get; set; }
        public string DddCel { get; set; }
        public string Celular { get; set; }
        public string Contato { get; set; }
        public EnderecoViewModel Endereco { get; set; }
        public string Completo { get; set; }
        public bool Ativo { get; set; }
    }
}