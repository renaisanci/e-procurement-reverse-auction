using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Contém as informações de envio.
    /// </summary>
    [JsonObject]
    public class Shipping
    {
        /// <summary>
        /// Rótulo do frete. 
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Valor do frete, em centavos (1990 equivale a R$19,90).
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }

        /// <summary>
        /// Código "Identificador da Conta", único por conta.
        /// </summary>
        [JsonProperty(PropertyName = "payee_code")]
        public string PayeeCode { get; set; }
    }
}
