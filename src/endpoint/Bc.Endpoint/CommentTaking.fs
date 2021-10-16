module Bc.CommentTaking

open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands
open Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
open NServiceBus
open NServiceBus.Logging

type CommentTakingPolicy() =
    interface IHandleMessages<TakeComment> with
        member this.Handle(message, context) =
            async {

                do! context.Send(RegisterComment(
                                    message.CommentId,
                                    message.UserName,
                                    message.UserWebsite,
                                    message.UserComment,
                                    message.ArticleFileName,
                                    message.CommentAddedDate)) |> Async.AwaitTask

                do! context.Send(RegisterCommentNotification(
                                    message.CommentId,
                                    message.UserEmail,
                                    message.ArticleFileName)) |> Async.AwaitTask

            } |> Async.StartAsTask :> Task
