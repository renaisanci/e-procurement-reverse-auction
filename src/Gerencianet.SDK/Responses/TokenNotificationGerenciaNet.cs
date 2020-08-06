using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Responses
{
    [JsonObject]
    public class TokenNotificationGerenciaNet
    {
        [JsonProperty(PropertyName = "notification")]
        public string Notification { get; set; }
    }
}