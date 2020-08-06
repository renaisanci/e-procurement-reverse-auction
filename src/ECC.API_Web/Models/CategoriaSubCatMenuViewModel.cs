using System.Collections.Generic;

namespace ECC.API_Web.Models
{
    public class CategoriaSubCatMenuViewModel
    {

        public int Id { get; set; }
        public string DescCategoria { get; set; }
        public string PrimeiraLetra { get; set; }

        public List<SubCategoriaViewModel> MenuSubCategorias;

    }
}