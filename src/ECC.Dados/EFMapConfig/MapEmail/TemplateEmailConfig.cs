using ECC.EntidadeEmail;

namespace ECC.Dados.EFMapConfig.MapEmail
{
    public class TemplateEmailConfig : EntidadeBaseConfig<TemplateEmail>
    {
        public TemplateEmailConfig()
        {
            Property(t => t.DescTemplate).IsRequired();
            Property(t => t.Template).IsRequired().HasColumnType("nvarchar(MAX)").IsMaxLength();
        }
    }
}
