using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class FaturaConfig : EntidadeBaseConfig<Fatura>
    {
        public FaturaConfig()
        {
            Property(b => b.Status).IsRequired();
            Property(b => b.FornecedorId).IsRequired();
            Property(b => b.DtFechamento).IsRequired();
            Property(b => b.DtVencimento).IsRequired();
            Property(b => b.DtRecebimento).IsOptional();
            Property(b => b.ChargerId).IsOptional();
            Property(b => b.Token).IsOptional();
            Property(b => b.UrlBoleto).IsOptional().HasMaxLength(200);

            HasRequired(x => x.Fornecedor).WithMany().HasForeignKey(x => x.FornecedorId);

            HasMany(x => x.Comissoes).WithRequired(x => x.Fatura).HasForeignKey(x => x.FaturaId);
        }
    }
}
