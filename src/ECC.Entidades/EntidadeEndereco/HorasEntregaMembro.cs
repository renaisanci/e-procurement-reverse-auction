using ECC.Entidades;

namespace ECC.EntidadeEndereco
{
    public class HorasEntregaMembro : EntidadeBase
    {
        public int PeriodoId { get; set; }
        public virtual PeriodoEntrega Periodo { get; set; }
        public int EnderecoId { get; set; }
        public virtual Endereco Endereco { get; set; }
        public string DescHorarioEntrega { get; set; }
    }
}
