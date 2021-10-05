module Bc.Endpoint.Integration.Tests.CommentAnswerNotification

open System
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands
open NServiceBus
open NUnit.Framework

[<Ignore("Only for manual tests")>]
module Sender =

    let commentId = Guid.Parse("9a3a0fdc-3a5a-4862-845c-09792d2b6f9a")

    let sendMessage<'T> (message:'T) =
        async {
            let! endpoint = EndpointFactory.getSenderEndpoint () |> Async.AwaitTask
            do! endpoint.Send(message) |> Async.AwaitTask
            do! endpoint.Stop() |> Async.AwaitTask
        } |> Async.RunSynchronously

    [<Test>]
    let registerCommentNotification_send_noException () =

        // Arrange
        let userEmail = "test@test.com"
        let articleFileName = @"_posts/2018-05-27-test.md"
        let message = RegisterCommentNotification(commentId, userEmail, articleFileName)

        // Act
        sendMessage<RegisterCommentNotification> message

        // Assert
        Assert.Pass()

    [<Test>]
    let notifyAboutCommentAnswer_send_noException () =

        // Arrange
        let isApproved = true
        let message = NotifyAboutCommentAnswer(commentId, isApproved)

        // Act
        sendMessage<NotifyAboutCommentAnswer> message

        // Assert
        Assert.Pass()