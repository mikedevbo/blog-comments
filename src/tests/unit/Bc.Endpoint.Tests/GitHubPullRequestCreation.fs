module Bc.Endpoint.Tests.GitHubPullRequestCreation

open System
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages
open Bc.GitHubPullRequestCreation
open Bc.Endpoint
open NServiceBus
open NServiceBus.Testing
open NUnit.Framework

let getContext() =
    TestableMessageHandlerContext()

module PolicyTests =

    let getPolicy data = Policy(Data = data)

    [<Test>]
    let Handle_RequestCreateGitHubPullRequest_ProperResult () =

        // Arrange
        let commentId = Guid.NewGuid()
        let content = "content"
        let fileName = "fileName"
        let addedDate = DateTime(2020, 7, 25)
        let message = RequestCreateGitHubPullRequest(
                                                        commentId,
                                                        fileName,
                                                        content,
                                                        addedDate
                                                    )

        let policyData = PolicyData()
        let policy = getPolicy policyData :> IHandleMessages<RequestCreateGitHubPullRequest>

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessage = context.SentMessages.[0].Message :?> RequestCreateBranch

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentMessage.AddedDate, Is.EqualTo(addedDate))
        Assert.That(policyData.FileName, Is.EqualTo(fileName))
        Assert.That(policyData.Content, Is.EqualTo(content))
        Assert.That(policyData.AddedDate, Is.EqualTo(addedDate))

    [<Test>]
    let Handle_ResponseCreateBranch_ProperResult () =

        // Arrange
        let branchName = "branch_name"
        let content = "content"
        let fileName = "fileName"
        let message = ResponseCreateBranch(branchName)

        let policyData = PolicyData(FileName = fileName, Content = content)
        let policy = getPolicy policyData :> IHandleMessages<ResponseCreateBranch>

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessage = context.SentMessages.[0].Message :?> RequestUpdateFile

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentMessage.BranchName, Is.EqualTo(branchName))
        Assert.That(sentMessage.FileName, Is.EqualTo(fileName))
        Assert.That(sentMessage.Content, Is.EqualTo(content))
        Assert.That(policyData.BranchName, Is.EqualTo(branchName))

    [<Test>]
    let Handle_ResponseUpdateFile_ProperResult () =

        // Arrange
        let branchName = "branch_name"
        let message = ResponseUpdateFile()

        let policyData = PolicyData(BranchName = branchName)
        let policy = getPolicy policyData :> IHandleMessages<ResponseUpdateFile>

        let context = getContext ()

        // Act
        policy.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.SentMessages.Length
        let sentMessage = context.SentMessages.[0].Message :?> RequestCreatePullRequest

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(sentMessage.BranchName, Is.EqualTo(branchName))
        Assert.That(policyData.BranchName, Is.EqualTo(branchName))

    [<Test>]
    let Handle_ResponseCreatePullRequest_ProperResult () =

        // Arrange
        let commentId = Guid.NewGuid()
        let pullRequestUri = "uri_123"
        let message = ResponseCreatePullRequest(pullRequestUri)

        let policyData = PolicyData(CommentId = commentId)
        let policy = getPolicy policyData
        policy.Entity.Originator <- "test"

        let context = getContext ()

        // Act
        let policyHandler = policy :> IHandleMessages<ResponseCreatePullRequest>
        policyHandler.Handle(message, context) |> ignore

        // Assert
        let sentNumberOfMessages = context.RepliedMessages.Length
        let repliedMessage = context.RepliedMessages.[0].Message :?> ResponseCreateGitHubPullRequest

        Assert.That(sentNumberOfMessages, Is.EqualTo(1))
        Assert.That(repliedMessage.CommentId, Is.EqualTo(commentId))
        Assert.That(repliedMessage.PullRequestUri, Is.EqualTo(pullRequestUri))
        Assert.That(policy.Completed, Is.True)