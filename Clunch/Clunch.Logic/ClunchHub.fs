namespace Clunch

open System
open Microsoft.AspNet.SignalR
open ImpromptuInterface.FSharp

type ClunchHub() =
    inherit Hub()

    member x.Send(message:string) : System.Threading.Tasks.Task =
        x.Clients.All?addMessage(message)

    override x.OnConnected() =
        x.Clients.All?addMessage(sprintf "Welcome %s!" x.Context.ConnectionId)
