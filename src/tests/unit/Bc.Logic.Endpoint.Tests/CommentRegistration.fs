module Bc.Logic.Endpoint.Tests.CommentRegistration

open System
open Bc.Contracts.Internals.Endpoint.CommentRegistration.Logic
open Bc.Logic.Endpoint.CommentRegistration
open NUnit.Framework

module CommentRegistrationPolicyLogicTests =

    let getLogic () =
        CommentRegistrationPolicyLogic()

    [<TestCase("user", null, "**user**")>]
    [<TestCase("user", "", "**user**")>]
    [<TestCase("user", "webSite", "[user](webSite)")>]
    let FormatUserName_Execute_ProperResult(userName, userWebsite, expectedResult) =

        // Arrange
        let logic = getLogic () :> ICommentRegistrationPolicyLogic

        // Act
        let result = logic.FormatUserName userName userWebsite

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult))

    [<Test>]
    let FormatUserComment_Execute_ProperResult() =

        // Arrange
        let userName = "user_name"
        let userComment = "user_comment"
        let commentAddedDate = DateTime(2020, 7, 29, 10, 0, 0)
        let expectedResult = "begin-user_name-user_comment-2020-07-29 10:00 UTC \n"
        let logic = getLogic () :> ICommentRegistrationPolicyLogic

        // Act
        let result = logic.FormatUserComment userName userComment commentAddedDate

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult))