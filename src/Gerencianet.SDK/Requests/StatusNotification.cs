using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    [JsonObject]
    public class StatusNotification
    {
        [JsonProperty(PropertyName = "current")]
        public string StatusAtual { get; set; }


        [JsonProperty(PropertyName = "previous")]
        public string StatusAnterior { get; set; }

    }
}
