using System.Web.Optimization;

namespace ECC.WebJobScheduler
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
             "~/Scripts/lib/modernizr.js"));

            bundles.Add(new ScriptBundle("~/bundles/lib").Include(
                 "~/Scripts/lib/jquery.min.js",
                "~/Scripts/lib/bootstrap.js",
                "~/Scripts/lib/toastr.js"
                ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                  "~/content/css/bootstrap.min.css",
                "~/content/css/toastr.css"
               ));

            BundleTable.EnableOptimizations = false;
        }
    }
}
