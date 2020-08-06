using ECC.Entidades;
using System;


namespace ECC.EntidadeUsuario
{
    public class RecuperaSenha: EntidadeBase
    {
        public Guid Chave { get; set; }
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public DateTime DtExpira { get; set; }
    }
}
