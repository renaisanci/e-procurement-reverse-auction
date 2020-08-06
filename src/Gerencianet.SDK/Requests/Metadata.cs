using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Define dados específicos da assinatura.
    /// </summary>
    [JsonObject]
    public class Metadata
    {
        /// <summary>
        /// Permite associar uma transação Gerencianet a uma ID específica de seu sistema ou aplicação, 
        /// permitindo identificá-la caso você possua uma identificação específica e queira mantê-la.
        /// </summary>
        [JsonProperty(PropertyName = "custom_id", Required = Required.Always)]
        public string CustomId { get; set; }

        /// <summary>
        /// Endereço de sua URL válida que receberá as notificações de mudanças de status das transaçõe
        /// </summary>
        [JsonProperty(PropertyName = "notification_url", Required = Required.Always)]
        public string NotificationURL { get; set; }
    }
}
