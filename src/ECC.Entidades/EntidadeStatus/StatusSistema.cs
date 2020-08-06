using ECC.Entidades;

namespace ECC.EntidadeStatus
{
    public class StatusSistema : EntidadeBase
    {
        public string DescStatus { get; set; }
        public virtual WorkflowStatus WorkflowStatus { get; set; }
        public int WorkflowStatusId { get; set; }
        public int Ordem { get; set; }
    }
}
