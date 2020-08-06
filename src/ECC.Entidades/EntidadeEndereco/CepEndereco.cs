using ECC.Entidades;

namespace ECC.EntidadeEndereco
{
    public class CepEndereco : EntidadeBase
    {
        public int EstadoId { get; set; }
        public int CidadeId { get; set; }
        public int BairroId { get; set; }
        public int LogradouroId { get; set; }
        public string Cep { get; set; }
        public string Complemento { get; set; }
        public string DescLogradouro { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual Cidade Cidade { get; set; }
        public virtual Bairro Bairro { get; set; }
        public virtual Logradouro Logradouro { get; set; }
    }
}
