using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class MensalidadeDetalheConfig : EntidadeBaseConfig<MensalidadeDetalhe>
    {
        public MensalidadeDetalheConfig()
        {
            Property(b => b.Descricao).IsRequired();
            Property(b => b.MensalidadeId).IsRequired();
            Property(b => b.Tipo).IsRequired();
            Property(b => b.Valor).IsOptional().HasColumnType("money");

            HasRequired(x => x.Mensalidade).WithMany().HasForeignKey(x => x.MensalidadeId);
        }
    }
}
