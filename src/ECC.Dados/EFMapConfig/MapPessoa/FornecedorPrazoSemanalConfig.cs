using ECC.EntidadePessoa;
using ECC.Entidades.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    class FornecedorPrazoSemanalConfig : EntidadeBaseConfig<FornecedorPrazoSemanal>
    {
        public FornecedorPrazoSemanalConfig()
        {
            Property(fr => fr.Id).IsRequired();
            Property(fr => fr.FornecedorId).IsRequired();
            Property(fr => fr.CidadeId).IsRequired();
            Property(fr => fr.DiaSemana).IsRequired();
            Property(fr => fr.VlPedMinRegiao).IsOptional();
            Property(fr => fr.TaxaEntrega).IsOptional();
            Property(fr => fr.Cif).IsOptional();
        }
    }
}
