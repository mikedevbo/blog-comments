module Bc.Logic.Endpoint.Tests.CommentAnswerNotification

open System
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Logic
open Bc.Logic.Endpoint.CommentAnswerNotification
open NUnit.Framework

module GetBodyTests =

    [<TestCase("_posts/2018-02-10-title_subtitle_something.md", "blogDomainName", "Sprawdź - blogDomainName/2018/02/10/title_subtitle_something.html")>]
    [<TestCase("_posts/2018-02-10-title-subtitle-something.md", "blogDomainName", "Sprawdź - blogDomainName/2018/02/10/title-subtitle-something.html")>]
    let GetBody_Execute_ProperResult(fileName, blogDomainName, expectedResult) =

        // Arrange

        // Act
        let result = GetBody.execute fileName blogDomainName

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult))

module CommentAnswerNotificationPolicyLogicTests =

    let getPolicy () =
        CommentAnswerNotificationPolicyLogic()

    [<TestCase(false, null, false)>]
    [<TestCase(false, "", false)>]
    [<TestCase(false, "user_email", false)>]
    [<TestCase(true, null, false)>]
    [<TestCase(true, "", false)>]
    [<TestCase(true, "user_email", true)>]
    let GetBody_Execute_ProperResult(isCommentApproved, userEmail, expectedResult) =

        // Arrange
        let commentId = Guid.NewGuid()
        let message = NotifyAboutCommentAnswer(commentId, isCommentApproved)
        let policy = getPolicy () :> ICommentAnswerNotificationPolicyLogic

        // Act
        let result = policy.IsSendNotification message userEmail

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult))