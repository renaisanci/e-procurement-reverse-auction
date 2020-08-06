using System;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Boleto Bancário.
    /// </summary>
    [JsonObject]
    public class BankingBillet
    {
        private string _expireAt;

        /// <summary>
        /// Dados pessoais do pagador.
        /// </summary>
        [JsonProperty(PropertyName = "customer")]
        public Customer Customer { get; set; }

        /// <summary>
        /// Data de vencimento do boleto.
        /// </summary>
        [JsonProperty(PropertyName = "expire_at")]
        public string ExpireAt { get; set; }
        //{
        //    get { return _expireAt; }

        //    set
        //    {
        //        var quebrado = value.Split('/');
        //        _expireAt = $"{quebrado[2]}-{quebrado[1]}-{quebrado[0]}";
        //    }
        //}




        /// <summary>
        /// Define dados de desconto sobre a cobrança.
        /// </summary>
        [JsonProperty(PropertyName = "discount",NullValueHandling = NullValueHandling.Ignore)]
        public Discount Discount { get; set; }

        /// <summary>
        /// Define dados de desconto sobre a cobrança.
        /// </summary>
        [JsonProperty(PropertyName = "configurations")]
        public ConfigGerenciaNet Configurations { get; set; }

        /// <summary>
        /// Permite incluir no boleto uma "observação", ou em outras palavras, uma mensagem para o cliente. 
        /// Essa mensagem poderá ser vista nos e-mails relacionados à cobrança, no boleto ou carnê.
        /// </summary>
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}
