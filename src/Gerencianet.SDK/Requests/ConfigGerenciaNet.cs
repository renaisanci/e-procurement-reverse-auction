using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Permite incluir no boleto multa e juros caso seja pago após o vencimento.
    /// </summary>
    [JsonObject]
    public class ConfigGerenciaNet
    {
        /// <summary>
        /// valor cobrado de multa após o vencimento. 
        /// Por exemplo: se você quiser 2%, você deve informar 200. Mínimo de 0 e máximo de 1000.
        /// Caso você possua configurações de multa ativada no Fortunus e queira gerar emissões na API sem multa, 
        /// utilize 0
        /// </summary>
        [JsonProperty(PropertyName = "fine")]
        public int Fine { get; set; }

        /// <summary>
        /// valor cobrado de juros por dia após a data de vencimento. 
        /// Por exemplo: se você quiser 0,033%, você deve informar 33. Mínimo de 0 e máximo de 330.
        /// Caso você possua configurações de multa ativada no Fortunus e queira gerar emissões na API sem juros, 
        /// utilize 0
        /// </summary>
        [JsonProperty(PropertyName = "interest")]
        public int Interest { get; set; }
    }
}
