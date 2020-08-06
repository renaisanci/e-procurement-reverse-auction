namespace ECC.API_Web.Models
{
    public class FornecedorPrazoSemanalViewModel
    {
        public int Id { get; set; }
        public int FornecedorId { get; set; }
        public int CidadeId { get; set; }
        public string DescCidade { get; set; }
        public int DiaSemana { get; set; }
        public decimal VlPedMinRegiao { get; set; }
        public decimal? TaxaEntrega { get; set; }
        public bool Cif { get; set; }
    }
}