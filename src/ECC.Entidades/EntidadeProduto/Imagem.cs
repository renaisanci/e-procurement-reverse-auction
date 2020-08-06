using ECC.Entidades;
using ECC.Entidades.EntidadeProduto;

namespace ECC.EntidadeProduto
{
    public class Imagem : EntidadeBase
    {
        public string CaminhoImagem { get; set; }

        public string CaminhoImagemGrande { get; set; }

        public int ProdutoId { get; set; }

        public virtual Produto Produto { get; set; }
    }
}
