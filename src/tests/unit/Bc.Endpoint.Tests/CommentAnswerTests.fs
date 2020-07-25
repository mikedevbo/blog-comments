module Bc.Endpoint.Tests

open System
open Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
open Bc.Contracts.Externals.Endpoint.CommentRegistration.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Logic
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages
open NServiceBus.Testing
open NSubstitute
open NUnit.Framework

let getContext() =
    TestableMessageHandlerContext()

module CommentAnswerEventsSubscribingPolicyTests =

    let getPolicy() =
        CommentAnswerEventsSubscribingPolicy()

    [<Test>]
    let Handle_CommentRegistered_ProperResult () =

        // Arrange
        let message = CommentRegistered(Guid.NewGuid(), " Uri_1234")
        let policy = getPolicy ()
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let isSentCheckCommentAnswer = context.SentMessages.[0].Message :? CheckCommentAnswer

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(isSentCheckCommentAnswer, Is.EqualTo(true))

module CommentAnswerPolicyTests =

    let logic = Substitute.For<ICommentAnswerPolicyLogic>()

    let getPolicy data =
        CommentAnswerPolicy(logic, Data = data)

    [<Test>]
    let Handle_CheckCommentAnswer_ProperResult () =

        // Arrange
        let commentUri = "Uri_123"
        let message = CheckCommentAnswer(Guid.NewGuid(), commentUri)

        let policyData = CommentAnswerPolicy.PolicyData()
        let policy = getPolicy policyData

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentCheckPullRequestStatus = context.SentMessages.[0].Message :?> RequestCheckPullRequestStatus

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentCheckPullRequestStatus.PullRequestUri, Is.EqualTo(commentUri))
        Assert.That(sentCheckPullRequestStatus.ETag, Is.Null)
        Assert.That(policy.Data.CommentUri, Is.EqualTo(commentUri))

    [<Test>]
    let Handle_ResponseCheckPullRequestStatusWhenStatusIsOpen_ProperResult () =

        // Arrange
        let pullRequestStatus = PullRequestStatus.Open
        let etag = "ETag_123"
        let message = ResponseCheckPullRequestStatus(pullRequestStatus, etag)

        let policyData = CommentAnswerPolicy.PolicyData(ETag = etag)
        let policy = getPolicy policyData

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let requestNumberOfTimeouts = context.TimeoutMessages.Length
        let isRequestTimeoutCheckCommentAnswer = context.TimeoutMessages.[0].Message :? TimeoutCheckCommentAnswer

        Assert.That(requestNumberOfTimeouts, Is.EqualTo(1))
        Assert.That(isRequestTimeoutCheckCommentAnswer, Is.True)
        Assert.That(policyData.ETag, Is.EqualTo(etag))

    [<Test>]
    let Handle_ResponseCheckPullRequestStatusWhenStatusIsMerged_ProperResult () =

        // Arrange
        let pullRequestStatus = PullRequestStatus.Merged
        let commentId = Guid.NewGuid()
        let etag = "ETag_123"
        let message = ResponseCheckPullRequestStatus(pullRequestStatus, etag)

        let policyData = CommentAnswerPolicy.PolicyData(ETag = etag, CommentId = commentId)
        let policy = getPolicy policyData

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let publishedNumberOfMessages = context.PublishedMessages.Length
        let publishedCommentApproved = context.PublishedMessages.[0].Message :?> CommentApproved

        Assert.That(publishedNumberOfMessages, Is.EqualTo(1))
        Assert.That(publishedCommentApproved.CommentId, Is.EqualTo(commentId))
        Assert.That(policyData.ETag, Is.EqualTo(etag))

    [<Test>]
    let Handle_ResponseCheckPullRequestStatusWhenStatusIsClosed_ProperResult () =

        // Arrange
        let pullRequestStatus = PullRequestStatus.Closed
        let commentId = Guid.NewGuid()
        let etag = "ETag_123"
        let message = ResponseCheckPullRequestStatus(pullRequestStatus, etag)

        let policyData = CommentAnswerPolicy.PolicyData(ETag = etag, CommentId = commentId)
        let policy = getPolicy policyData

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let publishedNumberOfMessages = context.PublishedMessages.Length
        let publishedCommentApproved = context.PublishedMessages.[0].Message :?> CommentRejected

        Assert.That(publishedNumberOfMessages, Is.EqualTo(1))
        Assert.That(publishedCommentApproved.CommentId, Is.EqualTo(commentId))
        Assert.That(policyData.ETag, Is.EqualTo(etag))