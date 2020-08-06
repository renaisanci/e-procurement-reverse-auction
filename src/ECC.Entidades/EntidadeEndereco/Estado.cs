using System.Collections.Generic;
using ECC.Entidades;

namespace ECC.EntidadeEndereco
{
    public class Estado : EntidadeBase
    {
        public string DescEstado { get; set; }
        public string Uf { get; set; }
        public virtual ICollection<Cidade> Cidades { get; set; }
        public virtual ICollection<Regiao> Regioes { get; set; }      
    }
}
