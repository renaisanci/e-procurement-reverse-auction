using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Contém as informações dos itens.
    /// </summary>
    [JsonObject]
    public class Item
    {
        /// <summary>
        /// Nome da inscrição associada ao plano de assinatura.
        /// Mínimo de 1 caractere e máximo de 255 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Valor da inscrição associada ao plano de assinatura.
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }

        ///// <summary>
        ///// Quantidade.
        ///// </summary>
        //[JsonProperty(PropertyName = "amount", NullValueHandling = NullValueHandling.Ignore)]
        //public int Amount { get; set; }
    }
}
