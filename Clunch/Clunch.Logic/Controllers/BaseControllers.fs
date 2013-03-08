namespace FsWeb.Controllers

open System.Web
open System.Web.Mvc
open System.Web.Http
open System.Threading.Tasks
open Raven.Client
open Raven.Client.Document.Async
open FsWeb

[<AbstractClass>]
type RavenController() =
    inherit Controller()

    let session = lazy ( FsWeb.Global.DocumentStore.OpenSession() )

    /// The RavenDB document session for the current request.
    member x.RavenSession = session.Value

    override x.OnActionExecuted filterContext =
        if not filterContext.IsChildAction && session.IsValueCreated then
            use rs = session.Value
            if isNull filterContext.Exception && isNotNull rs then
                rs.SaveChanges()

[<AbstractClass>]
type RavenApiController() =
    inherit ApiController()

    let session = lazy ( FsWeb.Global.DocumentStore.OpenAsyncSession() )

    /// The RavenDB document session for the current request.
    member x.RavenSession = session.Value

    override x.ExecuteAsync(ctx, cancelToken) = 
        let task = base.ExecuteAsync(ctx, cancelToken)
        let action = async {
            let! result = Async.AwaitTask task
            if session.IsValueCreated then
                use rs = session.Value
                if isNotNull rs then
                    do! rs.SaveChangesAsync() |> (Async.AwaitIAsyncResult >> Async.Ignore)
            return result
        }
        Async.StartAsTask(action, cancellationToken=cancelToken)
