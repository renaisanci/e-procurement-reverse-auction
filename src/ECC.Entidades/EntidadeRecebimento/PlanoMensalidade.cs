using ECC.Entidades;

namespace ECC.EntidadeRecebimento
{
    public class PlanoMensalidade : EntidadeBase
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public int QtdMeses { get; set; }
        public decimal Valor { get; set; }
        public bool Ativo { get; set; }
    }
}
