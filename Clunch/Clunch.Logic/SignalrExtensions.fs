namespace Clunch

open System
open System.Reflection
open Autofac
open Autofac.Builder
open Microsoft.AspNet.SignalR

[<AutoOpen>]
module SignalrExtensions =
    
    type ContainerBuilder with
        /// Register types that implement <see cref="IHub"/> in the provided assemblies.
        member builder.RegisterHubs([<ParamArray>] assemblies: Assembly[]) =
            builder.RegisterAssemblyTypes(assemblies)
                .Where(fun t -> typeof<Hubs.IHub>.IsAssignableFrom(t))
                .ExternallyOwned()

/// Autofac implementation of the <see cref="IDependencyResolver"/> interface.
type AutofacSignalrDependencyResolver(lifetimeScope:ILifetimeScope) =
    inherit DefaultDependencyResolver()
    do if isNull lifetimeScope then nullArg "lifetimeScope"

    /// Gets the Autofac implementation of the dependency resolver. 
    static member Current =
        GlobalHost.DependencyResolver :?> AutofacSignalrDependencyResolver

    /// Gets the <see cref="ILifetimeScope"/> that was provided to the constructor.
    member x.LifetimeScope = lifetimeScope

    /// Get a single instance of a service.
    override x.GetService(serviceType) =
        let service = lifetimeScope.ResolveOptional(serviceType)
        if isNull service
        then base.GetService(serviceType)
        else service

    /// Gets all available instances of a service.
    override x.GetServices(serviceType) =
        let enumerableServiceType = typedefof<seq<_>>.MakeGenericType(serviceType)
        let instance = lifetimeScope.Resolve(enumerableServiceType) :?> seq<obj>

        if Seq.isEmpty instance
        then base.GetServices(serviceType)
        else instance

open Microsoft.AspNet.SignalR.Hubs
open System.Web
open Elmah

type ElmahPipelineModule() =
    inherit HubPipelineModule()

    static let raiseErrorSignal ex =
        let context = HttpContext.Current
        if isNull context then
            false
        else
            let application = HttpContext.Current.ApplicationInstance
            if isNull application then
                false
            else
                let signal = ErrorSignal.Get(application)
                if isNull signal then
                    false
                else
                    signal.Raise(ex, context)
                    true

    static let logException ex =
        let context = HttpContext.Current
        ErrorLog.GetDefault(context).Log(Error(ex, context)) |> ignore

    override x.OnIncomingError(ex, context) =
        let ex =
            match ex with
            | :? TargetInvocationException as tex when isNotNull tex.InnerException ->
                tex.InnerException
            | :? AggregateException as aex when isNotNull aex.InnerException ->
                aex.InnerException
            | _ ->
                ex

        if not (raiseErrorSignal ex)
        then logException ex
