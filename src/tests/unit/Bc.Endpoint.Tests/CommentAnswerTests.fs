module Bc.Endpoint.Tests

open System
open Bc.Contracts.Externals.Endpoint.CommentRegistration.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Logic
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
        let etag = "ETag_123"
        let message = CheckCommentAnswer(Guid.NewGuid(), commentUri)

        let policyData = CommentAnswerPolicy.PolicyData(ETag = etag)
        let policy = getPolicy policyData

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore
        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentCheckPullRequestStatus = context.SentMessages.[0].Message :?> RequestCheckPullRequestStatus

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentCheckPullRequestStatus.PullRequestUri, Is.EqualTo(commentUri))
        Assert.That(sentCheckPullRequestStatus.ETag, Is.EqualTo(etag))
        Assert.That(policy.Data.CommentUri, Is.EqualTo(commentUri))