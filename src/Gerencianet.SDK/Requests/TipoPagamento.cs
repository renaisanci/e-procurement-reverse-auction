using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    public class TipoPagamento
    {
        [JsonProperty(PropertyName = "payment")]
        public Payment Payment { get; set; }
    }
}
