using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Contém as informações de criação da transação.
    /// </summary>
    [JsonObject]

    public class TransactionRequest
    {
        /// <summary>
        /// Item que está sendo vendido. Uma mesma transação pode possuir ilimitados itens.
        /// </summary>
        [JsonProperty(PropertyName = "items", Required = Required.Always)]
        public Item[] Items { get; set; }

        /// <summary>
        /// Determina o(s) valor(es) de frete(s) de uma transação. Uma mesma transação pode possuir ilimitados valores de frete.
        /// </summary>
        //[JsonProperty(PropertyName = "shippings", Required = Required.AllowNull)]
        //public Shipping[] Shipping { get; set; }

        /// <summary>
        /// Define dados específicos da transação.
        /// </summary>
        [JsonProperty(PropertyName = "metadata", Required = Required.AllowNull)]
        public Metadata Metadata { get; set; }
    }
}
