 
using ECC.EntidadeCotacao;

namespace ECC.Dados.EFMapConfig.MapCotacao
{
    public class HistStatusCotacaoConfig : EntidadeBaseConfig<HistStatusCotacao>
    {



        public HistStatusCotacaoConfig()
        {
            Property(b => b.CotacaoId).IsRequired();
            Property(b => b.StatusSistemaId).IsRequired();
        }
    }
}
