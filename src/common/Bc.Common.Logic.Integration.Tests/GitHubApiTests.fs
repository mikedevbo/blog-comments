module Bc.Common.Logic.Integration.Tests

open System.Configuration
open NUnit.Framework
open System

let userAgent = ConfigurationManager.AppSettings.["UserAgent"]
let authorizationToken = ConfigurationManager.AppSettings.["AuthorizationToken"]
let repositoryName = ConfigurationManager.AppSettings.["RepositoryName"]
let masterBranchName = ConfigurationManager.AppSettings.["MasterBranchName"]
let pullRequestUri = ConfigurationManager.AppSettings.["PullRequestUri"]

let branchName = "c-17"
    
[<Test>]
let getSha_execute_properResult () =

    // Arrange

    // Act
    let result = GitHubApi.GetSha.execute userAgent repositoryName masterBranchName |> Async.RunSynchronously

    // Assert
    Assert.NotNull(result)
    printfn "%s" result

[<Test>]
let getFile_execute_properResult () =

    // Arrange
    let fileName = "_posts/test.md"

    // Act
    let result = GitHubApi.GetFile.execute userAgent repositoryName fileName branchName |> Async.RunSynchronously

    // Assert
    Assert.NotNull(result)
    printfn "%A" result

[<Test>]
let createRepositoryBranch_execute_noException () =

    // Arrange

    // Act
    GitHubApi.CreateRepositoryBranch.execute userAgent authorizationToken repositoryName masterBranchName branchName
    |> Async.RunSynchronously

    // Assert
    Assert.Pass()

[<Test>]
let updateFile_execute_noException () =

    // Arrange
    let fileName = "_posts/test.md"
    let content = "new comment - " + DateTime.Now.ToString()

    // Act
    GitHubApi.UpdateFile.execute userAgent authorizationToken repositoryName fileName branchName content |> Async.RunSynchronously

    // Assert
    Assert.Pass()

[<Test>]
let createPullRequest_execute_noException () =

    // Arrange

    // Act
    GitHubApi.CreatePullRequest.execute userAgent authorizationToken repositoryName branchName masterBranchName |> Async.RunSynchronously

    // Assert
    Assert.Pass()
    
[<Test>]
let isPullRequestOpen_execute_noException () =

    // Arrange
    ////let etag = None
    let etag = Some "W/\"e06d1474660cd511ae4e78da4c1389da\""//"\"96ac3062f47cab793ff0aea264498eb4\""

    // Act
    let result = GitHubApi.IsPullRequestOpen.execute userAgent authorizationToken pullRequestUri etag |> Async.RunSynchronously

    // Assert
    printfn "%A" result
    Assert.Pass()
    
[<Test>]
let isPullRequestMerged_execute_noException () =

    // Arrange

    // Act
    let result = GitHubApi.IsPullRequestMerged.execute userAgent authorizationToken pullRequestUri |> Async.RunSynchronously

    // Assert
    printfn "%b" result
    Assert.Pass()    
