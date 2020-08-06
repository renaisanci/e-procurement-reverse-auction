using System;
using System.Collections.Generic;
using ECC.EntidadeFormaPagto;
using ECC.EntidadePessoa;



namespace ECC.Entidades.EntidadeProduto
{
    public class ProdutoPromocional : EntidadeBase
    {

        public ProdutoPromocional()
        {
            PromocaoFormaPagtos = new List<PromocaoFormaPagto>();
        }

        public virtual Fornecedor Fornecedor { get; set; }
        public int FornecedorId { get; set; }
        public int QuantidadeProduto { get; set; }

        public int? QtdMinVenda { get; set; }
        public string MotivoPromocao { get; set; }

        public string DescMotivoCancelamento { get; set; }

        public DateTime ValidadeProduto { get; set; }
        public DateTime InicioPromocao { get; set; }
        public DateTime FimPromocao { get; set; }
        public bool Aprovado { get; set; }
        public decimal PrecoPromocao { get; set; }
        public virtual ICollection<PromocaoFormaPagto> PromocaoFormaPagtos { get; set; }

        public bool FreteGratis { get; set; }
        public string ObsFrete { get; set; }
    }
}
