using System.Collections.Generic;
using ECC.Entidades;

namespace ECC.EntidadeStatus
{
    public class WorkflowStatus : EntidadeBase
    {
        public string DescWorkslowStatus { get; set; }

        public virtual ICollection<StatusSistema> StatusSistemas { get; set; } 
    }
}
