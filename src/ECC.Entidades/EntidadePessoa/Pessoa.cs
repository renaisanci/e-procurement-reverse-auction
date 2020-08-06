using ECC.Entidades;
using System.Collections.Generic;
using ECC.EntidadeEndereco;
using ECC.EntidadeUsuario;


namespace ECC.EntidadePessoa
{
    public class Pessoa : EntidadeBase
    {
        public Pessoa()
        {
            Telefones = new List<Telefone>();
            Usuarios = new List<Usuario>();
            Enderecos = new List<Endereco>();
        }

        public TipoPessoa TipoPessoa { get; set; }
        public virtual ICollection<Endereco> Enderecos { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
        public virtual ICollection<Telefone> Telefones { get; set; }
        public int? PessoaJuridicaId { get; set; }
        public virtual PessoaJuridica PessoaJuridica { get; set; }


        public int? PessoaFisicaId { get; set; }
        public virtual PessoaFisica PessoaFisica { get; set; }
    }
}
