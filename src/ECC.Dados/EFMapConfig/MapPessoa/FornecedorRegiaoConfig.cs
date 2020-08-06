using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    class FornecedorRegiaoConfig : EntidadeBaseConfig<FornecedorRegiao>
    {
        public FornecedorRegiaoConfig()
        {
            Property(fr => fr.Id).IsRequired();
            Property(fr => fr.FornecedorId).IsRequired();
            Property(fr => fr.CidadeId).IsRequired();
            Property(fr => fr.VlPedMinRegiao).IsOptional();
            Property(fr => fr.Prazo).IsOptional();
            Property(fr => fr.TaxaEntrega).IsOptional();
            Property(fr => fr.Cif).IsOptional();
        }
    }
}
