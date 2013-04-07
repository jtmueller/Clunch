namespace Clunch.Agents

open System
open Clunch

type UserMessage =
    | Connect of Guid * string
    | Disconnect of Guid * AsyncReplyChannel<string option>
    | Get of Guid * AsyncReplyChannel<string option>

module UserAgent =

    let private agent = Agent.Start(fun inbox ->
        let rec loop users = async {
            let! msg = inbox.Receive()
            match msg with
            | Connect (connId, name) ->
                let users = Map.add connId name users
                return! loop users
            | Disconnect (connId, reply) ->
                let user = Map.tryFind connId users
                let users = Map.remove connId users
                reply.Reply user
                return! loop users
            | Get (connId, reply) ->
                let user = Map.tryFind connId users
                reply.Reply user
                return! loop users
        }
        
        loop Map.empty
    )

    let connect connId name =
        agent.Post(Connect(connId, name))

    let disconnect connId =
        agent.PostAndAsyncReply(fun reply -> Disconnect(connId, reply))

    let get connId =
        agent.PostAndAsyncReply(fun reply -> Get(connId, reply))
