using ECC.EntidadeRecebimento;

namespace ECC.Dados.EFMapConfig.MapRecebimento
{
    public class ParametrosRecebimentoConfig : EntidadeBaseConfig<ParametrosRecebimento>
    {
        public ParametrosRecebimentoConfig()
        {
            Property(b => b.MembroMensalidade).IsRequired().HasColumnType("money");
            Property(b => b.MembroDiaFechamento).IsRequired();
            Property(b => b.MembroCompraDesconto).IsRequired().HasColumnType("money");
            Property(b => b.FornecedorComissao).IsRequired();
            Property(b => b.FornecedorDiaFechamento).IsRequired();
            Property(b => b.FornecedorDiaVencimento).IsRequired();
        }
    }
}
