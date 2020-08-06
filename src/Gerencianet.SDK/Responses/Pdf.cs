using System;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Responses
{
    /// <summary>
    /// Contém as informações de retorno do pdf.
    /// </summary>
    [JsonObject]
    public class Pdf
    {
        /// <summary>
        /// link do boleto do pdf
        /// </summary>
        [JsonProperty(PropertyName = "charge")]
        public string Charge { get; set; }

       
    }
}
