using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class MensalidadeConfig : EntidadeBaseConfig<Mensalidade>
    {
        public MensalidadeConfig()
        {
            Property(b => b.Status).IsRequired();
            Property(b => b.MembroId).IsRequired();
            Property(b => b.DtVencimento).IsRequired();
            Property(b => b.DtRecebimento).IsOptional();          
            Property(b => b.ChargerId).IsOptional();
            Property(b => b.UrlPdf).IsOptional().HasMaxLength(300);
            Property(b => b.Token).IsOptional();
            Property(b => b.PlanoMensalidadeId).IsRequired();
            Property(b => b.DtEfetivarPlano).IsOptional();
            HasRequired(x => x.Membro).WithMany().HasForeignKey(x => x.MembroId);
            HasRequired(x => x.PlanoMensalidade).WithMany().HasForeignKey(x => x.PlanoMensalidadeId);

            HasMany(x => x.Detalhes).WithRequired(x => x.Mensalidade).HasForeignKey(x => x.MensalidadeId);
        }
    }
}
