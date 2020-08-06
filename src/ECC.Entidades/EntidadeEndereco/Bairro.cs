using ECC.Entidades;

namespace ECC.EntidadeEndereco
{
    public class Bairro : EntidadeBase
    {
        public string DescBairro { get; set; }
        public int CidadeId { get; set; }
        public virtual Cidade Cidade { get; set; }
    }
}
