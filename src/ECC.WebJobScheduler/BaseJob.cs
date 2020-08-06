using System.Threading.Tasks;
using Quartz;

namespace ECC.WebJobScheduler
{
    public abstract class BaseJob :  IJob
    {
        public virtual void Execute(IJobExecutionContext context)
        {

        }
    }
}