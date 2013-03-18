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

type BundleConfig private () =
    static member RegisterBundles (bundles:BundleCollection) =
        bundles.IgnoreList.Ignore("*.map")

        ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-1*")
        |> bundles.Add

        ScriptBundle("~/bundles/angular").Include(
            "~/Scripts/angular.*",
            "~/Scripts/angular-bootstrap.*",
            "~/Scripts/angular-loader.*",
            "~/Scripts/angular-resource.*",
            "~/Scripts/angular-sanitize.*",
            "~/Scripts/angular-cookies.*",
            "~/Scripts/ui-bootstrap-tpls-*",
            "~/Scripts/i18n/angular-locale_en-us.js")
        |> bundles.Add
       
        ScriptBundle("~/bundles/extLibs").Include(
            "~/Scripts/underscore.js",   
            "~/Scripts/toastr.js")
        |> bundles.Add

        ScriptBundle("~/bundles/app").Include(
            "~/Scripts/app/app.js",
            "~/Scripts/app/controllers.js",
            "~/Scripts/app/directives.js",
            "~/Scripts/app/filters.js",
            "~/Scripts/app/services.js")
        |> bundles.Add

        ScriptBundle("~/bundles/modernizr").Include(
            "~/Scripts/modernizr-*") 
        |> bundles.Add

        StyleBundle("~/styles/bootstrap").Include(
            "~/Content/bootstrap/bootstrap.*",
            "~/Content/bootstrap/bootstrap-responsive.*")
        |> bundles.Add

        StyleBundle("~/styles/css").Include(
            "~/Content/toastr.css",
            "~/Content/toastr-responsive.css",
            "~/Content/Site.css") 
        |> bundles.Add


open Autofac
open Autofac.Integration.Mvc
open Autofac.Integration.WebApi
open Raven.Client
open Raven.Client.Document
open Raven.Client.Document.Async
open Raven.Database.Server

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
        routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" )
        routes.MapHttpRoute( "DefaultApi", "api/{controller}/{id}", 
            { id = RouteParameter.Optional } ) |> ignore
        routes.MapRoute("Default", "{controller}/{action}/{id}", 
            { controller = "Home"; action = "Index"; id = UrlParameter.Optional } ) |> ignore

    member this.Application_Start(sender:obj, e:EventArgs) =
        AreaRegistration.RegisterAllAreas()
        Global.RegisterRoutes RouteTable.Routes
        Global.RegisterGlobalFilters GlobalFilters.Filters
        BundleConfig.RegisterBundles BundleTable.Bundles
        AutofacConfig.Register GlobalConfiguration.Configuration
        ServiceStackTextFormatter.Register GlobalConfiguration.Configuration
