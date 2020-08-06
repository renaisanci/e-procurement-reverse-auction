using ECC.EntidadeProduto;

namespace ECC.Dados.EFMapConfig.MapProduto
{
    public class RankingConfig : EntidadeBaseConfig<Ranking>
    {


        public RankingConfig()
        {

            Property(s => s.ProdutoId).IsRequired();
            Property(s => s.Nota).IsRequired();

        }
      


    }
}
