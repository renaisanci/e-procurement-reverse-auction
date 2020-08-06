using Quartz;

namespace ECC.ClientJob
{
    public abstract class BaseJob :  IJob
    {
        public virtual void Execute(IJobExecutionContext context)
        {
            
        }
    }
}