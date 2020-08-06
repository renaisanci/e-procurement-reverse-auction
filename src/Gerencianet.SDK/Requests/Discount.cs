using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Define dados de desconto sobre a cobrança.
    /// </summary>
    [JsonObject]
    public class Discount
    {
        /// <summary>
        /// Tipo do desconto (String). Valores permitidos:
        /// currency: o desconto será informado em centavos;
        /// percentage: o desconto será informado em porcentagem.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        ///  Valor do desconto (Integer). Se o tipo do desconto for currency, o valor desta tag deverá ser informada pelo integrador em centavos (ou seja, 500 equivale a R$ 5,00). Caso o tipo do desconto seja percentage, o valor deverá ser multiplicado por 100 (ou seja, 1500 equivale a 15%). Exemplos:
        ///  1) currency // deve ser informado em centavos, ou seja, se o desconto será de R$ 5,99, o integrador deve informar 599;
        ///  2) percentage // deve ser informado em centavos, ou seja, se o desconto é de 15%, o integrador deve informar 1500
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }
    }
}
