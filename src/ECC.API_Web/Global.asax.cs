using System;
using ECC.WebAPI.App_Start;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ECC.Dados.Infra;
using ECC.Dados.Repositorio;
using ECC.EntidadeUsuario;
using WebGrease.Css.Extensions;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using Microsoft.AspNet.WebApi.Extensions.Compression.Server;

namespace ECC.API_Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(config);
            Bootstrapper.Run();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalConfiguration.Configuration.EnsureInitialized();
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //aplicado para fazer a compressão de dados nas requisições
            config.MessageHandlers.Insert(0, new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            //#if !DEBUG

            //            var unitOfWork = new UnitOfWork(new DbFactory());
            //            var usuarioRep = new EntidadeBaseRep<Usuario>(unitOfWork.DbFactory);
            //            var usuarios = usuarioRep.All;
            //            usuarios.ForEach(x =>
            //            {
            //                x.TokenSignalR = "";
            //                x.Logado = false;
            //                usuarioRep.Edit(x);
            //            });
            //            unitOfWork.Commit();            
            //#endif
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }
    }
}
