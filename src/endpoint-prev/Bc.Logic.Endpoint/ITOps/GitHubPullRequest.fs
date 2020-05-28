module Bc.Logic.Endpoint.ITOps.GitHubPullRequest

open System.Configuration
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.ITOps.GitHubPullRequest

type PolicyLogic(configurationProvider: IConfigurationProvider) =
        member this.ConfigurationProvider = configurationProvider
        interface IPolicyLogic with
            member this.CreateBranch(creationDate) =
                async {
                    let branchName = sprintf "c-%s" (creationDate.ToString("yyyy-MM-dd-HH-mm-ss-fff"))
                    do! GitHubApi.CreateRepositoryBranch.execute
                            this.ConfigurationProvider.UserAgent
                            this.ConfigurationProvider.AuthorizationToken
                            this.ConfigurationProvider.RepositoryName
                            this.ConfigurationProvider.MasterBranchName
                            branchName
                    return branchName
                } |> Async.StartAsTask

            member this.UpdateFile(branchName, fileName, content) =
                async {
                    do! GitHubApi.UpdateFile.execute
                                this.ConfigurationProvider.UserAgent
                                this.ConfigurationProvider.AuthorizationToken
                                this.ConfigurationProvider.RepositoryName
                                fileName
                                branchName
                                content
                } |> Async.StartAsTask :> Task

            member this.CreatePullRequest(branchName) =
                async {
                    let! pullRequestUri = GitHubApi.CreatePullRequest.execute
                                            this.ConfigurationProvider.UserAgent
                                            this.ConfigurationProvider.AuthorizationToken
                                            this.ConfigurationProvider.RepositoryName
                                            branchName
                                            this.ConfigurationProvider.MasterBranchName
                    return pullRequestUri
                } |> Async.StartAsTask
                
type ConfigurationProvider() =
    interface IConfigurationProvider with
        member this.UserAgent =
            ConfigurationManager.AppSettings.["UserAgent"];

        member this.AuthorizationToken =
            ConfigurationManager.AppSettings.["AuthorizationToken"];

        member this.RepositoryName =
            ConfigurationManager.AppSettings.["RepositoryName"];

        member this.MasterBranchName =
            ConfigurationManager.AppSettings.["MasterBranchName"];

type PolicyLogicFake() =
    interface IPolicyLogic with
        member this.CreateBranch(_) =
            async {
                return "branch_1234"
            } |> Async.StartAsTask

        member this.UpdateFile(_, _, _) =
            async {
                ()
            } |> Async.StartAsTask :> Task

        member this.CreatePullRequest(_) =
            async {
                return "uri_1234"
            } |> Async.StartAsTask