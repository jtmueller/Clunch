namespace Clunch

open System
open System.Reflection
open Autofac
open Autofac.Builder
open Microsoft.AspNet.SignalR

[<AutoOpen>]
module RegistrationExtensions =
    
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
