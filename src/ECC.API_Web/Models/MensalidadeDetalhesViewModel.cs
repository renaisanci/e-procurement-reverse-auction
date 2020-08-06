using ECC.Entidades.EntidadeRecebimento;

namespace ECC.API_Web.Models
{
    public class MensalidadeDetalhesViewModel
    {
        public string Descricao { get; set; }

        public decimal Valor { get; set; }

        public TipoMovimentacao Tipo { get; set; }

        public string TipoDescicao { get; set; }
    }
}