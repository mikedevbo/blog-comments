module Bc.Endpoint.Tests.CommentTaking

open System
open Bc.CommentTaking
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands
open Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
open Bc.Endpoint
open NServiceBus
open NServiceBus.Testing
open NUnit.Framework

let getContext() =
    TestableMessageHandlerContext()

module PolicyTests =

    let getPolicy() = Policy()

    [<Test>]
    let Handle_TakeComment_ProperResult () =

        // Arrange
        let commentId = Guid.NewGuid()
        let userName = "user_name"
        let userEmail = "user_email"
        let userWebsite = "user_website"
        let userComment = "user_comment"
        let articleFileName = "article_filename"
        let commentAddedDate = DateTime(2020, 7, 25)
        let message = TakeComment(
                                     commentId,
                                     userName,
                                     userEmail,
                                     userWebsite,
                                     userComment,
                                     articleFileName,
                                     commentAddedDate
                                 )

        let policy = getPolicy () :> IHandleMessages<TakeComment>
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessageRegisterComment = context.SentMessages.[0].Message :?> RegisterComment
        let sentMessageRegisterCommentNotification = context.SentMessages.[1].Message :?> RegisterCommentNotification

        Assert.That(sentNumberOfMessages, Is.EqualTo(2))
        Assert.That(sentMessageRegisterComment.CommentId, Is.EqualTo(commentId))
        Assert.That(sentMessageRegisterComment.UserName, Is.EqualTo(userName))
        Assert.That(sentMessageRegisterComment.UserWebsite, Is.EqualTo(userWebsite))
        Assert.That(sentMessageRegisterComment.UserComment, Is.EqualTo(userComment))
        Assert.That(sentMessageRegisterComment.ArticleFileName, Is.EqualTo(articleFileName))
        Assert.That(sentMessageRegisterComment.CommentAddedDate, Is.EqualTo(commentAddedDate))

        Assert.That(sentMessageRegisterCommentNotification.CommentId, Is.EqualTo(commentId))
        Assert.That(sentMessageRegisterCommentNotification.UserEmail, Is.EqualTo(userEmail))
        Assert.That(sentMessageRegisterCommentNotification.ArticleFileName, Is.EqualTo(articleFileName))
