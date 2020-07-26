module Bc.Endpoint.Tests.GitHubPullRequestVerification

open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Logic
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages
open Bc.Endpoint
open NServiceBus.Testing
open NSubstitute
open NUnit.Framework

let getContext() =
    TestableMessageHandlerContext()

module GitHubPullRequestVerificationPolicyTests =

    let logic = Substitute.For<IGitHubPullRequestVerificationPolicyLogic>()

    let getPolicy() =
        GitHubPullRequestVerificationPolicy(logic)

    [<Test>]
    let Handle_RequestCheckPullRequestStatus_ProperResult () =

        // Arrange
        let pullRequestUri = "Uri_123"
        let pullRequestStatus = PullRequestStatus.Open
        let etag = "ETag_123"
        let message = RequestCheckPullRequestStatus(pullRequestUri, etag)

        (logic.CheckPullRequestStatus pullRequestUri etag).Returns(ResponseCheckPullRequestStatus(pullRequestStatus, etag)) |> ignore

        let policy = getPolicy ()
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let repliedNumberOfMessages = context.RepliedMessages.Length
        let repliedMessage = context.RepliedMessages.[0].Message :?> ResponseCheckPullRequestStatus

        Assert.That(repliedNumberOfMessages, Is.EqualTo(1))
        Assert.That(repliedMessage.PullRequestStatus, Is.EqualTo(pullRequestStatus))
        Assert.That(repliedMessage.ETag, Is.EqualTo(etag))