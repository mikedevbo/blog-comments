module Bc.Endpoint.Tests

open System
open Bc.Contracts.Externals.Endpoint.CommentRegistration.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands
open NServiceBus.Testing
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
        policy.Handle(message, context) |> ignore;
        
        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentCheckCommentAnswerMessage = context.SentMessages.[0].Message :? CheckCommentAnswer
        
        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentCheckCommentAnswerMessage, Is.EqualTo(true))