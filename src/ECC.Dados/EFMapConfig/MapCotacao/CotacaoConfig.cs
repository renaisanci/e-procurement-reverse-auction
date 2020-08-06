
using ECC.EntidadeCotacao;

namespace ECC.Dados.EFMapConfig.MapCotacao
{
    public class CotacaoConfig : EntidadeBaseConfig<Cotacao>
    {
        public CotacaoConfig()
        {
            Property(b => b.StatusSistemaId).IsRequired();
            Property(b => b.DtFechamento).IsRequired();
        }
    }
}
