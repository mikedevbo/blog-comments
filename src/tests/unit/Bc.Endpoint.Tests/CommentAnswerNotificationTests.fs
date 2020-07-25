module Bc.Endpoint.Tests.CommentAnswerNotification

open System
open Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands
open Bc.Endpoint
open NServiceBus.Testing
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


