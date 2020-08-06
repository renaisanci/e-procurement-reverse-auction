using System;
using System.Collections.Generic;

namespace ECC.Entidades.EntidadeComum
{
    public class CotacaoUsuarios

    {
        public long CotacaoId { get; set; }
        public string TokenUsuario { get; set; }
        public object CotacaoGrupo { get; set; }
        public DateTime DataFechamentoCotacao { get; set; }
    }
}
