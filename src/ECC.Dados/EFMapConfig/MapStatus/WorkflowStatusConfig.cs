

using ECC.EntidadeStatus;

namespace ECC.Dados.EFMapConfig.MapStatus
{
   public  class WorkflowStatusConfig : EntidadeBaseConfig<WorkflowStatus>
    {

        public WorkflowStatusConfig()
        {
            

                       Property(w => w.DescWorkslowStatus).IsRequired().HasMaxLength(100);
                       HasMany(w => w.StatusSistemas).WithRequired(t => t.WorkflowStatus).HasForeignKey(t => t.WorkflowStatusId);

        }


    }
}
