using System.Web;
using System.Web.Optimization;

namespace EDTESP.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/methods_pt.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bs4").Include(
                      "~/scripts/umd/popper.js",
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/m4").Include(
                "~/Scripts/metro.js",
                "~/Scripts/metro.pt-br.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/mdb").Include(
                "~/Scripts/mdb.js"));

            bundles.Add(new ScriptBundle("~/bundles/masked").Include(
                "~/Scripts/jquery.mask.js"));

            bundles.Add(new ScriptBundle("~/bundles/bdp").Include(
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/i18n/bootstrap-datepicker.pt-BR.js"));

            bundles.Add(new ScriptBundle("~/bundles/maskmoney").Include(
                "~/Scripts/jquery.maskMoney.js"));

            bundles.Add(new ScriptBundle("~/bundles/numeral").Include(
                "~/Scripts/numeral.js",
                "~/Scripts/numeral.pt-br.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                "~/Scripts/select2.js",
                "~/Scripts/i18n/pt-BR.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                "~/Scripts/Chart.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/dt").Include(
                "~/Scripts/datatables/datatables.js",
                "~/Scripts/datatables/responsive/js/dataTables.responsive.js"
                ));

            bundles.Add(new StyleBundle("~/Content/bs4").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/m4").Include(
                "~/Content/metro.css",
                "~/Content/metro-colors.css",
                "~/Content/metro-rtl.css",
                "~/Content/metro-icons.css"));

            bundles.Add(new StyleBundle("~/Content/mdb").Include(
                "~/Content/mdb.css"));

            bundles.Add(new StyleBundle("~/Content/fluent").Include(
                "~/Content/fluent.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css",
                "~/Content/menus.css"));

            bundles.Add(new StyleBundle("~/Content/fa").Include(
                "~/Content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/bdp").Include(
                "~/Content/bootstrap-datepicker.standalone.css"));

            bundles.Add(new StyleBundle("~/Content/dt").Include(
                "~/Content/third-party/datatables.css"));

            bundles.Add(new StyleBundle("~/Content/select2").Include(
                "~/Content/select2.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
