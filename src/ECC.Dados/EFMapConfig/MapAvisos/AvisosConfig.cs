using ECC.EntidadeAvisos;

namespace ECC.Dados.EFMapConfig.MapAvisos
{
    public class AvisosConfig : EntidadeBaseConfig<Avisos>
    {
        public AvisosConfig()
        {
            Property(a => a.ModuloId).IsRequired();
            Property(a => a.TipoAvisosId).IsRequired();
            Property(a => a.TituloAviso).IsRequired().HasMaxLength(40);
            Property(a => a.DescricaoAviso).IsRequired().HasMaxLength(100);
            Property(u => u.ToolTip).IsRequired().HasMaxLength(40);
        }
    }
}
