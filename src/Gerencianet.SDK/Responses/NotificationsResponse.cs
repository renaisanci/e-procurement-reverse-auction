using System;
using Gerencianet.SDK.Requests;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Responses
{
    /// <summary>
    /// Contém as informações de retorno das notificações.
    /// </summary>
    [JsonObject]
    public class NotificationsResponse
    {
        /// <summary>
        /// id da notificação
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// tipo da notificação
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

       /// <summary>
        /// Id da Fatura
        /// </summary>
        [JsonProperty(PropertyName = "custom_id")]
        public int CustomId { get; set; }

        /// <summary>
        /// Status da Transação
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public StatusNotification Status { get; set; }
        
        /// <summary>
        /// Status da Transação
        /// </summary>
        [JsonProperty(PropertyName = "identifiers")]
        public Identifiers Identifiers { get; set; }

    }
}
