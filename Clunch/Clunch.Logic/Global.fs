namespace Clunch

open System
open System.Web
open System.Web.Mvc
open System.Web.Routing
open System.Web.Http
open System.Data.Entity
open System.Web.Optimization
open System.IO

type BundleConfig private () =
    static member RegisterBundles (bundles:BundleCollection) =
        ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-1*") 
        |> bundles.Add

        ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui*") 
        |> bundles.Add

        ScriptBundle("~/bundles/jqueryval").Include(
            "~/Scripts/jquery.unobtrusive*",
            "~/Scripts/jquery.validate*")
        |> bundles.Add
            
        ScriptBundle("~/bundles/extLibs").Include(
            "~/Scripts/underscore.js",   
            "~/Scripts/toastr.js",
            "~/Scripts/knockout-2.*")
        |> bundles.Add

        if Directory.Exists(HttpContext.Current.Server.MapPath "/Scripts/models") then                                     
            ScriptBundle("~/bundles/localApp").Include(
                "~/Scripts/app/main.js",
                "~/Scripts/app/utility.js",
                "~/Scripts/models/*.js",
                "~/Scripts/views/*.js",
                "~/Scripts/app/router.js")
            |> bundles.Add
        else                                     
            ScriptBundle("~/bundles/localApp").Include(
                "~/Scripts/app/main.js",
                "~/Scripts/app/utility.js",
                "~/Scripts/viewModels/*.js",
                "~/Scripts/app/router.js")
            |> bundles.Add

        ScriptBundle("~/bundles/sammy").IncludeDirectory("~/Scripts/Sammy", "*.js", true) 
        |> bundles.Add

        ScriptBundle("~/bundles/modernizr").Include(
            "~/Scripts/modernizr-*") 
        |> bundles.Add

        StyleBundle("~/Content/css").Include(
            "~/Content/*.css") 
        |> bundles.Add

        StyleBundle("~/Content/themes/base/css").Include(
            "~/Content/themes/base/jquery.ui.core.css",
            "~/Content/themes/base/jquery.ui.resizable.css",
            "~/Content/themes/base/jquery.ui.selectable.css",
            "~/Content/themes/base/jquery.ui.accordion.css",
            "~/Content/themes/base/jquery.ui.autocomplete.css",
            "~/Content/themes/base/jquery.ui.button.css",
            "~/Content/themes/base/jquery.ui.dialog.css",
            "~/Content/themes/base/jquery.ui.slider.css",
            "~/Content/themes/base/jquery.ui.tabs.css",
            "~/Content/themes/base/jquery.ui.datepicker.css",
            "~/Content/themes/base/jquery.ui.progressbar.css",
            "~/Content/themes/base/jquery.ui.theme.css")
        |> bundles.Add

open Autofac
open Autofac.Integration.Mvc
open Autofac.Integration.WebApi
open Raven.Client
open Raven.Client.Embedded
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
            // TODO: config to switch to remote server?
            //NonAdminHttp.EnsureCanListenToWhenInNonAdminContext 8080 
            let store = new EmbeddableDocumentStore(DataDirectory="App_Data", UseEmbeddedHttpServer=false)
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
