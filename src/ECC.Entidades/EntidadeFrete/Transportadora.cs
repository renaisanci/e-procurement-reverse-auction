using ECC.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECC.EntidadeFrete
{
    public class Transportadora : EntidadeBase
    {
        public Transportadora()
        {

        }

        public int cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string InscricaoEstadual { get; set; }
        public int CEP { get; set; }
        public string Endereco { get; set; }
        public int Numero { get; set; }
        public string Complemente { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string CodigoIBGE { get; set; }
        public string Estado { get; set; }
        public string Contato { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }

    }
}
