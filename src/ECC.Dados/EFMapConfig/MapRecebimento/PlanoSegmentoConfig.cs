using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class PlanoSegmentoConfig : EntidadeBaseConfig<PlanoSegmento>
    {
        public PlanoSegmentoConfig()
        {
            Property(b => b.SegmentoId).IsRequired();
            Property(b => b.PlanoMensalidadeId).IsRequired();
            Property(b => b.Ativo).IsRequired();
        }
    }
}
