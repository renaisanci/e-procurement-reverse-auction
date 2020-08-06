using ECC.ClientJob.JobEmail;
using ECC.ClientJob.JobFecharCotacao;
using ECC.ClientJob.JobGeraCotacao;
using ECC.ClientJob.JobNotificacoesAlertas;
using ECC.ClientJob.JobPagamentos;
using Quartz;
using Quartz.Impl;

namespace ECC.ClientJob
{
    public class JobScheduler
    {
        public static void Start()
        {

            #region Scheduler Envia Email Sms

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail jobEmailSms = JobBuilder.Create<EnvioEmailSms>().Build();

            ITrigger triggerEmailSms = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(120) // Chama serviço para enviar Email e Sms de 2 em 2 minutos.
            .RepeatForever())
            .Build();

            scheduler.ScheduleJob(jobEmailSms, triggerEmailSms);

            #endregion


            #region Scheduler Gera Cotação

            IScheduler schedulerGeraCotacao = StdSchedulerFactory.GetDefaultScheduler();
            schedulerGeraCotacao.Start();

            IJobDetail jobGeraCotacao = JobBuilder.Create<GeraCotacao>().Build();

            ITrigger triggerGeraCotacao = TriggerBuilder.Create()
            .WithIdentity("trigger2", "group2")
            .StartNow()
            .WithCronSchedule("0 00 12 * * ?") // Ás 12:00:00 é gerado cotação caso exista pedidos
            .Build();

            schedulerGeraCotacao.ScheduleJob(jobGeraCotacao, triggerGeraCotacao);

            #endregion


            #region Scheduler Fechar Cotação

            IScheduler schedulerFecharCotacao = StdSchedulerFactory.GetDefaultScheduler();
            schedulerFecharCotacao.Start();

            IJobDetail jobFecharCotacao = JobBuilder.Create<FecharCotacao>().Build();

            ITrigger triggerFecharCotacao = TriggerBuilder.Create()
            .WithIdentity("trigger3", "group3")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(2) // Chama serviço para fechar cotação de 2 em 2 minutos.
            .RepeatForever())
            .Build();

            schedulerFecharCotacao.ScheduleJob(jobFecharCotacao, triggerFecharCotacao);

            #endregion


            #region Scheduler Notificacoes e Alertas
            
            IScheduler schedulerNotificacoesAlertas = StdSchedulerFactory.GetDefaultScheduler();
            schedulerNotificacoesAlertas.Start();

            IJobDetail jobNotificacoesAlertas = JobBuilder.Create<NotificacoesAlertas>().Build();

            ITrigger triggerNotificacoesAlertas = TriggerBuilder.Create()
            .WithIdentity("trigger4", "group4")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(2) // Chama serviço para enviar notificações e alertas de 2 em 2 minutos.
            .RepeatForever())
            .Build();

            schedulerNotificacoesAlertas.ScheduleJob(jobNotificacoesAlertas, triggerNotificacoesAlertas);

            #endregion


            #region Scheduler Pagamentos / Gera Fatura Fornecedores

            IScheduler schedulerPagamentosFatura = StdSchedulerFactory.GetDefaultScheduler();
            schedulerPagamentosFatura.Start();

            IJobDetail jobPagamentosFatura = JobBuilder.Create<GeraFatura>().Build();

            ITrigger triggerPagamentosFatura = TriggerBuilder.Create()
            .WithIdentity("trigger5", "group5")
            .StartNow()
            .WithCronSchedule("0 00 01 * * ?") // Ás 00:01:00 são gerados boletos
            .Build();

            schedulerPagamentosFatura.ScheduleJob(jobPagamentosFatura, triggerPagamentosFatura);

            #endregion


            #region Scheduler Pagamentos / Gera Mensalidade Membros

            IScheduler schedulerPagamentosMensalidade = StdSchedulerFactory.GetDefaultScheduler();
            schedulerPagamentosMensalidade.Start();

            IJobDetail jobPagamentosMensalidade = JobBuilder.Create<GeraMensalidade>().Build();

            ITrigger triggerPagamentosMensalidade = TriggerBuilder.Create()
            .WithIdentity("trigger6", "group6")
            .StartNow()
            .WithCronSchedule("0 00 01 * * ?") // Ás 00:01:00 são geradas mensalidades
            .Build();

            schedulerPagamentosMensalidade.ScheduleJob(jobPagamentosMensalidade, triggerPagamentosMensalidade);

            #endregion

        }
    }
}