using System.Web;
using System.Web.Optimization;

namespace ZhiNotification
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            const string jQueryCdn = "http://ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js";

            bundles.Add(new ScriptBundle("~/bundles/jquery", jQueryCdn).Include(
                       "~/Content/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/Scripts/bootstrap.js",
                      "~/Content/Scripts/respond.js",
                      "~/Content/Scripts/material/material.js",
                      "~/Content/Scripts/material/ripples.js",
                      "~/Content/Scripts/jquery.blockUI.js",
                      "~/Content/Scripts/toastr.js",
                      "~/Content/Scripts/jquery.dataTables.js",
                      "~/Content/Scripts/bootstrap-treeview.min.js",
                      "~/Content/Scripts/lodash.min.js",
                      "~/Content/Scripts/jquery.livequery.min.js",
                      "~/Content/Scripts/jstree.min.js",
                      "~/Content/Scripts/app/account.js",
                      "~/Content/Scripts/app/notification-category-subscription.js",
                      "~/Content/Scripts/app/manage-changes.js",
                      "~/Content/Scripts/app/view-logs.js"));

            bundles.Add(new StyleBundle("~/Content/CSS").Include(
                      "~/Content/CSS/bootstrap.css",
                      "~/Content/CSS/animate.css",
                      "~/Content/CSS/font-awesome.css",
                      "~/Content/CSS/material-fullpalette.css",
                      "~/Content/CSS/ripples.css",
                      "~/Content/CSS/roboto.css",
                      "~/Content/CSS/toastr.css",
                      "~/Content/CSS/jquery.dataTables.css",
                      "~/Content/CSS/utilities.css",
                      "~/Content/CSS/site.css",
                      "~/Content/CSS/bootstrap-treeview.min.css",
                      "~/Content/CSS/jstree-themes/default/style.min.css"));

            bundles.IgnoreList.Clear();
            // Please keep it false in the development environment for debugging css and js.
            bundles.UseCdn = false;
            BundleTable.EnableOptimizations = false;
        }
    }
}
