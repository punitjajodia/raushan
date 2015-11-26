using System.Web;
using System.Web.Optimization;

namespace HK
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //Chosen JQuery Plugin for searchable and clickable select list
            bundles.Add(new ScriptBundle("~/bundles/Chosen").Include(
                "~/Scripts/chosen_v1.4.2/chosen.jquery.min.js"
                ));

            bundles.Add(new StyleBundle("~/Content/Chosen").Include(
                "~/Scripts/chosen_v1.4.2/chosen.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/JQueryUI").Include(
                "~/Scripts/jquery-ui/jquery-ui.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/JQueryUI").Include(
                "~/Scripts/jquery-ui/jquery-ui.min.js"));

            bundles.Add(new StyleBundle("~/Content/BootstrapTable").Include(
               "~/Scripts/bootstrap-table-1.8.1/dist/bootstrap-table.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/BootstrapTable").Include(
                "~/Scripts/bootstrap-table-1.8.1/dist/bootstrap-table.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/Custom").Include(
                "~/Scripts/custom.js"
                ));

          

            

        }
    }
}
