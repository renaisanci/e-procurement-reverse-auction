using System.Collections.Generic;
using ECC.Entidades;

namespace ECC.EntidadeEndereco
{
    public class Cidade : EntidadeBase
    {       
        public int RegiaoId { get; set; }
        public int EstadoId { get; set; }
        public string DescCidade { get; set; }
        public string Cep { get; set; }
        public int? CodigoIBGE { get; set; }
        
        public virtual Estado Estado { get; set; }
        public virtual Regiao Regiao { get; set; }

        public virtual ICollection<Bairro> Bairros { get; set; }
    }
}
