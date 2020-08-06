using ECC.EntidadeSms;

namespace ECC.Dados.EFMapConfig.MapSms
{
    public class SmsConfig : EntidadeBaseConfig<Sms>
    {
        public SmsConfig()
        {           
            Property(t => t.Status).IsRequired();
            Property(t => t.DataEnvio).IsOptional();
            Property(t => t.DataConfirmacaoEnvio).IsOptional();
            Property(t => t.Mensagem).IsRequired().HasMaxLength(160);
            Property(t => t.Numero).IsRequired().HasMaxLength(11);
            Property(t => t.Erro).IsOptional().HasColumnType("text");
            Property(t => t.IdMessageSmsGateway).IsOptional();
            Property(t => t.DataInicioProcesso).IsOptional();
            Property(t => t.OrigemSms).IsOptional();
            Property(t => t.IdEntidadeOrigem).IsOptional();
        }
    }
}
