namespace ECC.API_Web.Models
{
    public class ProdutoViewModel
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

        public decimal Preco { get; set; }
        public string DescAtivo { get; set; }
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

        public string CodigoCNP { get; set; }

        public int UnidadeMedidaId { get; set; }
        public int FabricanteId { get; set; }

        public int MarcaId { get; set; }
        public string Especificacao { get; set; }

        public int ProdutoPromocionalId { get; set; }

    }



}