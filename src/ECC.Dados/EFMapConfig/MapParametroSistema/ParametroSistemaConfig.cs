using ECC.EntidadeParametroSistema;

namespace ECC.Dados.EFMapConfig.MapParametroSistema
{
    public class ParametroSistemaConfig : EntidadeBaseConfig<ParametroSistema>
    {

        public ParametroSistemaConfig()
        {
            Property(s => s.Descricao).IsRequired().HasMaxLength(255);
            Property(s => s.Codigo).IsRequired().HasMaxLength(20);
            Property(s => s.Valor).IsRequired().HasMaxLength(512);
        }
    }
}
