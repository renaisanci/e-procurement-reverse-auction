using System;
using ECC.Entidades;

namespace ECC.EntidadePessoa
{
   public  class Funcionario : EntidadeBase
    
    {

       public int CargoId { get; set; }

       public virtual Cargo Cargo { get; set; }

       public int PessoaId { get; set; }
       public virtual Pessoa Pessoa { get; set; }

       public DateTime? DtContratacao { get; set; }

    }
}
