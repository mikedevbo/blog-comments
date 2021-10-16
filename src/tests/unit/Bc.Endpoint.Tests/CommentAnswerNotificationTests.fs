module Bc.Endpoint.Tests.CommentAnswerNotification

open System
open Bc.CommentAnswerNotification
open Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands
open Bc.Endpoint
open NServiceBus
open NServiceBus.Testing
open NSubstitute
open NUnit.Framework

let getContext() = TestableMessageHandlerContext()

module LogicTests =

    [<TestCase(false, false, false)>]
    [<TestCase(false, true, false)>]
    [<TestCase(true, false, false)>]
    [<TestCase(true, true, true)>]
    let isNotificationReady_dependsOnState_readyOrNot
        isNotificationRegistered
        isNotificationReadyToSend
        expectedResult =

        // Arrange

        // Act
        let result = Logic.isNotificationReady isNotificationRegistered isNotificationReadyToSend

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult))

    [<TestCase(false, "", false)>]
    [<TestCase(false, "test@test.test", false)>]
    [<TestCase(true, "", false)>]
    [<TestCase(true, "test@test.test", true)>]
    let isSendNotification_dependsOnState_rendOrNot
        isCommentApproved
        userEmail
        expectedResult =

        // Arrange

        // Act
        let result = Logic.isSendNotification isCommentApproved userEmail

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult))

    [<TestCase("_posts/2018-02-10-title_subtitle_something.md", "blogDomainName", "Sprawdź - blogDomainName/2018/02/10/title_subtitle_something.html")>]
    [<TestCase("_posts/2018-02-10-title-subtitle-something.md", "blogDomainName", "Sprawdź - blogDomainName/2018/02/10/title-subtitle-something.html")>]
    let getBody_dependsOnFileName_FormatBody
        fileName
        blogDomainName
        expectedResult =

        // Arrange

        // Act
        let result = Logic.getBody fileName blogDomainName

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult))


module EventSubscribingPolicyTests =

    let getPolicy () = EventSubscribingPolicy()

    [<Test>]
    let Handle_CommentApproved_ProperResult () =

        // Arrange
        let commentId = Guid.NewGuid()
        let message = CommentApproved(commentId)

        let policy = getPolicy () :> IHandleMessages<CommentApproved>
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

        let policy = getPolicy () :> IHandleMessages<CommentRejected>
        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessage = context.SentMessages.[0].Message :?> NotifyAboutCommentAnswer

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentMessage.CommentId, Is.EqualTo(commentId))
        Assert.That(sentMessage.IsApproved, Is.False)

module PolicyTests =

    let userEmail = "testuser@test.com"
    let articleFileName = "_posts/2021-01-02-sample-file-name.md"
    let smtpFrom = "test@test.com"
    let blogDomainName = "sampleBlogDomainName"

    let getPolicy smtpFrom blogDomainName data = CommentAnswerNotificationPolicy(smtpFrom, blogDomainName, Data=data)

    [<Test>]
    let Handle_RegisterCommentNotification_FillProperPolicyData () =

        // Arrange
        let commentId = Guid.NewGuid()
        let message = RegisterCommentNotification(commentId, userEmail, articleFileName)

        let policyData = PolicyData()

        let policy = getPolicy smtpFrom blogDomainName policyData
        let context = getContext ()

        // Act
        let handler = policy :> IHandleMessages<RegisterCommentNotification>
        handler.Handle(message, context) |> ignore

        // Assert
        Assert.That(policyData.UserEmail, Is.EqualTo(userEmail))
        Assert.That(policyData.ArticleFileName, Is.EqualTo(articleFileName))
        Assert.That(policyData.IsNotificationRegistered, Is.True)

    [<Test>]
    let Handle_NotifyAboutCommentAnswer_FillProperPolicyData () =

        // Arrange
        let commentId = Guid.NewGuid()
        let isCommentApproved = false
        let message = NotifyAboutCommentAnswer(commentId, isCommentApproved)

        let policyData = PolicyData()

        let policy = getPolicy smtpFrom blogDomainName policyData
        let context = getContext ()

        // Act
        let handler = policy :> IHandleMessages<NotifyAboutCommentAnswer>
        handler.Handle(message, context) |> ignore

        // Assert
        Assert.That(policyData.IsCommentApproved, Is.EqualTo(isCommentApproved))
        Assert.That(policyData.IsNotificationReadyToSend, Is.True)

    [<TestCase(false, false, false, "", false, false)>]
    [<TestCase(false, true, false, "", false, false)>]
    [<TestCase(true, false, false, "", false, false)>]
    [<TestCase(true, true, false, "", false, true)>]
    [<TestCase(true, true, true, "", false, true)>]
    [<TestCase(true, true, true, "test@test.test", true, true)>]
    let SendNotification_DependsOnPolicyState_SendNotificationAndCompletePolicy
        isNotificationRegistered
        isNotificationReadyToSend
        isCommentApproved
        userEmail
        expectedMessageSent
        expectedSagaCompleted =

        // Arrange
        let policyData =
            PolicyData (
                IsNotificationRegistered=isNotificationRegistered,
                IsNotificationReadyToSend=isNotificationReadyToSend,
                IsCommentApproved=isCommentApproved,
                UserEmail=userEmail,
                ArticleFileName=articleFileName
            )

        let policy = getPolicy smtpFrom blogDomainName policyData
        let context = getContext ()

        // Act>
        policy.SendNotification(context) |> ignore

        // Assert
        let isMessageSent = context.SentMessages.Length > 0
        Assert.That(isMessageSent, Is.EqualTo(expectedMessageSent))
        Assert.That(policy.Completed, Is.EqualTo(expectedSagaCompleted))