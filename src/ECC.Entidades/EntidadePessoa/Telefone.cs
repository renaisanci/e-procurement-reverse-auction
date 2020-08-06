using ECC.Entidades;
using ECC.EntidadeUsuario;

namespace ECC.EntidadePessoa
{
    public class Telefone : EntidadeBase
    {
   
        public string DddTelComl { get; set; }
        public string TelefoneComl { get; set; }
        public string DddCel { get; set; }
        public string Celular { get; set; }
        public string Contato { get; set; }

        public int PessoaId { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public int? UsuarioTelId { get; set; }
        public virtual Usuario UsuarioTel  { get; set; }
    }
}
