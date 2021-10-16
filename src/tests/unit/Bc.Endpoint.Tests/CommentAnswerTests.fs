module Bc.Endpoint.Tests.CommentAnswer

open System
open Bc.CommentAnswer
open Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
open Bc.Contracts.Externals.Endpoint.CommentRegistration.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages
open Bc.Endpoint
open NServiceBus
open NServiceBus.Testing
open NSubstitute
open NUnit.Framework

let getContext() = TestableMessageHandlerContext()

module EventsSubscribingPolicyTests =

    let getPolicy () = EventsSubscribingPolicy()

    [<Test>]
    let Handle_CommentRegistered_ProperResult () =

        // Arrange
        let message = CommentRegistered(Guid.NewGuid(), " Uri_1234")
        let policy = getPolicy () :> IHandleMessages<CommentRegistered>
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let isSentProperMessage = context.SentMessages.[0].Message :? CheckCommentAnswer

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(isSentProperMessage, Is.EqualTo(true))

module PolicyTests =

    let getPolicy timeoutInSeconds data =
        CommentAnswerPolicy(timeoutInSeconds, Data=data)

    [<Test>]
    let Handle_CheckCommentAnswer_ProperResult () =

        // Arrange
        let commentUri = "Uri_123"
        let message = CheckCommentAnswer(Guid.NewGuid(), commentUri)

        let policyData = PolicyData()
        let policy = getPolicy 2.0 policyData :> IAmStartedByMessages<CheckCommentAnswer>

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessage = context.SentMessages.[0].Message :?> RequestCheckPullRequestStatus

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentMessage.PullRequestUri, Is.EqualTo(commentUri))
        Assert.That(sentMessage.ETag, Is.Empty)
        Assert.That(policyData.CommentUri, Is.EqualTo(commentUri))

    [<Test>]
    let Handle_ResponseCheckPullRequestStatusWhenStatusIsOpen_ProperResult () =

        // Arrange
        let pullRequestStatus = PullRequestStatus.Open
        let etag = "ETag_123"
        let message = ResponseCheckPullRequestStatus(pullRequestStatus, etag)

        let policyData = PolicyData(ETag = etag)
        let policy = getPolicy 2.0 policyData :> IHandleMessages<ResponseCheckPullRequestStatus>

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let requestNumberOfTimeouts = context.TimeoutMessages.Length
        let isRequestProperTimeout = context.TimeoutMessages.[0].Message :? TimeoutCheckCommentAnswer

        Assert.That(requestNumberOfTimeouts, Is.EqualTo(1))
        Assert.That(isRequestProperTimeout, Is.True)
        Assert.That(policyData.ETag, Is.EqualTo(etag))

    [<Test>]
    let Handle_ResponseCheckPullRequestStatusWhenStatusIsMerged_ProperResult () =

        // Arrange
        let pullRequestStatus = PullRequestStatus.Merged
        let commentId = Guid.NewGuid()
        let etag = "ETag_123"
        let message = ResponseCheckPullRequestStatus(pullRequestStatus, etag)

        let policyData = PolicyData(ETag = etag, CommentId = commentId)
        let policy = getPolicy 2.0 policyData

        let context = getContext ()

        // Act
        let handler = policy :> IHandleMessages<ResponseCheckPullRequestStatus>
        handler.Handle(message, context) |> ignore

        // Assert
        let publishedNumberOfMessages = context.PublishedMessages.Length
        let publishedMessage = context.PublishedMessages.[0].Message :?> CommentApproved

        Assert.That(publishedNumberOfMessages, Is.EqualTo(1))
        Assert.That(publishedMessage.CommentId, Is.EqualTo(commentId))
        Assert.That(policyData.ETag, Is.EqualTo(etag))
        Assert.That(policy.Completed, Is.EqualTo(true))

    [<Test>]
    let Handle_ResponseCheckPullRequestStatusWhenStatusIsClosed_ProperResult () =

        // Arrange
        let pullRequestStatus = PullRequestStatus.Closed
        let commentId = Guid.NewGuid()
        let etag = "ETag_123"
        let message = ResponseCheckPullRequestStatus(pullRequestStatus, etag)

        let policyData = PolicyData(ETag = etag, CommentId = commentId)
        let policy = getPolicy 2.0 policyData

        let context = getContext ()

        // Act
        let handler = policy :> IHandleMessages<ResponseCheckPullRequestStatus>
        handler.Handle(message, context) |> ignore

        // Assert
        let publishedNumberOfMessages = context.PublishedMessages.Length
        let publishedMessage = context.PublishedMessages.[0].Message :?> CommentRejected

        Assert.That(publishedNumberOfMessages, Is.EqualTo(1))
        Assert.That(publishedMessage.CommentId, Is.EqualTo(commentId))
        Assert.That(policyData.ETag, Is.EqualTo(etag))
        Assert.That(policy.Completed, Is.EqualTo(true))

    [<Test>]
    let Handle_TimeoutCheckCommentAnswer_ProperResult () =

        // Arrange
        let commentUri = "Uri_123"
        let etag = "ETag_123"
        let message = TimeoutCheckCommentAnswer()

        let policyData = PolicyData(CommentUri = commentUri, ETag = etag)
        let policy = getPolicy 2.0 policyData :> IHandleTimeouts<TimeoutCheckCommentAnswer>

        let context = getContext ()

        // Act
        policy.Timeout(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessage = context.SentMessages.[0].Message :?> RequestCheckPullRequestStatus

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentMessage.PullRequestUri, Is.EqualTo(commentUri))
        Assert.That(sentMessage.ETag, Is.EqualTo(etag))