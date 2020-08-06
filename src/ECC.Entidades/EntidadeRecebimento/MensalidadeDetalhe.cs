using ECC.Entidades;
using ECC.Entidades.EntidadeRecebimento;



namespace ECC.EntidadeRecebimento
{
   /// <summary>
   /// Classe que implementa os Detalhes da Mensalidade.
   /// </summary>
    public class MensalidadeDetalhe : EntidadeBase
    {
        /// <summary>
        /// Id da mensalidade
        /// </summary>
        public int MensalidadeId { get; set; }
        /// <summary>
        /// Descrição da mensalidade
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Valor de cobrança da mensalidade
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo da mensalidade pode ser "C"(Crédito) ou "D"(Débito).
        /// </summary>
        public TipoMovimentacao Tipo { get; set; }

        /// <summary>
        /// Entidade que representa a mensalidade, pode trazer as propriedades de mensalidade.
        /// </summary>
        public virtual Mensalidade Mensalidade { get; set; }
    }
}
