module Bc.GitHubPullRequestCreation

open System
open System.Configuration
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Logic
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages
open GitHubApi
open NServiceBus
open NServiceBus.Persistence.Sql

module ConfigurationProvider =

    let isUseFakes =
        Convert.ToBoolean(ConfigurationManager.AppSettings.["IsUseFakes"])

module GitHub =
    let createBranch (creationDate: DateTime) =
        async {
            match ConfigurationProvider.isUseFakes with
            | true ->
                return "branch_1234"
            | false ->
                let branchName = sprintf "c-%s" (creationDate.ToString("yyyy-MM-dd-HH-mm-ss-fff"))
                do! GitHubApi.CreateRepositoryBranch.execute
                        GitHubConfigurationProvider.userAgent
                        GitHubConfigurationProvider.authorizationToken
                        GitHubConfigurationProvider.repositoryName
                        GitHubConfigurationProvider.masterBranchName
                        branchName
                return branchName
        }

    let updateFile fileName branchName content =
        async {
            match ConfigurationProvider.isUseFakes with
            | true ->
                ()
            | false ->
                do! GitHubApi.UpdateFile.execute
                                            GitHubConfigurationProvider.userAgent
                                            GitHubConfigurationProvider.authorizationToken
                                            GitHubConfigurationProvider.repositoryName
                                            fileName
                                            branchName
                                            content
        }

    let createPullRequest branchName =
        async {
            match ConfigurationProvider.isUseFakes with
            | true ->
                return "uri_1234"
            | false ->
                let! pullRequestUri = GitHubApi.CreatePullRequest.execute
                                                                    GitHubConfigurationProvider.userAgent
                                                                    GitHubConfigurationProvider.authorizationToken
                                                                    GitHubConfigurationProvider.repositoryName
                                                                    branchName
                                                                    GitHubConfigurationProvider.masterBranchName
                return pullRequestUri
        }

type PolicyData () =
    inherit ContainSagaData()

    member val CommentId = Guid.Empty with get, set
    member val FileName = "" with get, set
    member val Content = "" with get, set
    member val AddedDate = DateTime.MinValue with get, set
    member val BranchName = "" with get, set


[<SqlSaga(nameof Unchecked.defaultof<PolicyData>.CommentId)>]
type Policy() =
    inherit Saga<PolicyData>()
        override this.ConfigureHowToFindSaga(mapper: SagaPropertyMapper<PolicyData>) =
            mapper.MapSaga(fun saga -> saga.CommentId :> obj)
                  .ToMessage<RequestCreateGitHubPullRequest>(fun message -> message.CommentId :> obj) |> ignore

    interface IAmStartedByMessages<RequestCreateGitHubPullRequest> with
        member this.Handle(message, context) =
            this.Data.FileName <- message.FileName
            this.Data.Content <- message.Content
            this.Data.AddedDate <- message.AddedDate

            context.Send(RequestCreateBranch(this.Data.AddedDate))

    interface IHandleMessages<ResponseCreateBranch> with
        member this.Handle(message, context) =
            this.Data.BranchName <- message.BranchName

            context.Send(RequestUpdateFile(this.Data.BranchName, this.Data.FileName, this.Data.Content))

    interface IHandleMessages<ResponseUpdateFile> with
        member this.Handle(message, context) =
            context.Send(RequestCreatePullRequest(this.Data.BranchName))

    interface IHandleMessages<ResponseCreatePullRequest> with
        member this.Handle(message, context) =
            this.MarkAsComplete()
            this.ReplyToOriginator(
                context,
                ResponseCreateGitHubPullRequest(this.Data.CommentId, message.PullRequestUri))

type PolicyHandlers() =
    interface IHandleMessages<RequestCreateBranch> with
        member this.Handle(message, context) =
            async {

                let! branchName = GitHub.createBranch message.AddedDate
                do! context.Reply(ResponseCreateBranch(branchName)) |> Async.AwaitTask

            } |> Async.StartAsTask :> Task

    interface IHandleMessages<RequestUpdateFile> with
        member this.Handle(message, context) =
            async {

                do! GitHub.updateFile message.FileName message.BranchName message.Content
                do! context.Reply(ResponseUpdateFile()) |> Async.AwaitTask

            } |> Async.StartAsTask :> Task

    interface IHandleMessages<RequestCreatePullRequest> with
        member this.Handle(message, context) =
            async {

                let! pullRequestUri = GitHub.createPullRequest message.BranchName
                do! context.Reply(ResponseCreatePullRequest(pullRequestUri)) |> Async.AwaitTask

            } |> Async.StartAsTask :> Task
