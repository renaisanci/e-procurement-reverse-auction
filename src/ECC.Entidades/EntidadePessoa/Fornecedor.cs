using System;
using System.Collections.Generic;
using ECC.EntidadeFormaPagto;
using ECC.EntidadePedido;
using ECC.EntidadeProduto;
using ECC.EntidadeRecebimento;
using ECC.Entidades;
using ECC.Entidades.EntidadePessoa;

namespace ECC.EntidadePessoa
{
    public class Fornecedor : EntidadeBase
    {
        public Fornecedor()
        {
            this.FornecedorFormaPagtos = new List<FornecedorFormaPagto>();
            this.FornecedorRegiao = new List<FornecedorRegiao>();
            this.FornecedorRegiaoSemanal = new List<FornecedorPrazoSemanal>();
            this.AvaliacaoFornecedor = new List<AvaliacaoFornecedor>();
            this.Faturas = new List<Fatura>();
            this.Produtos = new List<FornecedorProduto>();
            this.Franquias = new List<FranquiaFornecedor>();
        }

        public int PessoaId { get; set; }

        public string Responsavel { get; set; }

        public decimal VlPedidoMin { get; set; }

        public virtual Pessoa Pessoa { get; set; }

        public string Descricao { get; set; }

        public string PalavrasChaves { get; set; }

        public bool? PrimeiraAvista { get; set; }

        public string Observacao{ get; set; }

        public string ObservacaoEntrega { get; set; }

        public string DddTel { get; set; }
        public string Telefone { get; set; }

        public string DddCel { get; set; }
        public string Celular { get; set; }

        public string Contato { get; set; }

        public DateTime? DataFimPeriodoGratuito { get; set; }

        public virtual IList<FornecedorFormaPagto> FornecedorFormaPagtos { get; set; }

        public virtual ICollection<FornecedorCategoria> FornecedorCategorias { get; set; }

        public virtual ICollection<FornecedorRegiao> FornecedorRegiao { get; set; }

        public virtual ICollection<FornecedorPrazoSemanal> FornecedorRegiaoSemanal { get; set; }

        public virtual ICollection<AvaliacaoFornecedor> AvaliacaoFornecedor { get; set; }

        public virtual ICollection<Fatura> Faturas { get; set; }

        public virtual ICollection<FornecedorProduto> Produtos { get; set; }

        public virtual ICollection<FranquiaFornecedor> Franquias { get; set; }
    }
}
