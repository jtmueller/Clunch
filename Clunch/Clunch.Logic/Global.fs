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

open Autofac
open Autofac.Integration
open Autofac.Integration.Mvc
open Autofac.Integration.WebApi
open Raven.Client
open Raven.Client.Document
open Raven.Client.Document.Async
open Raven.Database.Server
open Elmah.Contrib.WebApi
open Microsoft.AspNet.SignalR
open Newtonsoft.Json
open Newtonsoft.Json.Converters
open Newtonsoft.Json.Serialization

type RavenConverter = Raven.Imports.Newtonsoft.Json.JsonConverter

type AutofacConfig private () =
    static member Register (config:HttpConfiguration) =
        let builder = Autofac.ContainerBuilder()

        builder.RegisterControllers(typeof<Controllers.RavenController>.Assembly).End()
        builder.RegisterApiControllers(typeof<Controllers.RavenApiController>.Assembly).End()

        builder.RegisterType<ExtensibleActionInvoker>().As<IActionInvoker>().End()
        builder.RegisterWebApiFilterProvider(config)

        builder.Register<IDocumentStore>(fun c ->
            let store = new DocumentStore(ConnectionStringName="RavenDB")
            store.Conventions.CustomizeJsonSerializer <-
                fun serializer ->
                    [ Raven.Converters.OptionConverter()     :> RavenConverter
                      Raven.Converters.ListConverter()       :> RavenConverter
                      Raven.Converters.TupleArrayConverter() :> RavenConverter
                      Raven.Converters.UnionTypeConverter()  :> RavenConverter ]
                    |> List.iter serializer.Converters.Add
                    
            store.Initialize()
        ).SingleInstance().End()

        builder.Register<IDocumentSession>(fun (c:IComponentContext) -> c.Resolve<IDocumentStore>().OpenSession())
            .InstancePerHttpRequest().End()

        builder.Register<IAsyncDocumentSession>(fun (c:IComponentContext) -> c.Resolve<IDocumentStore>().OpenAsyncSession())
            .InstancePerHttpRequest().End()

        builder.Register(fun _ -> Json.JsonNetSerializer(config.Formatters.JsonFormatter.SerializerSettings))
            .As<Json.IJsonSerializer>()
            .SingleInstance().End()

        builder.RegisterHubs(typeof<ClunchHub>.Assembly).End()

        let container = builder.Build()

        DependencyResolver.SetResolver (new Mvc.AutofacDependencyResolver(container))
        config.DependencyResolver     <- new WebApi.AutofacWebApiDependencyResolver(container)
        GlobalHost.DependencyResolver <- new AutofacSignalrDependencyResolver(container)

type Route = { controller: string; action: string; id: UrlParameter }
type ApiRoute = { id: RouteParameter }

type Global() =
    inherit System.Web.HttpApplication()

    static member private ConfigureJson (config:HttpConfiguration) =
        let jsonSettings = config.Formatters.JsonFormatter.SerializerSettings
        jsonSettings.ContractResolver <- DefaultContractResolver(SerializeCompilerGeneratedMembers=false)
        [ OptionConverter()     :> JsonConverter
          ListConverter()       :> JsonConverter
          TupleArrayConverter() :> JsonConverter
          UnionTypeConverter()  :> JsonConverter ]
        |> List.iter jsonSettings.Converters.Add

    static member private RegisterGlobalFilters (filters:GlobalFilterCollection) =
        filters.Add(new HandleErrorAttribute())

    static member private RegisterRoutes(routes:RouteCollection) =
        routes.MapHubs() |> ignore

        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.IgnoreRoute("robots.txt")
        routes.IgnoreRoute("sitemap")
        routes.IgnoreRoute("sitemap.gz")
        routes.IgnoreRoute("sitemap.xml")
        routes.IgnoreRoute("sitemap.xml.gz")
        routes.IgnoreRoute("google_sitemap.xml")
        routes.IgnoreRoute("google_sitemap.xml.gz")
        routes.IgnoreRoute("favicon.ico")
        routes.IgnoreRoute("apple-touch-icon.png")
        routes.IgnoreRoute("apple-touch-icon-precomposed.png")

        routes.MapHttpRoute( "DefaultApi", "api/{controller}/{id}", 
            { id = RouteParameter.Optional } ) |> ignore
        routes.MapRoute("Default", "{controller}/{action}/{id}", 
            { controller = "Home"; action = "Index"; id = UrlParameter.Optional } ) |> ignore

    member this.Application_Start(sender:obj, e:EventArgs) =
        Global.ConfigureJson GlobalConfiguration.Configuration
        AutofacConfig.Register GlobalConfiguration.Configuration
        Global.RegisterRoutes RouteTable.Routes
        AreaRegistration.RegisterAllAreas()
        Global.RegisterGlobalFilters GlobalFilters.Filters
        BundleConfig.RegisterBundles BundleTable.Bundles
        GlobalConfiguration.Configuration.Filters.Add(ElmahHandleErrorApiAttribute())
        GlobalHost.HubPipeline.AddModule (ElmahPipelineModule()) |> ignore

