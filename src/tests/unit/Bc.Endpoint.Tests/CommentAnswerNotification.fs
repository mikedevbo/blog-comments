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

    [<Test>]
    let Handle_RegisterCommentNotification_ProperResult () =

        // Arrange
        let commentId = Guid.NewGuid()
        let userEmail = "sample_user_email"
        let articleFileName = "sample_file_name"
        let message = RegisterCommentNotification(commentId, userEmail, articleFileName)

        let policyData = CommentAnswerNotificationPolicy.PolicyData()
        let policy = getPolicy policyData

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length

        Assert.That(sentNumberOfMessages, Is.EqualTo(0))
        Assert.That(policyData.UserEmail, Is.EqualTo(userEmail))
        Assert.That(policyData.ArticleFileName, Is.EqualTo(articleFileName))

    [<TestCase(false)>]
    [<TestCase(true)>]
    let Handle_NotifyAboutCommentAnswer_ProperResult isApproved =

        // Arrange
        let commentId = Guid.NewGuid()
        let userEmail = "sample_user_email"
        let message = NotifyAboutCommentAnswer(commentId, isApproved)

        (logic.IsSendNotification message userEmail).Returns(isApproved) |> ignore

        let policyData = CommentAnswerNotificationPolicy.PolicyData(UserEmail = userEmail)
        let policy = getPolicy policyData

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length

        Assert.That((sentNumberOfMessages = 1), Is.EqualTo(isApproved))
        Assert.That(policy.Completed, Is.True)

