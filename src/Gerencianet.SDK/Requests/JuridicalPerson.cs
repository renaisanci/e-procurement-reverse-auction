using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Dados de pessoa jurídica
    /// </summary>
    [JsonObject]
    public class JuridicalPerson
    {
        /// <summary>
        /// Nome da razão social.
        /// </summary>
        [JsonProperty(PropertyName = "corporate_name")]
        public string CorporateName { get; set; }

        /// <summary>
        /// CNPJ da empresa.
        /// </summary>
        [JsonProperty(PropertyName = "cnpj")]
        public string CNPJ { get; set; }
    }
}
