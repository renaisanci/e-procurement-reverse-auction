using System.Data.Entity.ModelConfiguration;
using ECC.EntidadePessoa;

namespace ECC.Dados.EFMapConfig.MapPessoa
{
    public class FranquiaFornecedorConfig : EntidadeBaseConfig<FranquiaFornecedor>  
    {
        public FranquiaFornecedorConfig()
        {
            HasKey(w => new { w.FranquiaId, w.FornecedorId });

            Property(w => w.FranquiaId);
            Property(w => w.FornecedorId);
        }
    }
}
