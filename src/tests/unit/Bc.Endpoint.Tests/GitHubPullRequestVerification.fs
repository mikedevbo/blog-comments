module Bc.Endpoint.Tests.GitHubPullRequestVerification

open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages
open Bc.Endpoint
open Bc.GitHubPullRequestVerification
open NServiceBus
open NServiceBus.Testing
open NSubstitute
open NUnit.Framework

let getContext() =
    TestableMessageHandlerContext()

module PolicyTests =

    let getPolicy() = Policy()

    [<Test>]
    let Handle_RequestCheckPullRequestStatus_ProperResult () =

        // Arrange
        let pullRequestUri = "Uri_123"
        let pullRequestStatus = PullRequestStatus.Open
        let etag = "ETag_123"
        let message = RequestCheckPullRequestStatus(pullRequestUri, etag)

        let policy = getPolicy () :> IHandleMessages<RequestCheckPullRequestStatus>
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let repliedNumberOfMessages = context.RepliedMessages.Length
        let repliedMessage = context.RepliedMessages.[0].Message :?> ResponseCheckPullRequestStatus

        Assert.That(repliedNumberOfMessages, Is.EqualTo(1))
        Assert.That(repliedMessage.PullRequestStatus, Is.EqualTo(pullRequestStatus))
        Assert.That(repliedMessage.ETag, Is.EqualTo(etag))