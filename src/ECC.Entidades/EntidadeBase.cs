using ECC.EntidadeUsuario;
using System;

namespace ECC.Entidades
{
    public abstract class EntidadeBase : IEntidadeBase
    {
        #region IEntidadeBase

        public int Id { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }
        public int? UsuarioCriacaoId { get; set; }
        public DateTime DtCriacao { get; set; }
        public virtual Usuario UsuarioAlteracao { get; set; }
        public int? UsuarioAlteracaoId { get; set; }
        public DateTime? DtAlteracao { get; set; }
        public bool Ativo { get; set; }

        #endregion IEntidadeBase
    }
}
