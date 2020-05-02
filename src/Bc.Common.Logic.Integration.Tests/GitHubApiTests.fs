module Bc.Common.Logic.Integration.Tests
//
//open NUnit.Framework
//open Microsoft.Extensions.Configuration
//open System.IO
//open System
//
//let getConfiguration () =
//    let config = (new ConfigurationBuilder()).SetBasePath(Directory.GetCurrentDirectory())
//                                             .AddJsonFile("appsettings.components.integration.tests.json", false, true)
//                                             .Build()
//    config
//
//let getConfiguartionProvider () =
//    Common.ConfigurationProvider(getConfiguration ())
//    
//[<Test>]
//let getSha_execute_properResult () =
//
//    // Arrange
//    let config = getConfiguartionProvider ()
//
//    // Act
//    let result = GitHubApi.GetSha.execute config.UserAgent config.RepositoryName config.MasterBranchName |> Async.RunSynchronously
//
//    // Assert
//    Assert.NotNull(result)
//    printfn "%s" result
//
//[<Test>]
//let getFile_execute_properResult () =
//
//    // Arrange
//    let fileName = "_posts/test.md"
//    let branchName = "c-15"
//    let config = getConfiguartionProvider ()
//
//    // Act
//    let result = GitHubApi.GetFile.execute config.UserAgent config.RepositoryName fileName branchName |> Async.RunSynchronously
//
//    // Assert
//    Assert.NotNull(result)
//    printfn "%A" result
//
//[<Test>]
//let createRepositoryBranch_execute_noException () =
//
//    // Arrange
//    let newBranchName = "c-15"
//    let config = getConfiguartionProvider ()
//
//    // Act
//    GitHubApi.CreateRepositoryBranch.execute config.UserAgent config.AuthorizationToken config.RepositoryName config.MasterBranchName newBranchName
//    |> Async.RunSynchronously
//
//    // Assert
//    Assert.Pass()
//
//[<Test>]
//let updatFile_execute_noException () =
//
//    // Arrange
//    let fileName = "_posts/test.md"
//    let branchName = "c-15"
//    let content = "new comment - " + DateTime.Now.ToString()
//    let config = getConfiguartionProvider ()
//
//    // Act
//    GitHubApi.UpdateFile.execute config.UserAgent config.AuthorizationToken config.RepositoryName fileName branchName content |> Async.RunSynchronously
//
//    // Assert
//    Assert.Pass()
//
//[<Test>]
//let createPullRequest_execute_noException () =
//
//    // Arrange
//    let branchName = "c-15"
//    let config = getConfiguartionProvider ()
//
//    // Act
//    GitHubApi.CreatePullRequest.execute config.UserAgent config.AuthorizationToken config.RepositoryName branchName config.MasterBranchName |> Async.RunSynchronously
//
//    // Assert
//    Assert.Pass()
//    
//[<Test>]
//let isPullRequestOpen_execute_noException () =
//
//    // Arrange
//    let config = getConfiguration ()
//    let configProvider = getConfiguartionProvider ()
//    ////let etag = None
//    let etag = Some "W/\"96ac3062f47cab793ff0aea264498eb4\""//"\"96ac3062f47cab793ff0aea264498eb4\""
//
//    // Act
//    let result = GitHubApi.IsPullRequestOpen.execute configProvider.UserAgent configProvider.AuthorizationToken config.["pullRequestUri"] etag |> Async.RunSynchronously
//
//    // Assert
//    printfn "%A" result
//    Assert.Pass()
//    
//[<Test>]
//let isPullRequestMerged_execute_noException () =
//
//    // Arrange
//    let config = getConfiguration ()
//    let configProvider = getConfiguartionProvider ()
//
//    // Act
//    let result = GitHubApi.IsPullRequestMerged.execute configProvider.UserAgent configProvider.AuthorizationToken config.["pullRequestUri"] |> Async.RunSynchronously
//
//    // Assert
//    printfn "%b" result
//    Assert.Pass()    
