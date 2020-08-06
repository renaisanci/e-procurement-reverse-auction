using Autofac;
using Autofac.Integration.WebApi;
using ECC.Dados;
using ECC.Dados.Infra;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;
using ECC.API_Web.InfraWeb;
using ECC.Dados.Repositorio;
using ECC.Servicos;
using ECC.Servicos.Abstrato;

namespace ECC.WebAPI.App_Start
{

    public class AutofacWebapiConfig
    {
        public static IContainer Container;

        public static void Initialize(HttpConfiguration config)
        {
            Initialize(config, RegisterServices(new ContainerBuilder()));
        }

        public static void Initialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // EF EconomizajaContext
            builder.RegisterType<EconomizajaContext>()
                   .As<DbContext>()
                   .InstancePerRequest();

            builder.RegisterType<DbFactory>()
                .As<IDbFactory>()
                .InstancePerRequest();

            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerRequest();

            builder.RegisterGeneric(typeof(EntidadeBaseRep<>))
                   .As(typeof(IEntidadeBaseRep<>))
                   .InstancePerRequest();

            // Services
            builder.RegisterType<EncryptionService>()
                .As<IEncryptionService>()
                .InstancePerRequest();

            builder.RegisterType<MembershipService>()
                .As<IMembershipService>()
                .InstancePerRequest();

            builder.RegisterType<EmailService>()
              .As<IEmailService>()
              .InstancePerRequest();

            builder.RegisterType<SmsService>()
                .As<ISmsService>()
                .InstancePerRequest();

            builder.RegisterType<UtilService>()
                .As<IUtilService>()
                .InstancePerRequest();

            builder.RegisterType<PagamentoService>()
                .As<IPagamentoService>()
                .InstancePerRequest();


            builder.RegisterType<NotificacoesAlertasService>()
                .As<INotificacoesAlertasService>()
                .InstancePerRequest();

            builder.RegisterType<CalendarioFeriadoService>()
             .As<ICalendarioFeriadoService>()
             .InstancePerRequest();


            // Generic Data Repository Factory
            builder.RegisterType<DataRepositoryFactory>()
                .As<IDataRepositoryFactory>().InstancePerRequest();

            Container = builder.Build();

            return Container;
        }
    }
}