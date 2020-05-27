module Bc.Logic.Endpoint.ITOps.GitHub

open Bc.Contracts.Internals.Endpoint.ITOps.GitHub
open Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification
open Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification.Messages

module PullRequestStatusVerification =
    type PolicyLogic(configurationProvider: IConfigurationProvider) =
        member this.ConfigurationProvider = configurationProvider
        interface IPolicyLogic with
            member this.CheckPullRequestStatus(pullRequestUri, etag) =
                async {
//                let! isOpenResult = GitHubApi.IsPullRequestOpen.execute
//                                        this.configurationProvider.UserAgent
//                                        this.configurationProvider.AuthorizationToken
//                                        commentUri
//                                        (Some etag)
//
//                if isOpenResult.isOpen then
//                    return CheckCommentAnswerMsgResponseMsg(CommentAnswerStatus.NotAdded, isOpenResult.etag)
//                else
//                    let! isMerged = GitHubApi.IsPullRequestMerged.execute
//                                            this.configurationProvider.UserAgent
//                                            this.configurationProvider.AuthorizationToken
//                                            commentUri
//                    if isMerged then
//                        return CheckCommentAnswerMsgResponseMsg(CommentAnswerStatus.Approved, isOpenResult.etag)
//                    else
//                        return CheckCommentAnswerMsgResponseMsg(CommentAnswerStatus.Rejected, isOpenResult.etag)
                    return ResponseCheckPullRequestStatus(PullRequestStatus.Merged, "123")
                } |> Async.StartAsTask
            