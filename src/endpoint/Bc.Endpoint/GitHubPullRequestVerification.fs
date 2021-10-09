module Bc.GitHubPullRequestVerification

open System
open System.Configuration
open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages
open GitHubApi
open NServiceBus

module ConfigurationProvider =

    let isUseFakes =
        Convert.ToBoolean(ConfigurationManager.AppSettings.["isUseFakes"])

module GitHub =
    let checkPullRequestStatus (pullRequestUri: string) (etag: string) =
        async {
            match ConfigurationProvider.isUseFakes with
            | true ->
                return ResponseCheckPullRequestStatus(PullRequestStatus.Merged, "etag_123")
            | false ->
                let! isOpenResult = GitHubApi.IsPullRequestOpen.execute
                                        GitHubConfigurationProvider.userAgent
                                        GitHubConfigurationProvider.authorizationToken
                                        pullRequestUri
                                        (Some etag)

                if isOpenResult.IsOpen then
                    return ResponseCheckPullRequestStatus(PullRequestStatus.Open, isOpenResult.Etag)
                else
                    let! isMerged = GitHubApi.IsPullRequestMerged.execute
                                            GitHubConfigurationProvider.userAgent
                                            GitHubConfigurationProvider.authorizationToken
                                            pullRequestUri
                    if isMerged then
                        return ResponseCheckPullRequestStatus(PullRequestStatus.Merged, isOpenResult.Etag)
                    else
                        return ResponseCheckPullRequestStatus(PullRequestStatus.Closed, isOpenResult.Etag)
        }

type Policy (checkPullRequestStatus: string -> string -> Async<ResponseCheckPullRequestStatus>) =
    new() = Policy(GitHub.checkPullRequestStatus)

    interface IHandleMessages<RequestCheckPullRequestStatus> with
        member this.Handle(message, context) =
            async {

                let! response = checkPullRequestStatus message.PullRequestUri message.ETag

                do! context.Reply(response) |> Async.AwaitTask

            } |> Async.StartAsTask :> Task
