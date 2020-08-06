using ECC.EntidadeArquivo;

namespace ECC.Dados.EFMapConfig.MapArquivo
{
    public class TermoUsoConfig : EntidadeBaseConfig<TermoUso>
    {
        public TermoUsoConfig()
        {
            Property(t => t.Tipo).IsRequired();
            Property(t => t.Documento).IsRequired().HasColumnType("nvarchar(MAX)").IsMaxLength();
        }
    }
}
