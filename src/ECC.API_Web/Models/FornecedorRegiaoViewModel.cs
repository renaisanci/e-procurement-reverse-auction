namespace ECC.API_Web.Models
{
    public class FornecedorRegiaoViewModel
    {
        public int Id { get; set; }
        public int FornecedorId { get; set; }
        public int EstadoId { get; set; }
        public string DescEstado { get; set; }
        public int CidadeId { get; set; }
        public string DescCidade { get; set; }
        public int RegiaoId { get; set; }
        public string DescRegiao { get; set; }
        public int Prazo { get; set; }
        public decimal VlPedMinRegiao { get; set; }
        public decimal? TaxaEntrega { get; set; }
        public bool Cif { get; set; }
    }

    public class DadosPrazoRegiao
    {
        public int fornecedorId { get; set; }
        public int regiaoId { get; set; }
        public int cidadeId { get; set; }
        public int prazo { get; set; }
    }

}