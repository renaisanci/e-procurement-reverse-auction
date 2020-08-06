using ECC.Entidades;

namespace ECC.EntidadeRecebimento
{
    public class ParametrosRecebimento : EntidadeBase
    {
        public ParametrosRecebimento()
        {

        }

        #region Membro

        /// <summary>
        /// Valor da mensalidade do membro
        /// </summary>
        public decimal MembroMensalidade { get; set; }

        /// <summary>
        /// Dia de fechamento da fatura do membro
        /// </summary>
        public int MembroDiaFechamento { get; set; }

        /// <summary>
        /// Total de compras partir deste valor,
        /// o membro ganha abatimento na mensalidade
        /// </summary>
        public decimal MembroCompraDesconto { get; set; }

        /// <summary>
        /// Valor que o membro ganha para usar em nosso sistema por indicaar de outros membros
        /// </summary>
        //public decimal MembroValorIndicacao { get; set; }

        #endregion

        #region Fornecedor

        /// <summary>
        /// Percentual de comissão que ganhamos de cada venda do fornecedor
        /// </summary>
        public decimal FornecedorComissao { get; set; }

        /// <summary>
        /// Dia de fechamento da fatura do fornecedor
        /// </summary>
        public int FornecedorDiaFechamento { get; set; }

        /// <summary>
        /// Dia de vencimento da fatura do fornecedor
        /// </summary>
        public int FornecedorDiaVencimento { get; set; }

        #endregion
    }
}
