module GitHubConfigurationProvider

open System.Configuration

let userAgent =
    ConfigurationManager.AppSettings.["UserAgent"]

let authorizationToken =
    ConfigurationManager.AppSettings.["AuthorizationToken"];

let repositoryName =
    ConfigurationManager.AppSettings.["RepositoryName"]
    
let masterBranchName =
    ConfigurationManager.AppSettings.["MasterBranchName"];