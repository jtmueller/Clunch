namespace Clunch

open System
open System.Threading.Tasks
open Microsoft.AspNet.SignalR
open Autofac
open ImpromptuInterface.FSharp

// https://code.google.com/p/autofac/wiki/SignalRIntegration

// Game state in memory. Users, characters, rooms, objects definitions in DB.
// MailboxProcessors to coordinate things.

// hub is one instance per call. Use room ID for group name.
type ClunchHub(lifetimeScope:ILifetimeScope) =
    inherit Hub()
    let scope = lifetimeScope.BeginLifetimeScope("httpRequest")
    // TODO: scope.Resolve<T>() for any services we need here

    member x.Send(message:string) : Task =
        x.Clients.All?addMessage(message)

    override x.OnConnected() =
        x.Clients.Caller?success(sprintf "Welcome %s!" x.Context.ConnectionId, "Connected")
        x.Clients.Others?info(sprintf "Client connected: %s" x.Context.ConnectionId)

    override x.OnDisconnected() =
        x.Clients.Others?warning(sprintf "Client %s has disconnected." x.Context.ConnectionId)

    override x.Dispose(disposing) =
        if disposing then
            scope.Dispose()

