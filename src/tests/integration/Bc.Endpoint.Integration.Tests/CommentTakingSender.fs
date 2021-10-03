module Bc.Endpoint.Integration.Tests.CommentTakingSender

open System
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
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
    let takeComment_send_noException () =

        // Arrange
        let commentId = Guid.NewGuid()
        let userName = "test_user"
        let userEmail = "test_user_email"
        let userWebsite = "test_user_website"
        let fileName = @"_posts/2018-05-27-test.md"
        let content = "new_comment"
        let addedDate = DateTime.UtcNow

        let message =
            TakeComment (
                commentId,
                userName,
                userEmail,
                userWebsite,
                fileName,
                content,
                addedDate
            )

        // Act
        sendMessage<TakeComment> message

        // Assert
        Assert.Pass()
