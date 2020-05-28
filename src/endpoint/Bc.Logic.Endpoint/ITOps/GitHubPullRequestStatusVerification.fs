namespace Bc.Logic.Endpoint.ITOps.GitHubPullRequestStatusVerification

open Bc.Contracts.Internals.Endpoint.ITOps.GitHub
open Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification
open Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification.Messages

type PolicyLogic(configurationProvider: IConfigurationProvider) =
    member this.ConfigurationProvider = configurationProvider
    interface IPolicyLogic with
        member this.CheckPullRequestStatus(pullRequestUri, etag) =
            async {
                let! isOpenResult = GitHubApi.IsPullRequestOpen.execute
                                        GitHubConfigurationProvider.userAgent
                                        GitHubConfigurationProvider.authorizationToken
                                        pullRequestUri
                                        (Some etag)

                if isOpenResult.isOpen then
                    return ResponseCheckPullRequestStatus(PullRequestStatus.Open, isOpenResult.etag)
                else
                    let! isMerged = GitHubApi.IsPullRequestMerged.execute
                                            GitHubConfigurationProvider.userAgent
                                            GitHubConfigurationProvider.authorizationToken
                                            pullRequestUri
                    if isMerged then
                        return ResponseCheckPullRequestStatus(PullRequestStatus.Merged, isOpenResult.etag)
                    else
                        return ResponseCheckPullRequestStatus(PullRequestStatus.Closed, isOpenResult.etag)
            } |> Async.StartAsTask
            