module Bc.CommentAnswer

open System
open System.Configuration
open System.Threading.Tasks
open Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
open Bc.Contracts.Externals.Endpoint.CommentRegistration.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswer
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands
open Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages
open NServiceBus
open NServiceBus.Persistence.Sql

module ConfigurationProvider =

    let checkCommentAnswerTimeoutInSeconds =
        ConfigurationManager.AppSettings.["CheckCommentAnswerTimeoutInSeconds"]

type EventsSubscribingPolicy() =
    interface IHandleMessages<CommentRegistered> with
        member this.Handle(message, context) =
            context.Send(CheckCommentAnswer(message.CommentId, message.CommentUri))

type PolicyData() =
    inherit ContainSagaData()

    member val CommentId = Guid.Empty with get, set
    member val CommentUri = "" with get, set
    member val ETag = "" with get, set

[<SqlSaga(nameof Unchecked.defaultof<PolicyData>.CommentId)>]
type CommentAnswerPolicy(checkCommentAnswerTimeoutInSeconds: double) =
    inherit Saga<PolicyData>()
        override this.ConfigureHowToFindSaga(mapper: SagaPropertyMapper<PolicyData>) =
            mapper.MapSaga(fun saga -> saga.CommentId :> obj)
                  .ToMessage<CheckCommentAnswer>(fun message -> message.CommentId :> obj) |> ignore

    new() = CommentAnswerPolicy(double ConfigurationProvider.checkCommentAnswerTimeoutInSeconds)

    interface IAmStartedByMessages<CheckCommentAnswer> with
        member this.Handle(message, context) =
            this.Data.CommentUri <- message.CommentUri
            context.Send(RequestCheckPullRequestStatus(this.Data.CommentUri, this.Data.ETag))

    interface IHandleMessages<ResponseCheckPullRequestStatus> with
        member this.Handle(message, context) =
            let answerStatus =
                match message.PullRequestStatus with
                | PullRequestStatus.Open -> AnswerStatus.NotAdded
                | PullRequestStatus.Merged -> AnswerStatus.Approved
                | PullRequestStatus.Closed -> AnswerStatus.Rejected
                | _ -> raise(ArgumentOutOfRangeException($"Not supported pull request status: {message.PullRequestStatus}"))

            match answerStatus with
            | AnswerStatus.NotAdded ->
                this.Data.ETag <- message.ETag
                this.RequestTimeout<TimeoutCheckCommentAnswer>(
                        context,
                        TimeSpan.FromSeconds(checkCommentAnswerTimeoutInSeconds))

            | AnswerStatus.Approved ->
                this.MarkAsComplete()
                context.Publish(CommentApproved(this.Data.CommentId))

            | AnswerStatus.Rejected ->
                this.MarkAsComplete()
                context.Publish(CommentRejected(this.Data.CommentId))

            | _ ->
                raise (ArgumentOutOfRangeException($"Not supported comment answer status: {answerStatus}"))

    interface IHandleTimeouts<TimeoutCheckCommentAnswer> with
        member this.Timeout(state, context) =
            context.Send(RequestCheckPullRequestStatus(this.Data.CommentUri, this.Data.ETag))

