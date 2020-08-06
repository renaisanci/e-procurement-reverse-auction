using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Endereço de entrega.
    /// </summary>
    [JsonObject]
    public class Address
    {
        /// <summary>
        /// Nome da Rua.
        /// </summary>
        [JsonProperty(PropertyName = "street")]
        public object Street { get; set; }

        /// <summary>
        /// Número.
        /// </summary>
        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        /// <summary>
        /// Bairro.
        /// </summary>
        [JsonProperty(PropertyName = "neighborhood")]
        public string Neighborhood { get; set; }

        /// <summary>
        /// CEP.
        /// </summary>
        [JsonProperty(PropertyName = "zipcode")]
        public string ZipCode { get; set; }

        /// <summary>
        /// Cidade.
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// Complemento.
        /// </summary>
        [JsonProperty(PropertyName = "complement")]
        public string Complement { get; set; }

        /// <summary>
        /// Estado
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
    }
}
