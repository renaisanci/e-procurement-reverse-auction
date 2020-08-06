namespace ECC.API_Web.Models
{
    public class StatusSistemaViewModel 
    {
        public int Id { get; set; }
        public string DescStatus { get; set; }
        public int WorkflowStatusId { get; set; }
        public string WorkflowStatusSistema { get; set; }
        public bool Ativo { get; set; }
        public string DescAtivo { get; set; }

        public int Ordem { get; set; }
    }
}