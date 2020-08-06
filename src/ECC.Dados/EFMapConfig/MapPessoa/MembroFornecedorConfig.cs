 
using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class MembroFornecedorConfig : EntidadeBaseConfig<MembroFornecedor>
    {
        public MembroFornecedorConfig()
        {
            Property(mf => mf.MembroId).IsRequired();
            Property(mf => mf.FornecedorId).IsRequired();
        }
    }
}
