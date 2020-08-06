namespace ECC.API_Web.Models
{
    public class SubCategoriaViewModel
    {
        
        public int Id { get; set; }
        public string DescSubCategoria { get; set; }

        public int CategoriaId { get; set; }

        public string DescCategoria { get; set; }

        public bool Ativo { get; set; }

        public string DescAtivo { get; set; }
    }
}