namespace Bc.Logic.Endpoint

open Bc.Contracts.Internals.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentAnswer
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages

type CommentAnswerPolicyLogic(configurationProvider: IEndpointConfigurationProvider) =
    member this.configurationProvider = configurationProvider
    interface ICommentAnswerPolicyLogic with
        member this.CheckAnswer(commentUri, etag) =
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
                return ResponseCheckCommentAnswer(CommentAnswerStatus.Approved, "123")
            } |> Async.StartAsTask

type CommentAnswerPolicyLogicFake(configurationProvider: IEndpointConfigurationProvider) =
    member this.configurationProvider = configurationProvider
    interface ICommentAnswerPolicyLogic with
        member this.CheckAnswer(_, _) =
            async {
                return ResponseCheckCommentAnswer(CommentAnswerStatus.Approved, "123")
            } |> Async.StartAsTask

