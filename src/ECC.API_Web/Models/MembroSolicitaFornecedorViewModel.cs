using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class MembroSolicitaFornecedorViewModel
    {
        public int FornecedorId { get; set; }

        public string NomeFornecedor { get; set; }

        public string NomeRazaoSocialFornecedor { get; set; }

        public string CnpjFornecedor { get; set; }

        public string PrazoEntegaFornecedor { get; set; }

        public int[] FormaPagtos { get; set; }

        public int MediaAvaliacaoPedido { get; set; }

        public string Descricao { get; set; }

        public string PalavrasChaves { get; set; }

        public int QtdNotas { get; set; }

        public decimal? VlPedMinimo { get; set; }

        public string ObservacaoFormPagto { get; set; }

        public string ObservacaoEntrega{ get; set; }

        public List<FormaPagtoViewModel> FormasPagamento { get; set; }
        public List<FornecedorPrazoSemanalViewModel> FornecedorPrazoSemanal { get; set; }
        public List<FornecedorRegiaoViewModel> FornecedorRegiao { get; set; }
       public string Cidade { get; set; }

        public bool DescAtivo { get; set; }

        public bool TrabalhaMembro { get; set; }

        public bool TrabalhaFranquia { get; set; }
    }
}