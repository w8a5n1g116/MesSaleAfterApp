using System.Web;
using System.Web.Optimization;

namespace MesSaleAfterApp
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备就绪，请使用 https://modernizr.com 上的生成工具仅选择所需的测试。

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/popper").Include(
                      "~/Scripts/umd/popper.min.js",
                      "~/Scripts/umd/popper-utils.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/axios").Include(
                      "~/Scripts/axios.min.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-css").Include("~/Content/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/font-awesome-css").Include("~/Content/font-awesome.min.css"));

            bundles.Add(new StyleBundle("~/Content/Site-css").Include("~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/fileinput").Include("~/Scripts/fileinput_js/fileinput.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/purify").Include("~/Scripts/fileinput_js/purify.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/sortable").Include("~/Scripts/fileinput_js/sortable.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/piexif").Include("~/Scripts/fileinput_js/piexif.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/theme").Include("~/Scripts/fileinput_js/theme.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/LANG").Include("~/Scripts/fileinput_js/LANG.js"));

            bundles.Add(new StyleBundle("~/Content/fileinput-css").Include("~/Content/fileinput.min.css"));
            bundles.Add(new StyleBundle("~/Content/theme-css").Include("~/Content/theme.min.css"));
        }
    }
}
