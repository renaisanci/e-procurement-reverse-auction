namespace ECC.API_Web.Models
{
    public class EstoqueViewModel
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }

        public string DescProduto { get; set; }

        public int MembroId { get; set; }

        public string DescMembro { get; set; }

        public int EnderecoId { get; set; }

        public string DescEndereco { get; set; }

        public int MinimoEstoque { get; set; }

        public int MaximoEstoque { get; set; }

        public int QtdEstoque { get; set; }

        public int QtdEstoqueReceber { get; set; }

        public bool Ativo { get; set; }

        public string DescAtivo { get; set; }
    }
}