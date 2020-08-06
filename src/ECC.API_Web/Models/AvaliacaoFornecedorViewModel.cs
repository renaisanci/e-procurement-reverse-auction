namespace ECC.API_Web.Models
{
    public class AvaliacaoFornecedorViewModel
    {
        public int PedidoId { get; set; }

        public int FornecedorId { get; set; }

        public int QualidadeProdutos { get; set; }

        public int TempoEntrega { get; set; }

        public int Atendimento { get; set; }

        public string ObsQualidade { get; set; }

        public string ObsEntrega{ get; set; }

        public string ObsAtendimento { get; set; }

        public FornecedorViewModel Fornecedor { get; set; }

        public EntregaViewModel Entrega { get; set; }

    }


}