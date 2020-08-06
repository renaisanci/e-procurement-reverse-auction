namespace ECC.API_Web.Models
{
    public class ImagemViewModel
    {

        public int ImagemId { get; set; }
        public int IdProduto { get; set; }
        public string CaminhoImagem { get; set; }

        public string CaminhoImagemGrande { get; set; }
    }
}