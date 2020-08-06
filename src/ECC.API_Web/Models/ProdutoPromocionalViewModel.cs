using System;
using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class ProdutoPromocionalViewModel
    {

        public int Id { get; set; }

        public string DescProduto { get; set; }

        public string Fabricante { get; set; }
        public string Marca { get; set; }

        public string UnidadeMedida { get; set; }

        public int CategoriaId { get; set; }

        public string DescCategoria { get; set; }
        public string DescSubCategoria { get; set; }

        public int SubCategoriaId { get; set; }

        public string Imagem { get; set; }

        public string ImagemGrande { get; set; }

        public int Ranking { get; set; }
        public string DescAtivo { get; set; }
        public bool PromoAtivo { get; set; }

        public bool Ativo { get; set; }


        string _precoMedio;
        public string PrecoMedio
        {
            get
            {
                return _precoMedio;
            }
            set
            {
                _precoMedio = value.Replace("R$", "");
            }
        }

        public int UnidadeMedidaId { get; set; }
        public int FabricanteId { get; set; }

        public int MarcaId { get; set; }
        public string Especificacao { get; set; }
        public int FornecedorId { get; set; }

        public FornecedorViewModel Fornecedor { get; set; }
        public int QtdProdutos { get; set; }
        public int QtdMinVenda { get; set; }
        public bool Status { get; set; }
        public string MotivoPromocao { get; set; }
        public string DescMotivoCancelamento { get; set; }
        public DateTime ValidadeProd { get; set; }
        public DateTime InicioPromocao { get; set; }
        public DateTime FimPromocao { get; set; }
        public bool Aprovado { get; set; }
        public bool AprovacaoFranquia { get; set; }

        string _precoPromocao;
        public string PrecoPromocao
        {
            get
            {
                return _precoPromocao;
            }
            set
            {
                _precoPromocao = value.Replace("R$", "");
            }
        }

        public List<PromocaoFormaPagtoViewModel> PromocaoFormaPagto { get; set; }


        public bool FreteGratis { get; set; }
        public string ObsFrete { get; set; }

    }



}