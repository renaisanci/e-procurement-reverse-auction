using System;
using System.Collections.Generic;
using ECC.EntidadeProduto;

namespace ECC.Entidades.EntidadeProduto
{
    public class Produto : EntidadeBase
    {
        public Produto()
        {
            this.Imagens = new List<Imagem>();
            this.Rankings = new List<Ranking>();
            this.Fornecedores = new List<FornecedorProduto>();
            this.Franquias = new List<FranquiaProduto>();
        }

        public int SubCategoriaId { get; set; }
        public virtual SubCategoria SubCategoria { get; set; }
        public int UnidadeMedidaId { get; set; }
        public virtual UnidadeMedida UnidadeMedida { get; set; }

        public int? FabricanteId { get; set; }
        public virtual Fabricante Fabricante { get; set; }

        public int MarcaId { get; set; }
        public virtual Marca Marca { get; set; }

        public virtual ProdutoPromocional ProdutoPromocional { get; set; }
        public int? ProdutoPromocionalId { get; set; }

        public string DescProduto { get; set; }
        public String Especificacao { get; set; }
        public decimal PrecoMedio { get; set; }

        public string CodigoCNP { get; set; }

        //campos a serem criados
        // no caso de produtos congelados ou resfriados
        //public string Conservacao { get; set; }

        //campos a serem criados
        //no caso de carnes o peso da peça varia
        //public string PesoMedio { get; set; }

        public virtual ICollection<Imagem> Imagens { get; set; }

        public virtual ICollection<Ranking> Rankings { get; set; }

        public virtual ICollection<FornecedorProduto> Fornecedores { get; set; }

        public virtual ICollection<FranquiaProduto> Franquias { get; set; }
    }
}
