using System;
using System.Collections.Generic;
using ECC.EntidadePessoa;
using ECC.Entidades;

namespace ECC.EntidadeRecebimento
{
    public class Fatura : EntidadeBase
    {
        public Fatura()
        {
            Comissoes = new List<Comissao>();
        }

        public StatusFatura Status { get; set; }

        public int FornecedorId { get; set; }

        public virtual Fornecedor Fornecedor { get; set; }

        public DateTime DtFechamento { get; set; }
        
        public DateTime DtVencimento { get; set; }

        public DateTime? DtRecebimento { get; set; }

        public int ChargerId { get; set; }

        public string Token { get; set; }

        public string UrlBoleto { get; set; }

        public virtual ICollection<Comissao> Comissoes { get; set; }
    }
}
