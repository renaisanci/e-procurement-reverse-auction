using System;
using ECC.EntidadeUsuario;

namespace ECC.Entidades
{
    public interface IEntidadeBase
    {
        int Id { get; set; }
        Usuario UsuarioCriacao { get; set; }
        int? UsuarioCriacaoId { get; set; }
        DateTime DtCriacao { get; set; }
        Usuario UsuarioAlteracao { get; set; }
        int? UsuarioAlteracaoId { get; set; }
        DateTime? DtAlteracao { get; set; }
        bool Ativo { get; set; }

    }
}
