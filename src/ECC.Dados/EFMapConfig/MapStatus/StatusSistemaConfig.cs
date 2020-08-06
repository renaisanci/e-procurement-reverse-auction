
using ECC.EntidadeStatus;

namespace ECC.Dados.EFMapConfig.MapStatus
{
    public class StatusSistemaConfig: EntidadeBaseConfig<StatusSistema>
    {
        public StatusSistemaConfig()
        {
            Property(s => s.DescStatus).IsRequired().HasMaxLength(100);
            Property(s => s.WorkflowStatusId).IsRequired();     
        }

    }
}
