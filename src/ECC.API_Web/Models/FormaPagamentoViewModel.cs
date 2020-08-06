using ECC.EntidadePessoa;
using Newtonsoft.Json;

namespace ECC.API_Web.Models
{
    public class FormaPagamentoViewModel
    {
        public int Id { get; set; }

        public string Identificacao { get; set; }

        public string Nome { get; set; }

        public string Numero { get; set; }

        public string Validade { get; set; }

        public string CVC { get; set; }
    }
}