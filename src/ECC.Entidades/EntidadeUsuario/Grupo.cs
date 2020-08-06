using System.Collections.Generic;
using ECC.Entidades;

namespace ECC.EntidadeUsuario
{
    public class Grupo : EntidadeBase
    {
        public string DescGrupo{ get; set; }
        public virtual ICollection<PermissaoGrupo> PermissoesGrupos { get; set; }
    }
}
