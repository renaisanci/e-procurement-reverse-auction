using ECC.EntidadeEmail;

namespace ECC.Dados.EFMapConfig.MapEmail
{
   public class EmailsConfig : EntidadeBaseConfig<Emails>
    {

        public EmailsConfig()
        {
            Property(t => t.EmailDestinatario).IsRequired();
            Property(t => t.CorpoEmail).IsRequired().HasColumnType("nvarchar(MAX)").IsMaxLength();
            Property(t => t.AssuntoEmail).HasMaxLength(250);


               

        }

    }
}
