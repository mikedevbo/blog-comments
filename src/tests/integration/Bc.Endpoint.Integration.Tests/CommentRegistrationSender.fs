module Bc.Endpoint.Integration.Tests.CommentRegistrationSender

open System
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands
open NServiceBus
open NUnit.Framework

[<Ignore("Only for manual tests")>]
module Sender =

    let sendMessage<'T> (message:'T) =
        async {
            let! endpoint = EndpointFactory.getSenderEndpoint () |> Async.AwaitTask
            do! endpoint.Send(message) |> Async.AwaitTask
            do! endpoint.Stop() |> Async.AwaitTask
        } |> Async.RunSynchronously

    [<Test>]
    let registerComment_send_noException () =

        // Arrange
        let commentId = Guid.NewGuid()
        let userName = "test_user"
        let userWebsite = "test_user_website"
        let userComment = "new_comment"
        let articleFileName = @"_posts/2018-05-27-test.md"
        let addedDate = DateTime.UtcNow

        let message =
            RegisterComment (
                commentId,
                userName,
                userWebsite,
                userComment,
                articleFileName,
                addedDate
            )

        // Act
        sendMessage<RegisterComment> message

        // Assert
        Assert.Pass()