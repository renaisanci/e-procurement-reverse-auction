using CrystalQuartz.Core.SchedulerProviders;
using ECC.WebJobScheduler.JobEmail;
using ECC.WebJobScheduler.JobFecharCotacao;
using ECC.WebJobScheduler.JobGeraCotacao;
using ECC.WebJobScheduler.JobNotificacoesAlertas;
using ECC.WebJobScheduler.JobPagamentos;
using ECC.WebJobScheduler.JobPrecoCotacaoFornecedor;
using ECC.WebJobScheduler.JobCancelarPlanoAssinatura;
using Quartz;
using Quartz.Impl;
using ECC.WebJobScheduler.JobCancelarPedidoMembro;

namespace ECC.WebJobScheduler
{
    public class JobScheduler : StdSchedulerProvider
    {
        public static IScheduler CurrentScheduler { get; private set; }

        protected override void InitScheduler(IScheduler scheduler)
        {
            if (scheduler.IsStarted)
                return;

            CurrentScheduler = scheduler;
            JobEnviaEmailSmsService();
            JobGerarCotacaoService();
            JobFecharCotacaoService();
            JobNotificacaoAlertasService();
            JobGeraFaturaFornecedoresService();
            JobGeraMensalidadeMembroService();
            JobPrecoCotacaoFornecedorService();
            JobLembreteFornecedorAceiteMembroService();
            JobCancelarPlanoAssinaturaService();
            JobCancelarPedidoMembroService();
        }

        #region Métodos

        public static void JobEnviaEmailSmsService()
        {
            IJobDetail jobEmailSms = JobBuilder.Create<EnvioEmailSms>().WithIdentity("jobEmailSms").Build();
            ITrigger triggerEmailSms = TriggerBuilder.Create()
            .WithIdentity("triggerEmailSms")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(2) // Chama serviço para enviar Email e Sms de 2 em 2 minutos.
            .RepeatForever())
            .Build();

            CurrentScheduler.ScheduleJob(jobEmailSms, triggerEmailSms);
        }

        public static void JobGerarCotacaoService()
        {
            IJobDetail jobGeraCotacao = JobBuilder.Create<GeraCotacao>().WithIdentity("jobGeraCotacao").Build();
            ITrigger triggerGeraCotacao = TriggerBuilder.Create()
            .WithIdentity("triggerGeraCotacao")
            .StartNow()
            .WithCronSchedule("0 00 12 * * ?") // Ás 12:00:00 é gerado cotação caso exista pedidos
            .Build();

            CurrentScheduler.ScheduleJob(jobGeraCotacao, triggerGeraCotacao);
        }

        public static void JobFecharCotacaoService()
        {
            IJobDetail jobFecharCotacao = JobBuilder.Create<FecharCotacao>().WithIdentity("jobFecharCotacao").Build();
            ITrigger triggerFecharCotacao = TriggerBuilder.Create()
            .WithIdentity("triggerFecharCotacao")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(2) // Chama serviço para fechar cotação de 2 em 2 minutos.
            .RepeatForever())
            .Build();

