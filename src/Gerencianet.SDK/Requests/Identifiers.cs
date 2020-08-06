using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    [JsonObject]
    public class Identifiers
    {

        [JsonProperty(PropertyName = "charge_id")]
        public int ChargeId { get; set; }
    }
}
