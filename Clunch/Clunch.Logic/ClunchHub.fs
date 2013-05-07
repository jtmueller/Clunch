namespace Clunch

open System
open System.Threading.Tasks
open Microsoft.AspNet.SignalR
open Autofac
open ImpromptuInterface.FSharp
open Clunch.Agents

type UserListModel = {
    ConnectionId: Guid
    Name: string
}

// https://code.google.com/p/autofac/wiki/SignalRIntegration

// Game state in memory. Users, characters, rooms, objects definitions in DB.
// MailboxProcessors to coordinate things.

// hub is one instance per call. Use room ID for group name.
type ClunchHub(parentScope:ILifetimeScope) =
    inherit Hub()
    let scope = parentScope.BeginLifetimeScope("httpRequest")
    // TODO: scope.Resolve<T>() for any services we need here

    let exec (action:Async<unit>) =
        Async.StartAsTask action :> Task

    let await = Async.AwaitEmptyTask

    member x.Send(message:string) : Task =
        try
            let name : string = x.Clients.Caller?name
            x.Clients.All?addMessage(name, message)
        with
        | :? NullReferenceException ->
            x.Clients.Caller?login()

    member x.Login(name:string) =
        async {
            let connId = Guid(x.Context.ConnectionId)
            let! users = UserAgent.connect connId name
            let userList =
                users 
                |> Seq.map (function KeyValue(cnId, name) -> { ConnectionId = cnId; Name = name })
                |> Seq.sortBy (fun u -> u.Name)
            x.Clients.Caller?name <- name
            
            do! [ await <| x.Clients.Caller?success (sprintf "Welcome, %s!" name)
                  await <| x.Clients.Caller?setUsers userList
                  await <| x.Clients.Others?addUser { ConnectionId = connId; Name = name } ]
                |> Async.Parallel |> Async.Ignore
        } |> exec

    override x.OnConnected() =
        x.Clients.Caller?login()

    override x.OnDisconnected() = 
        async {
            let connId = Guid(x.Context.ConnectionId)
            let! name = UserAgent.disconnect connId
            do! await <| x.Clients.All?removeUser { ConnectionId = connId; Name = (name |? "Unknown") }
        } |> exec

    override x.OnReconnected() =
        async {
            let connId = Guid(x.Context.ConnectionId)
            let! name = UserAgent.get connId
            match name with
            | Some n ->
                do! await <| x.Clients.Others?info (sprintf "User '%s' has reconnected." n)
            | None ->
                do! await <| x.Clients.Caller?login()
        } |> exec

    override x.Dispose(disposing) =
        base.Dispose(disposing)
        if disposing then
            scope.Dispose()