            CurrentScheduler.ScheduleJob(jobFecharCotacao, triggerFecharCotacao);
        }

        public static void JobNotificacaoAlertasService()
        {
            IJobDetail jobNotificacoesAlertas = JobBuilder.Create<NotificacoesAlertas>().WithIdentity("jobNotificacoesAlertas").Build();
            ITrigger triggerNotificacoesAlertas = TriggerBuilder.Create()
            .WithIdentity("triggerNotificacoesAlertas")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(2) // Chama serviço para enviar notificações e alertas de 2 em 2 minutos.
            .RepeatForever())
            .Build();

            CurrentScheduler.ScheduleJob(jobNotificacoesAlertas, triggerNotificacoesAlertas);
        }

        public static void JobGeraFaturaFornecedoresService()
        {
            IJobDetail jobPagamentosFatura = JobBuilder.Create<GeraFatura>().WithIdentity("jobPagamentosFatura").Build();
            ITrigger triggerPagamentosFatura = TriggerBuilder.Create()
            .WithIdentity("triggerPagamentosFatura")
            .StartNow()
            .WithCronSchedule("0 00 01 * * ?") // Ás 00:01:00 são gerados boletos
            .Build();

            CurrentScheduler.ScheduleJob(jobPagamentosFatura, triggerPagamentosFatura);
        }

        public static void JobGeraMensalidadeMembroService()
        {
            IJobDetail jobPagamentosMensalidade = JobBuilder.Create<GeraMensalidade>().WithIdentity("jobPagamentosMensalidade").Build();
            ITrigger triggerPagamentosMensalidade = TriggerBuilder.Create()
            .WithIdentity("triggerPagamentosMensalidade")
            .StartNow()
            .WithCronSchedule("0 00 01 * * ?") // Ás 00:00:01 são efetuadas as trocas de planos para o membro, caso exista
            .Build();

            CurrentScheduler.ScheduleJob(jobPagamentosMensalidade, triggerPagamentosMensalidade);
        }

        public static void JobPrecoCotacaoFornecedorService()
        {
            IJobDetail jobPrecoCotacaoFornecedor = JobBuilder.Create<PrecoCotacaoFornecedor>().WithIdentity("jobPrecoCotacaoFornecedor").Build();
            ITrigger triggerPrecoCotacaoFornecedor = TriggerBuilder.Create()
            .WithIdentity("triggerPrecoCotacaoFornecedor")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(10) // Chama serviço para precificar cotações para fornecedores de 2 em 2 minutos.
            .RepeatForever())
            .Build();

            CurrentScheduler.ScheduleJob(jobPrecoCotacaoFornecedor, triggerPrecoCotacaoFornecedor);
        }

        public static void JobLembreteFornecedorAceiteMembroService()
        {
            IJobDetail jobLembreteFornecedorAceiteMembro = JobBuilder.Create<LembreteFornecedor>().WithIdentity("jobLembreteFornecedorAceiteMembro").Build();
            ITrigger triggerLembreteFornecedorAceiteMembro = TriggerBuilder.Create()
            .WithIdentity("triggerLembreteFornecedorAceiteMembro")
            .StartNow()
            .WithCronSchedule("0 00 09 * * ?") // Ás 09:00:00 é enviado lembrete para fornecedores aceitarem os membros
            .Build();


            CurrentScheduler.ScheduleJob(jobLembreteFornecedorAceiteMembro, triggerLembreteFornecedorAceiteMembro);
        }

        public static void JobCancelarPlanoAssinaturaService()
        {
            IJobDetail jobCancelarPlanoAssinatura = JobBuilder.Create<CancelarPlanoAssinatura>().WithIdentity("jobCancelarPlanoAssinatura").Build();
            ITrigger triggerCancelarPlanoAssinatura = TriggerBuilder.Create()
            .WithIdentity("triggerCancelarPlanoAssinatura")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(2) // Chama serviço para Cancelar Plano e Assinatura Membro e Fornecedor de 2 em 2 minutos.
            .RepeatForever())
            .Build();

            CurrentScheduler.ScheduleJob(jobCancelarPlanoAssinatura, triggerCancelarPlanoAssinatura);
        }

        public static void JobCancelarPedidoMembroService()
        {
            IJobDetail jobCancelarPedidoMembro = JobBuilder.Create<CancelarPedidoMembro>().WithIdentity("jobCancelarPedidoMembro").Build();
            ITrigger triggerCancelarPedidoMembro = TriggerBuilder.Create()
            .WithIdentity("triggerCancelarPedidoMembro")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(15) // Chama serviço para Cancelar Pedidos Membro sem Aprovação de 15 em 15 minutos.
            .RepeatForever())
            .Build();

            CurrentScheduler.ScheduleJob(jobCancelarPedidoMembro, triggerCancelarPedidoMembro);
        }

        #endregion
    }
}