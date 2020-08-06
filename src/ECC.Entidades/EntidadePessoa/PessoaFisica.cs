using System;
using System.Collections.Generic;
using ECC.Entidades;

namespace ECC.EntidadePessoa
{
    public class PessoaFisica : EntidadeBase
    {

        public PessoaFisica()
        {
            Pessoas = new List<Pessoa>();
        }

        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Cro { get; set; }
        public string Rg { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string Email { get; set; }
        public Sexo Sexo { get; set; }

        public int PessoaId { get; set; }
        public virtual Pessoa Pessoa { get; set; }



        public virtual ICollection<Pessoa> Pessoas { get; set; }


    }

 
}
