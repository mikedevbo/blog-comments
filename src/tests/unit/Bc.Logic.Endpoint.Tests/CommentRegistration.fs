module Bc.Logic.Endpoint.Tests.CommentRegistration

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
        let userWebsiteOption =
            match userWebsite with
            | null -> None
            | "" -> None
            | value -> (Some value)

        let logic = getLogic () :> ICommentRegistrationPolicyLogic

        // Act
        let result = logic.FormatUserName userName userWebsiteOption

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult))