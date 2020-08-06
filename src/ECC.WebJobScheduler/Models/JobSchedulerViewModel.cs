using Quartz;

namespace ECC.WebJobScheduler.Models
{
    public class JobSchedulerViewModel
    {
        public IScheduler EnviaEmailsSms { get; set; }
        public IScheduler GerarCotacao { get; set; }
        public IScheduler FecharCotacao { get; set; }
        public IScheduler NotificacaoAlertas { get; set; }
        public IScheduler GeraFaturaFornecedores { get; set; }
        public IScheduler GeraMensalidadeMembro { get; set; }
    }
}