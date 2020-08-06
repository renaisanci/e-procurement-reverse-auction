using ECC.EntidadeEntrega;

namespace ECC.Dados.EFMapConfig.MapEntrega
{
    public class EntregaConfig : EntidadeBaseConfig<Entrega>
    {
        public EntregaConfig()
        {
			Property(b => b.DescEntrega).IsRequired();
		}
    }
}
