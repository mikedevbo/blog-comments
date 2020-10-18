module Bc.Endpoint.Tests.CommentAnswerNotification

open System
open Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Logic
open Bc.Endpoint
open NServiceBus.Testing
open NSubstitute
open NUnit.Framework

let getContext() =
    TestableMessageHandlerContext()

module CommentAnswerNotificationEventSubscribingPolicyTests =

    let getPolicy() =
        CommentAnswerNotificationEventSubscribingPolicy()

    [<Test>]
    let Handle_CommentApproved_ProperResult () =

        // Arrange
        let commentId = Guid.NewGuid()
        let message = CommentApproved(commentId)
        let policy = getPolicy ()
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessage = context.SentMessages.[0].Message :?> NotifyAboutCommentAnswer

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentMessage.CommentId, Is.EqualTo(commentId))
        Assert.That(sentMessage.IsApproved, Is.True)

    [<Test>]
    let Handle_CommentRejected_ProperResult () =

        // Arrange
        let commentId = Guid.NewGuid()
        let message = CommentRejected(commentId)
        let policy = getPolicy ()
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessage = context.SentMessages.[0].Message :?> NotifyAboutCommentAnswer

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentMessage.CommentId, Is.EqualTo(commentId))
        Assert.That(sentMessage.IsApproved, Is.False)

module CommentAnswerNotificationPolicyTests =

    let logic = Substitute.For<ICommentAnswerNotificationPolicyLogic>()

    let getPolicy data =
        CommentAnswerNotificationPolicy(logic, Data = data)

    [<TestCase(false, false, false, false)>]
    [<TestCase(false, true, false, false)>]
    [<TestCase(true, false, false, true)>]
    [<TestCase(true, true, true, true)>]
    let Handle_RegisterCommentNotification_ProperResult
        isNotificationReadyToSend
        isCommentApproved
        expectedMessageSent
        expectedSagaCompleted =

        // Arrange
        let commentId = Guid.NewGuid()
        let userEmail = "sample_user_email"
        let articleFileName = "sample_file_name"
        let message = RegisterCommentNotification(commentId, userEmail, articleFileName)

        let policyData = CommentAnswerNotificationPolicy.PolicyData(
                                                                    IsNotificationReadyToSend = isNotificationReadyToSend,
                                                                    IsCommentApproved = isCommentApproved
                                                                   )

        (logic.IsSendNotification (Arg.Any<bool> ()) (Arg.Any<string> ())).Returns(isCommentApproved) |> ignore

        let policy = getPolicy policyData
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        Assert.That(policyData.UserEmail, Is.EqualTo(userEmail))
        Assert.That(policyData.ArticleFileName, Is.EqualTo(articleFileName))
        Assert.That(policyData.IsNotificationRegistered, Is.True)

        let isMessageSent = context.SentMessages.Length > 0
        Assert.That(isMessageSent, Is.EqualTo(expectedMessageSent))
        Assert.That(policy.Completed, Is.EqualTo(expectedSagaCompleted))

    [<TestCase(false, false, false, false)>]
    [<TestCase(false, true, false, false)>]
    [<TestCase(true, false, false, true)>]
    [<TestCase(true, true, true, true)>]
    let Handle_NotifyAboutCommentAnswer_ProperResult
        isNotificationRegistered
        isCommentApproved
        expectedMessageSent
        expectedSagaCompleted =

        // Arrange
        let commentId = Guid.NewGuid()
        let message = NotifyAboutCommentAnswer(commentId, isCommentApproved)

        let policyData = CommentAnswerNotificationPolicy.PolicyData(IsNotificationRegistered = isNotificationRegistered)

        (logic.IsSendNotification (Arg.Any<bool> ()) (Arg.Any<string> ())).Returns(isCommentApproved) |> ignore

        let policy = getPolicy policyData
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        Assert.That(policyData.IsCommentApproved, Is.EqualTo(isCommentApproved))
        Assert.That(policyData.IsNotificationReadyToSend, Is.True)

        let isMessageSent = context.SentMessages.Length > 0
        Assert.That(isMessageSent, Is.EqualTo(expectedMessageSent))
        Assert.That(policy.Completed, Is.EqualTo(expectedSagaCompleted))
