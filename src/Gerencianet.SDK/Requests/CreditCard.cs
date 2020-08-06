using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Dados de pessoa jurídica
    /// </summary>
    [JsonObject]
    public class CreditCard
    {
        /// <summary>
        /// Dados pessoais do pagador.
        /// </summary>
        [JsonProperty(PropertyName = "customer")]
        public Customer Customer { get; set; }

        /// <summary>
        /// Define dados de desconto sobre a cobrança.
        /// </summary>
        //[JsonProperty(PropertyName = "discount")]
        //public Discount Discount { get; set; }

        /// <summary>
        /// Endereço de cobrança
        /// </summary>
        [JsonProperty(PropertyName = "billing_address")]
        public Address BillingAddress { get; set; }

        /// <summary>
        /// Token único de pagamento obtido na primeira etapa da geração da transação.
        /// </summary>
        [JsonProperty(PropertyName = "payment_token")]
        public string PaymentToken { get; set; }
    }
}
