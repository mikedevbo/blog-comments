module Bc.Endpoint.Integration.Tests.GitHubPullRequestCreationSender

open System
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages
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
    let requestCreateGitHubPullRequest_send_noException () =

        // Arrange
        let commentId = Guid.NewGuid()
        let fileName = @"_posts/2018-05-27-test.md"
        let content = "new_comment"
        let addedDate = DateTime.UtcNow

        let message =
            RequestCreateGitHubPullRequest (
                commentId,
                fileName,
                content,
                addedDate
            )

        // Act
        sendMessage<RequestCreateGitHubPullRequest> message

        // Assert
        Assert.Pass()
