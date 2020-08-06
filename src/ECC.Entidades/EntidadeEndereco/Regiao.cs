using System.Collections.Generic;
using ECC.Entidades;

namespace ECC.EntidadeEndereco
{
    /// <summary>
    /// Cadastro de Regiões
    /// A região é por estado. 
    /// Ex: são paulo tem suas regiões rio de janeiro tem sua regiões 
    /// </summary>
    public class Regiao : EntidadeBase
    {
        public string DescRegiao { get; set; }
        public int EstadoId { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual ICollection<Cidade> Cidades { get; set; }
    }
}
