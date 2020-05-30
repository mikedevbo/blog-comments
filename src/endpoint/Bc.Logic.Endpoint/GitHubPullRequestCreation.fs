module Bc.Logic.Endpoint.GitHubPullRequestCreation

open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation

type PolicyLogic() =
        interface IPolicyLogic with
            member this.CreateBranch creationDate =
                async {
                    let branchName = sprintf "c-%s" (creationDate.ToString("yyyy-MM-dd-HH-mm-ss-fff"))
                    do! GitHubApi.CreateRepositoryBranch.execute
                            GitHubConfigurationProvider.userAgent
                            GitHubConfigurationProvider.authorizationToken
                            GitHubConfigurationProvider.repositoryName
                            GitHubConfigurationProvider.masterBranchName
                            branchName
                    return branchName
                } |> Async.StartAsTask

            member this.UpdateFile branchName fileName content =
                async {
                    do! GitHubApi.UpdateFile.execute
                                GitHubConfigurationProvider.userAgent
                                GitHubConfigurationProvider.authorizationToken
                                GitHubConfigurationProvider.repositoryName
                                fileName
                                branchName
                                content
                } |> Async.StartAsTask :> Task

            member this.CreatePullRequest branchName  =
                async {
                    let! pullRequestUri = GitHubApi.CreatePullRequest.execute
                                            GitHubConfigurationProvider.userAgent
                                            GitHubConfigurationProvider.authorizationToken
                                            GitHubConfigurationProvider.repositoryName
                                            branchName
                                            GitHubConfigurationProvider.masterBranchName
                    return pullRequestUri
                } |> Async.StartAsTask
                
type PolicyLogicFake() =
    interface IPolicyLogic with
        member this.CreateBranch(_) =
            async {
                return "branch_1234"
            } |> Async.StartAsTask

        member this.UpdateFile _ _ _ =
            async {
                ()
            } |> Async.StartAsTask :> Task

        member this.CreatePullRequest _ =
            async {
                return "uri_1234"
            } |> Async.StartAsTask