namespace Clunch

open System
open System.Linq
open Raven.Client
open Raven.Client.Document
open Raven.Client.Document.Async
open Raven.Client.Connection
open Raven.Client.Connection.Async

[<AutoOpen>]
module RavenExtensions =

    type IAsyncDocumentSession with 
        member x.AsyncSaveChanges() =
            x.SaveChangesAsync() |> Async.AwaitEmptyTask

        member x.AsyncLoad([<ParamArray>] ids: string[]) =
            x.LoadAsync(ids) |> Async.AwaitTask
        
        member x.AsyncLoad([<ParamArray>] ids: ValueType[]) =
            x.LoadAsync(ids) |> Async.AwaitTask

        member x.AsyncLoad(ids: seq<string>) =
            x.LoadAsync(ids) |> Async.AwaitTask

        member x.AsyncLoad(ids: seq<ValueType>) =
            x.LoadAsync(ids) |> Async.AwaitTask

        member x.AsyncLoad(id:string) =
            x.LoadAsync(id) |> Async.AwaitTask

        member x.AsyncLoad(id:ValueType) =
            x.LoadAsync(id) |> Async.AwaitTask
        
        member x.AsyncStore(entity) =
            x.StoreAsync(entity) |> Async.AwaitEmptyTask

    type IQueryable<'a> with
        member x.AsyncToList() =
            x.ToListAsync() |> Async.AwaitTask

