using System;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Responses
{
    /// <summary>
    /// Contém as informações de retorno da transação.
    /// </summary>
    [JsonObject]
    public class BankingBilletResponse
    {
        /// <summary>
        /// código de barras do boleto
        /// </summary>
        [JsonProperty(PropertyName = "barcode")]
        public string BarCode { get; set; }

        /// <summary>
        /// link do boleto gerado
        /// </summary>
        [JsonProperty(PropertyName = "link")]
        public string Link { get; set; }


        /// <summary>
        /// Objeto Pdf
        /// </summary>
        [JsonProperty(PropertyName = "pdf")]
        public Pdf Pdf { get; set; }

        /// <summary>
        /// data de vencimento do boleto no seguinte 
        /// formato: 2018-12-30 (ou seja, equivale a 30/12/2018)
        /// </summary>
        [JsonProperty(PropertyName = "expire_at")]
        public DateTime ExpireAt { get; set; }

        /// <summary>
        /// número da ID referente à transação gerada
        /// </summary>
        [JsonProperty(PropertyName = "charge_id")]
        public int ChargeId { get; set; }

        /// <summary>
        /// forma de pagamento selecionada, aguardando a confirmação do pagamento ("waiting" equivale a "aguardando")
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        /// <summary>
        /// valor, em centavos. 
        /// Por exemplo: 8900 (equivale a R$ 89,00)
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        /// <summary>
        /// forma de pagamento associada à esta transação 
        /// ("banking_billet" equivale a "boleto bancário")
        /// </summary>
        [JsonProperty(PropertyName = "payment")]
        public string Payment { get; set; }
    }
}
