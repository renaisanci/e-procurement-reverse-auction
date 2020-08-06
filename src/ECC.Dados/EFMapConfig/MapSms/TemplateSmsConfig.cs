using ECC.EntidadeSms;

namespace ECC.Dados.EFMapConfig.MapSms
{
    public class TemplateSmsConfig : EntidadeBaseConfig<TemplateSms>
    {
        public TemplateSmsConfig()
        {
            Property(t => t.DescTemplate).IsRequired();
            Property(t => t.Template).IsRequired().HasColumnType("text");
        }
    }
}
