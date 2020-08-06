using System;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Responses
{
    /// <summary>
    /// Contém as informações de retorno da transação.
    /// </summary>
    [JsonObject]
    public class TransactionResponse
    {
        /// <summary>
        /// número da ID referente à transação gerada
        /// </summary>
        [JsonProperty(PropertyName = "charge_id", Required = Required.Always)]
        public int ChargerId { get; set; }

        /// <summary>
        /// Status da cobrança
        /// </summary>
        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public string Status { get; set; }

        /// <summary>
        /// valor total da transação (em centavos, sendo 8900 = R$89,00)
        /// </summary>
        [JsonProperty(PropertyName = "total", Required = Required.Always)]
        public int Total { get; set; }

        /// <summary>
        /// identificador próprio opcional
        /// </summary>
        [JsonProperty(PropertyName = "custom_id")]
        public int CustomId { get; set; }

        /// <summary>
        /// data e hora da criação da transação
        /// </summary>
        [JsonProperty(PropertyName = "created_at", Required = Required.Always)]
        public DateTime CreatedAt { get; set; }
    }
}
