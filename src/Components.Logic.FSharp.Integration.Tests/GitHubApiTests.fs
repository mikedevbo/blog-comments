module Components.Logic.FSharp.Integration.Tests

open NUnit.Framework
open Microsoft.Extensions.Configuration
open System.IO

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
    GitHubApi.CreateRepositoryBranch.execute config.UserAgent config.AuthorizationToken config.RepositoryName config.MasterBranchName newBranchName //|> Async.RunSynchronously
    |> Async.RunSynchronously

    // Assert
    Assert.Pass()
