module Components.Logic.FSharp.Integration.Tests

open NUnit.Framework
open Microsoft.Extensions.Configuration
open System.IO
open System

let getConfiguartion () =
    let config = (new ConfigurationBuilder()).SetBasePath(Directory.GetCurrentDirectory())
                                             .AddJsonFile("appsettings.components.integration.tests.json", false, true)
                                             .Build();
        
    (new Common.ConfigurationProvider(config))
    
[<Test>]
let getSha_execute_properResult () =

    // Arrange
    let config = getConfiguartion ()

    // Act
    let result = GitHubApi.GetSha.execute config.UserAgent config.RepositoryName config.MasterBranchName |> Async.RunSynchronously

    // Assert
    Assert.NotNull(result)
    printfn "%s" result

[<Test>]
let createRepositoryBranch_execute_noException () =

    // Arrange
    let newBranchName = "c-15"
    let config = getConfiguartion ()

    // Act
    GitHubApi.CreateRepositoryBranch.execute config.UserAgent config.AuthorizationToken config.RepositoryName config.MasterBranchName newBranchName
    |> Async.RunSynchronously

    // Assert
    Assert.Pass()

[<Test>]
let updatFile_execute_noException () =

    // Arrange
    let fileName = "_posts/test.md"
    let branchName = "c-15"
    let content = "\nnew comment - " + DateTime.Now.ToString()
    let config = getConfiguartion ()

    // Act
    let result = GitHubApi.UpdatFile.execute config.UserAgent config.AuthorizationToken config.RepositoryName branchName fileName content |> Async.RunSynchronously

    // Assert
    Assert.Pass()
