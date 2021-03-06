﻿namespace Clunch

open System.Web.Optimization

type BundleConfig private () =
    static member RegisterBundles (bundles:BundleCollection) =
        bundles.IgnoreList.Ignore("*.map")
        bundles.IgnoreList.Ignore("*.coffee")
        bundles.UseCdn <- true

        ScriptBundle("~/bundles/jquery", "//ajax.googleapis.com/ajax/libs/jquery/2.0.0/jquery.min.js").Include(
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
            "~/AppScripts/services.js",
            "~/AppScripts/app.js",
            "~/AppScripts/controllers.js",
            "~/AppScripts/filters.js",
            "~/AppScripts/console.js",
            "~/AppScripts/directives.js" // directives has to come last. maybe it's the embedded HTML, but in release/bundled mode, nothing after this file is included
        ) |> bundles.Add

        ScriptBundle("~/bundles/modernizr").Include(
            "~/Scripts/modernizr-*"
        ) |> bundles.Add

        StyleBundle("~/styles/css").Include(
            "~/Css/Site.css"
        ) |> bundles.Add
