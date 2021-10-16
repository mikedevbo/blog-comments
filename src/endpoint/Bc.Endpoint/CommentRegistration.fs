module Bc.CommentRegistration

open System
open System.Threading.Tasks
open Bc.Contracts.Externals.Endpoint.CommentRegistration.Events
open Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages
open NServiceBus
open NServiceBus.Persistence.Sql

[<RequireQualifiedAccess>]
module Logic =

    let formatUserName userName userWebsite =
        match userWebsite with
        | null -> sprintf "**%s**" userName
        | "" -> sprintf "**%s**" userName
        | _ -> sprintf "[%s](%s)" userName userWebsite

    let formatUserComment userName userComment (commentAddedDate: DateTime) =
        sprintf "begin-%s-%s-%s UTC %s" userName userComment (commentAddedDate.ToString("yyyy-MM-dd HH:mm")) Environment.NewLine

type PolicyData() =
    inherit ContainSagaData()

    member val CommentId = Guid.Empty with get, set
    member val UserName = "" with get, set
    member val UserWebsite = "" with get, set
    member val UserComment = "" with get, set
    member val ArticleFileName = "" with get, set
    member val CommentAddedDate = DateTime.MinValue with get, set

[<SqlSaga(nameof Unchecked.defaultof<PolicyData>.CommentId)>]
type CommentRegistrationPolicy() =
    inherit Saga<PolicyData>()
        override this.ConfigureHowToFindSaga(mapper: SagaPropertyMapper<PolicyData>) =
            mapper.MapSaga(fun saga -> saga.CommentId :> obj)
                .ToMessage<RegisterComment>(fun message -> message.CommentId :> obj)
                .ToMessage<ResponseCreateGitHubPullRequest>(fun message -> message.CommentId :> obj) |> ignore

    interface IAmStartedByMessages<RegisterComment> with
        member this.Handle(message, context) =
            this.Data.UserName <- message.UserName
            this.Data.UserWebsite <- message.UserWebsite
            this.Data.UserComment <- message.UserComment
            this.Data.ArticleFileName <- message.ArticleFileName
            this.Data.CommentAddedDate <- message.CommentAddedDate

            let formatUserName = Logic.formatUserName this.Data.UserName this.Data.UserWebsite
            let formatUserComment = Logic.formatUserComment formatUserName this.Data.UserComment this.Data.CommentAddedDate

            context.Send(RequestCreateGitHubPullRequest(
                            message.CommentId,
                            this.Data.ArticleFileName,
                            formatUserComment,
                            this.Data.CommentAddedDate))

    interface IHandleMessages<ResponseCreateGitHubPullRequest> with
        member this.Handle(message, context) =
            this.MarkAsComplete()
            context.Publish(CommentRegistered(this.Data.CommentId, message.PullRequestUri))