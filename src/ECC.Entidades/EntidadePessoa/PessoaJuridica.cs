using ECC.Entidades;
using System;
using System.Collections.Generic;


namespace ECC.EntidadePessoa
{
    /// <summary>
    /// Dados de Pessoa Juridica
    /// </summary>
    public class PessoaJuridica : EntidadeBase
    {
        public PessoaJuridica()
        {
            Pessoas = new List<Pessoa>();
        }

        public int PessoaId { get; set; }
        public string NomeFantasia { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string InscEstadual { get; set; }
        public DateTime? DtFundacao { get; set; }
        public string Email { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual ICollection<Pessoa> Pessoas { get; set; }    
    }
}
