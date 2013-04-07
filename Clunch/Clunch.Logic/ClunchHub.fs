namespace Clunch

open System
open System.Threading.Tasks
open Microsoft.AspNet.SignalR
open Autofac
open ImpromptuInterface.FSharp
open Clunch.Agents

// https://code.google.com/p/autofac/wiki/SignalRIntegration

// Game state in memory. Users, characters, rooms, objects definitions in DB.
// MailboxProcessors to coordinate things.

// hub is one instance per call. Use room ID for group name.
type ClunchHub(lifetimeScope:ILifetimeScope) =
    inherit Hub()
    let scope = lifetimeScope.BeginLifetimeScope("httpRequest")
    // TODO: scope.Resolve<T>() for any services we need here

    member x.Send(message:string) : Task =
        try
            let name : string = x.Clients.Caller?name
            x.Clients.All?addMessage(name, message)
        with
        | :? NullReferenceException ->
            x.Clients.Caller?login()

    member x.Login(name:string) : Task =
        let connId = Guid(x.Context.ConnectionId)
        UserAgent.connect connId name
        x.Clients.Caller?name <- name
        x.Clients.Caller?success(sprintf "Welcome, %s!" name)
        x.Clients.Others?info(sprintf "User '%s' has connected." name)

    override x.OnConnected() =
        x.Clients.Caller?login()

    override x.OnDisconnected() = 
        async {
            let connId = Guid(x.Context.ConnectionId)
            let! name = UserAgent.disconnect connId
            x.Clients.Others?warning(sprintf "User '%s' has disconnected." (name |? "Unknown"))
        } |> Async.StartAsTask :> Task

    override x.OnReconnected() =
        async {
            let connId = Guid(x.Context.ConnectionId)
            let! name = UserAgent.get connId
            match name with
            | Some n ->
                x.Clients.Others?info(sprintf "User '%s' has reconnected." n)
            | None ->
                x.Clients.Caller?login()
        } |> Async.StartAsTask :> Task

    override x.Dispose(disposing) =
        base.Dispose(disposing)
        if disposing then
            scope.Dispose()

