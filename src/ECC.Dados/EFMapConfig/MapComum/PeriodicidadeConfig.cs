 
using ECC.Entidades.EntidadeComum;

namespace ECC.Dados.EFMapConfig.MapComum
{
    public class PeriodicidadeConfig : EntidadeBaseConfig<Periodicidade>
    {
        public PeriodicidadeConfig()
        {
            Property(mc => mc.DescPeriodicidade).IsRequired();
        }
    }
}
