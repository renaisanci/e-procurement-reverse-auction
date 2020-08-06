using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Objeto de pagamento
    /// </summary>
    [JsonObject]
    public class Payment
    {
        /// <summary>
        /// Forma de pagamento através de "boleto bancário" 
        /// </summary>
        [JsonProperty(PropertyName = "banking_billet", NullValueHandling = NullValueHandling.Ignore)]
        public BankingBillet BankingBillet { get; set; }

        [JsonProperty(PropertyName = "credit_card", NullValueHandling = NullValueHandling.Ignore)]
        public CreditCard CreditCard { get; set; }
    }
}
