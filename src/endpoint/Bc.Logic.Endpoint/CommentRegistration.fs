namespace Bc.Logic.Endpoint

open System
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentRegistration

type RegisterCommentPolicyLogic(configurationProvider: IEndpointConfigurationProvider) =
    member this.configurationProvider = configurationProvider
    interface IRegisterCommentPolicyLogic with
        member this.CreateBranch(creationDate) =
            async {
                return "branch_123"
            } |> Async.StartAsTask

        member this.AddComment(branchName, commentData) =
            async {
                ()
            } |> Async.StartAsTask :> Task

        member this.CreatePullRequest(branchName) =
            async {
                return "uri_123"
            } |> Async.StartAsTask
