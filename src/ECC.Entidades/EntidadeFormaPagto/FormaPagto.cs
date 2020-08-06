using ECC.Entidades;

namespace ECC.EntidadeFormaPagto
{
    public class FormaPagto : EntidadeBase
    {
        public string DescFormaPagto { get; set; }

        public bool Avista { get; set; }

        public int QtdParcelas { get; set; }
    }
}
