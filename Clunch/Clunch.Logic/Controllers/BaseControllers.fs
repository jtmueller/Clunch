namespace Clunch.Controllers

open System.Web
open System.Web.Mvc
open System.Web.Http
open System.Threading.Tasks
open Raven.Client
open Raven.Client.Document.Async
open Clunch

// Consider this: http://alexmg.com/post/2010/05/16/Introducing-Action-Injection-with-Autofac-ASPNET-MVC-Integration.aspx

[<AbstractClass>]
type RavenController(session:IDocumentSession) =
    inherit Controller()

    override x.OnActionExecuted filterContext =
        if not filterContext.IsChildAction && isNotNull session then
            use rs = session
            if isNull filterContext.Exception then
                rs.SaveChanges()

[<AbstractClass>]
type RavenApiController(session:IAsyncDocumentSession) =
    inherit ApiController()

    override x.ExecuteAsync(ctx, cancelToken) = 
        let task = base.ExecuteAsync(ctx, cancelToken)
        let action = async {
            let! result = Async.AwaitTask task
            if isNotNull session then
                use rs = session
                // to-do F# async wrapper methods
                do! rs.SaveChangesAsync() |> Async.AwaitIAsyncResult |> Async.Ignore
            return result
        }
        Async.StartAsTask(action, cancellationToken=cancelToken)
