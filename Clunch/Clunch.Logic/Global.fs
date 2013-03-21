namespace Clunch

open System
open System.Configuration
open System.Web
open System.Web.Mvc
open System.Web.Routing
open System.Web.Http
open System.Data.Entity
open System.Web.Optimization
open System.IO
open Microsoft.AspNet.SignalR

open Autofac
open Autofac.Integration.Mvc
open Autofac.Integration.WebApi
open Raven.Client
open Raven.Client.Document
open Raven.Client.Document.Async
open Raven.Database.Server

type BundleConfig private () =
    static member RegisterBundles (bundles:BundleCollection) =
        bundles.IgnoreList.Ignore("*.map")
        bundles.IgnoreList.Ignore("*.coffee")

        ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js")
        |> bundles.Add

        ScriptBundle("~/bundles/angular").Include(
            "~/Scripts/angular.js",
            "~/Scripts/angular-bootstrap.js",
            "~/Scripts/angular-loader.js",
            "~/Scripts/angular-resource.js",
            "~/Scripts/angular-sanitize.js",
            "~/Scripts/angular-cookies.js",
            "~/Scripts/ui-bootstrap-tpls-*",
            "~/Scripts/i18n/angular-locale_en-us.js")
        |> bundles.Add
       
        ScriptBundle("~/bundles/extLibs").Include(
            "~/Scripts/jquery.signalR-{version}.js",
            "~/Scripts/underscore.js",   
            "~/Scripts/toastr.js")
        |> bundles.Add

        ScriptBundle("~/bundles/app").Include(
            "~/Scripts/app/services.js",
            "~/Scripts/app/app.js",
            "~/Scripts/app/controllers.js",
            "~/Scripts/app/filters.js",
            "~/Scripts/app/directives.js" // directives has to come last. maybe it's the embedded HTML, but in release/bundled mode, nothing after this file is included
        ) |> bundles.Add

        ScriptBundle("~/bundles/modernizr").Include(
            "~/Scripts/modernizr-*") 
        |> bundles.Add

        StyleBundle("~/styles/bootstrap").Include(
            "~/Content/bootstrap.css",
            "~/Content/bootstrap-responsive.css")
        |> bundles.Add

        StyleBundle("~/styles/css").Include(
            "~/Content/toastr.css",
            "~/Content/toastr-responsive.css",
            "~/Content/Site.css") 
        |> bundles.Add


type AutofacConfig private () =
    static member Register config =
        let builder = Autofac.ContainerBuilder()

        builder.RegisterControllers(typeof<Controllers.RavenController>.Assembly) |> ignore
        builder.RegisterApiControllers(typeof<Controllers.RavenApiController>.Assembly) |> ignore

        builder.RegisterType<ExtensibleActionInvoker>().As<IActionInvoker>() |> ignore
        builder.RegisterWebApiFilterProvider(config)

        builder.Register<IDocumentStore>(fun c ->
            let store = new DocumentStore(ConnectionStringName="Clunch")
            store.Initialize()
        ).SingleInstance() |> ignore

        builder.Register<IDocumentSession>(fun (c:IComponentContext) -> c.Resolve<IDocumentStore>().OpenSession())
            .InstancePerHttpRequest() |> ignore

        builder.Register<IAsyncDocumentSession>(fun (c:IComponentContext) -> c.Resolve<IDocumentStore>().OpenAsyncSession())
            .InstancePerHttpRequest() |> ignore

        builder.RegisterType<ServiceStackSerializer>().As<Json.IJsonSerializer>().SingleInstance() |> ignore

        let container = builder.Build()

        DependencyResolver.SetResolver(new AutofacDependencyResolver(container))
        config.DependencyResolver <- new AutofacWebApiDependencyResolver(container)


type Route = { controller: string; action: string; id: UrlParameter }
type ApiRoute = { id: RouteParameter }

type Global() =
    inherit System.Web.HttpApplication()

    static let mutable docStore : IDocumentStore = null
        
    /// The shared RavenDB Document Store.
    static member DocumentStore = docStore

    static member RegisterGlobalFilters (filters:GlobalFilterCollection) =
        filters.Add(new HandleErrorAttribute())

    static member RegisterRoutes(routes:RouteCollection) =
        routes.MapHubs() |> ignore

        routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" )
        routes.MapHttpRoute( "DefaultApi", "api/{controller}/{id}", 
            { id = RouteParameter.Optional } ) |> ignore
        routes.MapRoute("Default", "{controller}/{action}/{id}", 
            { controller = "Home"; action = "Index"; id = UrlParameter.Optional } ) |> ignore

    member this.Application_Start(sender:obj, e:EventArgs) =
        AutofacConfig.Register GlobalConfiguration.Configuration
        Global.RegisterRoutes RouteTable.Routes
        AreaRegistration.RegisterAllAreas()
        Global.RegisterGlobalFilters GlobalFilters.Filters
        BundleConfig.RegisterBundles BundleTable.Bundles
        ServiceStackTextFormatter.Register GlobalConfiguration.Configuration
