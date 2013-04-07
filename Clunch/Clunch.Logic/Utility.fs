[<AutoOpen>]
module Clunch.Utility

open System

type Agent<'a> = MailboxProcessor<'a>

module Async =
    let AwaitEmptyTask (t:System.Threading.Tasks.Task) =
        t |> Async.AwaitIAsyncResult |> Async.Ignore

let inline isNull x = Object.ReferenceEquals(x, null)
let inline isNotNull x = isNull x |> not

let inline (|?) arg defaultValue = defaultArg arg defaultValue
let inline (|?!) arg (defaultValue:Lazy<_>) =
    match arg with
    | Some x -> x
    | None -> defaultValue.Value

let inline (|??) (arg:Nullable<_>) defaultValue =
    if arg.HasValue then arg.Value else defaultValue

type Microsoft.FSharp.Control.Async with
    static member AwaitEmptyTask (t:System.Threading.Tasks.Task) =
        t |> Async.AwaitIAsyncResult |> Async.Ignore

type System.String with
    member s.IsEmpty = String.IsNullOrWhiteSpace s

type Autofac.Builder.IRegistrationBuilder<'a,'b,'c> with
    /// A shorter alternative to piping everything to |> ignore.
    member x.End() = ()

