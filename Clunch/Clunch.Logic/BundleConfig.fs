namespace Clunch

open System.Web.Optimization

type BundleConfig private () =
    static member RegisterBundles (bundles:BundleCollection) =
        bundles.IgnoreList.Ignore("*.map")
        bundles.IgnoreList.Ignore("*.coffee")

        ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js"
        ) |> bundles.Add

        ScriptBundle("~/bundles/angular").Include(
            "~/Scripts/angular.js",
            "~/Scripts/angular-bootstrap.js",
            //"~/Scripts/angular-loader.js",
            "~/Scripts/angular-resource.js",
            "~/Scripts/angular-sanitize.js",
            //"~/Scripts/angular-cookies.js",
            "~/Scripts/ui-bootstrap-tpls-{version}.js",
            "~/Scripts/i18n/angular-locale_en-us.js"
        ) |> bundles.Add
       
        ScriptBundle("~/bundles/extLibs").Include(
            "~/Scripts/jquery.signalR-{version}.js",
            "~/Scripts/underscore.js",   
            "~/Scripts/toastr.js"
        ) |> bundles.Add

        ScriptBundle("~/bundles/app").Include(
            "~/Scripts/app/services.js",
            "~/Scripts/app/app.js",
            "~/Scripts/app/controllers.js",
            "~/Scripts/app/filters.js",
            "~/Scripts/app/console.js",
            "~/Scripts/app/directives.js" // directives has to come last. maybe it's the embedded HTML, but in release/bundled mode, nothing after this file is included
        ) |> bundles.Add

        ScriptBundle("~/bundles/modernizr").Include(
            "~/Scripts/modernizr-*"
        ) |> bundles.Add

        StyleBundle("~/styles/bootstrap").Include(
            "~/Content/bootstrap.css",
            "~/Content/bootstrap-responsive.css"
        ) |> bundles.Add

        StyleBundle("~/styles/css").Include(
            "~/Content/toastr.css",
            "~/Content/toastr-responsive.css",
            "~/Content/Site.css"
        ) |> bundles.Add
