namespace Bc.Logic.Endpoint

open Bc.Contracts.Internals.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentAnswer

type CommentAnswerPolicyLogic(configurationProvider: IEndpointConfigurationProvider) =
    member this.configurationProvider = configurationProvider
    interface ICommentAnswerPolicyLogic with
        member this.CheckAnswer(commentUri, etag) =
            async {
                ////TODO: add logic
                return CheckCommentAnswerMsgResponseMsg(CommentAnswerStatus.Approved, "123")
            } |> Async.StartAsTask

type CommentAnswerPolicyLogicFake(configurationProvider: IEndpointConfigurationProvider) =
    member this.configurationProvider = configurationProvider
    interface ICommentAnswerPolicyLogic with
        member this.CheckAnswer(commentUri, etag) =
            async {
                return CheckCommentAnswerMsgResponseMsg(CommentAnswerStatus.Approved, "123")
            } |> Async.StartAsTask

