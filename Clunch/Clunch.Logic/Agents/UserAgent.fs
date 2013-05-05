namespace Clunch.Agents

open System
open Clunch

type UserMessage =
    | Connect of Guid * string * AsyncReplyChannel<Map<Guid, string>>
    | Disconnect of Guid * AsyncReplyChannel<string option>
    | Get of Guid * AsyncReplyChannel<string option>
    | GetAll of AsyncReplyChannel<Map<Guid, string>>

module UserAgent =

    let private agent = Agent.Start(fun inbox ->
        let rec loop users = async {
            let! msg = inbox.Receive()
            match msg with
            | Connect (connId, name, reply) ->
                let users = Map.add connId name users
                reply.Reply users
                return! loop users
            | Disconnect (connId, reply) ->
                let user = Map.tryFind connId users
                let users = 
                    match user with
                    | None   -> users
                    | Some _ -> Map.remove connId users
                reply.Reply user
                return! loop users
            | Get (connId, reply) ->
                let user = Map.tryFind connId users
                reply.Reply user
                return! loop users
            | GetAll reply ->
                reply.Reply users
                return! loop users
        }
        
        loop Map.empty
    )

    let connect connId name =
        agent.PostAndAsyncReply(fun reply -> Connect(connId, name, reply))

    let disconnect connId =
        agent.PostAndAsyncReply(fun reply -> Disconnect(connId, reply))

    let get connId =
        agent.PostAndAsyncReply(fun reply -> Get(connId, reply))
