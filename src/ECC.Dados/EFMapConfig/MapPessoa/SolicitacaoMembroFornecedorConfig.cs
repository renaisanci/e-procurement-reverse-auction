 
using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class SolicitacaoMembroFornecedorConfig : EntidadeBaseConfig<SolicitacaoMembroFornecedor>
    {
        public SolicitacaoMembroFornecedorConfig()
        {
            
            Property(p => p.MembroId).IsRequired();
            Property(p => p.FornecedorId).IsRequired();
            Property(p => p.MotivoRecusa).IsRequired().HasMaxLength(200);
          
        }


    }
}
