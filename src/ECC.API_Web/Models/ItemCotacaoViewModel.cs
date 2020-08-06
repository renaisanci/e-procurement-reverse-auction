namespace ECC.API_Web.Models
{
    public class ItemCotacaoViewModel
    {


        public string DescProduto { get; set; }
        public int qtd { get; set; }
        public int ProdutoId { get; set; }
		
		public string Observacao { get; set; }

		string _precoNegociadoUnit;
        public string PrecoNegociadoUnit
        {
            get
            {
                return _precoNegociadoUnit;
            }
            set
            {
                _precoNegociadoUnit = value.Replace("R$", "");
            }
        }

        public bool flgOutraMarca { get; set; }

    }
}