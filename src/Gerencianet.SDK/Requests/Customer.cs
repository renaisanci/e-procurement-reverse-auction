using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Gerencianet.SDK.Requests
{
    /// <summary>
    /// Dados de pessoa jurídica
    /// </summary>
    [JsonObject]
    public class Customer
    {
        private string _birth;

        /// <summary>
        /// Nome do cliente.
        /// Mínimo de 1 caractere e máximo de 255.
        /// </summary>
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// CPF válido do cliente (sem pontos, vírgulas ou hífen).
        /// Tamanho: 11 caracteres.
        /// </summary>
        [JsonProperty(PropertyName = "cpf", NullValueHandling = NullValueHandling.Ignore)]
        public string CPF { get; set; }

        /// <summary>
        /// Email do cliente.
        /// Máximo de 255 caracteres
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        [JsonProperty(PropertyName = "phone_number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Data de nascimento.
        /// </summary>
        [JsonProperty(PropertyName = "birth")]
        public string Birth { get; set; }
        //{
        //    get { return _birth; }
        //    set
        //    {
        //        var quebrado = value.Split('/');
        //        _birth = $"{quebrado[2]}-{quebrado[1]}-{quebrado[0]}";

        //    }
        //}

        /// <summary>
        /// Endereço do cliente.
        /// </summary>
        [JsonProperty(PropertyName = "address",NullValueHandling = NullValueHandling.Ignore)]
        public Address Address { get; set; }

        /// <summary>
        /// Dados de pessoa jurídica
        /// </summary>
        [JsonProperty(PropertyName = "juridical_person")]
        public JuridicalPerson JuridicalPerson { get; set; }
    }
}
