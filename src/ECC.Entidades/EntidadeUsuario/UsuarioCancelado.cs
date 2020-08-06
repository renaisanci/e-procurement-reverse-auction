
using ECC.Entidades;
using System;

namespace ECC.EntidadeUsuario
{
    public class UsuarioCancelado : EntidadeBase
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public bool Ativo { get; set; }
    }
}
