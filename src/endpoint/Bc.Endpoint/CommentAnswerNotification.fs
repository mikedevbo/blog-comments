module Bc.CommentAnswerNotification

open System
open System.Configuration
open System.Linq
open System.Threading.Tasks
open Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands
open NServiceBus
open NServiceBus.Mailer
open NServiceBus.Persistence.Sql

module ConfigurationProvider =

    let smtpFrom =
        ConfigurationManager.AppSettings.["SmtpFrom"]

    let blogDomainName =
        ConfigurationManager.AppSettings.["BlogDomainName"]

[<RequireQualifiedAccess>]
module Logic =

    let isNotificationReady isNotificationRegistered isNotificationReadyToSend =
        isNotificationRegistered && isNotificationReadyToSend

    let isSendNotification isCommentApproved userEmail =
        isCommentApproved && not(String.IsNullOrEmpty(userEmail))

    let getBody (fileName: String) (blogDomainName: string) =
        // depend on Jekyll file format

        // the file name has format _posts/fileName.md
        // the first step is to remove _posts prefix
        let onlyFileName = fileName.Split('/').[1]

        let split = onlyFileName.Split('-')
        let year = split.[0]
        let month = split.[1]
        let day = split.[2]

        // remove year, month and day
        //split.RemoveRange(0, 3)
        let skip = split.Skip(3)

        let join = String.Join("-", skip).Split('.').[0]

        ////TODO: move to resource file
        let result = sprintf "Sprawdź - %s/%s/%s/%s/%s.html" blogDomainName year month day join
        result

type EventSubscribingPolicy() =
    interface IHandleMessages<CommentApproved> with
        member this.Handle(message, context) =
            context.Send(NotifyAboutCommentAnswer(message.CommentId, true))

    interface IHandleMessages<CommentRejected> with
        member this.Handle(message, context) =
            context.Send(NotifyAboutCommentAnswer(message.CommentId, false))

type PolicyData() =
    inherit ContainSagaData()

    member val CommentId = Guid.Empty with get, set
    member val UserEmail = "" with get, set
    member val ArticleFileName = "" with get, set
    member val IsCommentApproved = false with get, set
    member val IsNotificationRegistered = false with get, set
    member val IsNotificationReadyToSend = false with get, set

[<SqlSaga(nameof Unchecked.defaultof<PolicyData>.CommentId)>]
type CommentAnswerNotificationPolicy(smtpFrom: string, blogDomainName: string) =
    inherit Saga<PolicyData>()
        override this.ConfigureHowToFindSaga(mapper: SagaPropertyMapper<PolicyData>) =
            mapper.MapSaga(fun saga -> saga.CommentId :> obj)
                .ToMessage<RegisterCommentNotification>(fun message -> message.CommentId :> obj)
                .ToMessage<NotifyAboutCommentAnswer>(fun message -> message.CommentId :> obj) |> ignore

    new() = CommentAnswerNotificationPolicy(ConfigurationProvider.smtpFrom, ConfigurationProvider.blogDomainName)

    member this.SendNotification (context: IMessageHandlerContext) =
                match Logic.isNotificationReady this.Data.IsNotificationRegistered this.Data.IsNotificationReadyToSend with
                | false ->
                    Task.CompletedTask
                | true ->
                    this.MarkAsComplete()
                    match Logic.isSendNotification this.Data.IsCommentApproved this.Data.UserEmail with
                    | false ->
                        Task.CompletedTask
                    | true ->
                        let mail = Mail()
                        mail.From <- smtpFrom
                        mail.To.Add(this.Data.UserEmail)
                        ////TODO: move to resource file
                        mail.Subject <- "Dodano odpowiedź do komentarza."
                        mail.Body <- Logic.getBody this.Data.ArticleFileName blogDomainName

                        context.SendMail(mail)

    interface IAmStartedByMessages<RegisterCommentNotification> with
        member this.Handle(message, context) =
            this.Data.UserEmail <- message.UserEmail
            this.Data.ArticleFileName <- message.ArticleFileName
            this.Data.IsNotificationRegistered <- true

            this.SendNotification(context)

    interface IAmStartedByMessages<NotifyAboutCommentAnswer> with
        member this.Handle(message, context) =
            this.Data.IsCommentApproved <- message.IsApproved
            this.Data.IsNotificationReadyToSend <- true

            this.SendNotification(context)


