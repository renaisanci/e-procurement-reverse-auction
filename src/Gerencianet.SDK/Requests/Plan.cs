using System.ComponentModel;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Assinaturas
    /// </summary>
    [JsonObject]
    public class Plan
    {
        /// <summary>
        /// Nome do plano de assinatura.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Periodicidade da cobrança.Determina o intervalo, em meses, que a cobrança da assinatura deve ser gerada.
        /// Informe 1 para assinatura mensal.
        /// 
        /// Exemplo 1: se interval = 1 e repeats = null, será gerada 1 (uma) cobrança por mês, ou seja, 
        /// a cobrança recorrente será mensal, de acordo com a primeira data de vencimento escolhida e gerada indefinidamente.
        /// 
        /// Exemplo 2: se interval = 6 e repeats = 2, será gerada 1 (uma) cobrança a cada 6 (seis) meses, 
        /// totalizando 2 (duas) cobranças em 12 meses (uma no 6º mês e outra no 12º mês).
        /// </summary>
        [DefaultValue(1)]
        [JsonProperty(PropertyName = "interval", Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Interval { get; set; }

        /// <summary>
        /// Quantas cobranças devem ser geradas.Determina o número de vezes que a cobrança deve ser gerada.
        /// Se nada for enviado, a cobrança é gerada por tempo indeterminado ou até que o plano seja cancelado.
        ///
        /// Exemplo 1: se interval = 1 e repeats = null, será gerada 1 (uma) cobrança por mês, ou seja, 
        /// a cobrança recorrente será mensal, de acordo com a primeira data de vencimento escolhida.
        ///
        /// Exemplo 2: se interval = 6 e repeats = 2, será gerada 1 (uma) cobrança a cada 6 (seis) meses, 
        /// totalizando 2 (duas) cobranças em 12 meses (uma no 6º mês e outra no 12º mês).
        /// </summary>
        [JsonProperty(PropertyName = "repeats", Required = Required.AllowNull)]
        public int? Repeats { get; set; }
    }
}
