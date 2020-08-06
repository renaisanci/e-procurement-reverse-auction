using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class ImagemConfig : EntidadeBaseConfig<Imagem>
    {
        public ImagemConfig()
        {


            Property(s => s.ProdutoId).IsRequired();
            Property(s => s.CaminhoImagem).IsRequired().HasMaxLength(100);
        }
    }
}
