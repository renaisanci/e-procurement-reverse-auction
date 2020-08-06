using System.Web.Optimization;

namespace ECC.API_Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
             "~/Scripts/lib/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                "~/Scripts/lib/bootstrap.js",
                "~/Scripts/lib/toastr.js",
                "~/Scripts/lib/jquery.raty.js",
                "~/Scripts/lib/respond.src.js",
                "~/Scripts/lib/angular.js",
                "~/Scripts/lib/angular-animate.js",
                "~/Scripts/lib/angular-route.js",
                "~/Scripts/lib/angular-cookies.js",
                "~/Scripts/lib/angular-validator.js",
                 "~/Scripts/lib/angular-locale_pt-br.js",
                "~/Scripts/lib/angular-base64.js",
                "~/Scripts/lib/angular-file-upload.js",
                "~/Scripts/lib/angular-mask.js",
                "~/Scripts/lib/angular-money-mask.js",
                "~/Scripts/lib/angucomplete-alt.min.js",
                "~/Scripts/lib/ui-bootstrap-tpls-0.13.1.js",
                "~/Scripts/lib/underscore.js",
                "~/Scripts/lib/raphael.js",
                "~/Scripts/lib/morris.js",
                "~/Scripts/lib/jquery.fancybox.js",
                "~/Scripts/lib/jquery.fancybox-media.js",
                "~/Scripts/lib/loading-bar.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/SPAAdm").Include(
                "~/Scripts/SPAAdm/modules/common.core.js",
                "~/Scripts/SPAAdm/modules/common.ui.js",
                "~/Scripts/SPAAdm/app.js",
                "~/Scripts/SPAAdm/services/apiService.js",
                "~/Scripts/SPAAdm/services/fileUploadService.js",
                "~/Scripts/SPAAdm/services/notificationService.js",
                "~/Scripts/SPAAdm/services/membershipService.js",
                "~/Scripts/SPAAdm/services/admUtilService.js",
                "~/Scripts/SPAAdm/layout/topBar.directive.js",
                 "~/Scripts/SPAAdm/layout/topBarCtrl.js",
                "~/Scripts/SPAAdm/layout/menuLateral.directive.js",
                "~/Scripts/SPAAdm/layout/menuLateralAdmCtrl.js",
                "~/Scripts/SPAAdm/layout/customPager.directive.js",
                "~/Scripts/SPAAdm/home/rootCtrl.js",
                "~/Scripts/SPAAdm/home/indexCtrl.js",
                "~/Scripts/SPAAdm/login/loginCtrl.js",
                "~/Scripts/SPAAdm/login/recuperasenhaCtrl.js",
                "~/Scripts/SPAAdm/membro/membroCtrl.js",
                "~/Scripts/SPAAdm/membro/membroCadastroCtrl.js",               
                "~/Scripts/SPAAdm/fornecedor/fornecedorCtrl.js",
                "~/Scripts/SPAAdm/statusSistema/statusSistemaCtrl.js",
                "~/Scripts/SPAAdm/statusSistema/workflowStatusCtrl.js",
                "~/Scripts/SPAAdm/unidadeMedida/unidadeMedidaCtrl.js",
                "~/Scripts/SPAAdm/categoria/categoriaCtrl.js",
                "~/Scripts/SPAAdm/categoria/subcategoriaCtrl.js",
                "~/Scripts/SPAAdm/produto/produtoCtrl.js",
                "~/Scripts/SPAAdm/fabricante/fabricanteCtrl.js",
                "~/Scripts/SPAAdm/usuario/usuarioCtrl.js",
                "~/Scripts/SPAAdm/usuario/usuarioAdmCtrl.js",
                "~/Scripts/SPAAdm/marca/marcaCtrl.js",
                "~/Scripts/SPAAdm/segmento/segmentoCtrl.js",
                "~/Scripts/SPAAdm/login/alteraSenhaCtrl.js",
                "~/Scripts/SPAAdm/grupo/grupoPermissaoCtrl.js",
                "~/Scripts/SPAAdm/cotacao/painelCtrl.js",
                "~/Scripts/SPAAdm/demanda/demandaCtrl.js",
                "~/Scripts/SPAAdm/email/emailBoasVindaCtrl.js",
                "~/Scripts/SPAAdm/produto/aprovarpromocaoCtrl.js",
                "~/Scripts/SPAAdm/produto/detalhesPromocaoCtrl.js",
                "~/Scripts/SPAAdm/franquias/franquiasCtrl.js",
                "~/Scripts/SPAAdm/menu/menuCtrl.js"


                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/content/css/morris.css",
                "~/content/css/toastr.css",
                "~/content/css/jquery.fancybox.css",
                 "~/content/css/autocomplete.css",
                "~/content/css/loading-bar.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
