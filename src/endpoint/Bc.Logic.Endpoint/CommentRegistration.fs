namespace Bc.Logic.Endpoint

open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentRegistration

type RegisterCommentPolicyLogic(configurationProvider: IEndpointConfigurationProvider) =
    member this.configurationProvider = configurationProvider
    interface IRegisterCommentPolicyLogic with
        member this.CreateBranch(creationDate) =
            async {
                let branchName = sprintf "c-%s" (creationDate.ToString("yyyy-MM-dd-HH-mm-ss-fff"))
                do! GitHubApi.CreateRepositoryBranch.execute
                        this.configurationProvider.UserAgent
                        this.configurationProvider.AuthorizationToken
                        this.configurationProvider.RepositoryName
                        this.configurationProvider.MasterBranchName
                        branchName
                return branchName
            } |> Async.StartAsTask

        member this.AddComment(branchName, commentData) =
            async {
                ()
            } |> Async.StartAsTask :> Task

        member this.CreatePullRequest(branchName) =
            async {
                return "uri_123"
            } |> Async.StartAsTask

type RegisterCommentPolicyLogicFake() =
    interface IRegisterCommentPolicyLogic with
        member this.CreateBranch(creationDate) =
            async {
                return "branch_1234"
            } |> Async.StartAsTask

        member this.AddComment(branchName, commentData) =
            async {
                ()
            } |> Async.StartAsTask :> Task

        member this.CreatePullRequest(branchName) =
            async {
                return "uri_123"
            } |> Async.StartAsTask