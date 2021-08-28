namespace Bc.Web.Controllers

open System
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
open Bc.Web.Dto
open Microsoft.AspNetCore.Mvc
open NServiceBus

[<ApiController>]
[<Route("[controller]")>]
type CommentController(messageSession: IMessageSession) =
    inherit ControllerBase()

    [<HttpPost>]
    member this.Post(comment: Comment) =

        // // guess what for ;)
        if not (String.IsNullOrEmpty(comment.UserPhone)) then
           base.Ok()
        else
            let command = TakeComment(
                            Guid.NewGuid(),
                            comment.UserName,
                            comment.UserEmail,
                            comment.UserWebsite,
                            comment.UserComment,
                            comment.ArticleFileName,
                            DateTime.UtcNow)

            messageSession.Send(command) |> Async.AwaitTask |> ignore
            
            base.Ok()

    [<HttpGet>]
    member this.Get() =
        "Ready."