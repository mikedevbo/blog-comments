namespace Bc.Logic.Endpoint.CommentAnswerNotification

open System
open System.Linq
open System.Configuration
open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Logic

module private ConfigurationProvider =
    let SmtpFrom = ConfigurationManager.AppSettings.["SmtpFrom"]

    let BlogDomainName = ConfigurationManager.AppSettings.["BlogDomainName"]

module GetBody =
    let execute (fileName: String) (blogDomainName: string) =
        // depend on Jekyll file format

        // the file name has format _posts/fileName.md
        // the first step is to remove _posts prefix
        let onlyFileName = fileName.Split('/').[1]

        let split = onlyFileName.Split('-')
        let year = split.[0];
        let month = split.[1];
        let day = split.[2];

        // remove year, month and day
        //split.RemoveRange(0, 3)
        let skip = split.Skip(3)

        let join = String.Join("-", skip).Split('.').[0]

        ////TODO: move to resource file
        let result = sprintf "Sprawdź - %s/%s/%s/%s/%s.html" blogDomainName year month day join
        result

type CommentAnswerNotificationPolicyLogic() =
    interface ICommentAnswerNotificationPolicyLogic with
        member this.From = ConfigurationProvider.SmtpFrom

        ////TODO: move to resource file
        member this.Subject = "Dodano odpowiedź do komentarza."
        member this.GetBody fileName = GetBody.execute fileName ConfigurationProvider.BlogDomainName
        member this.IsSendNotification isCommentApproved userEmail =
            not(System.String.IsNullOrEmpty(userEmail)) && isCommentApproved

type CommentAnswerNotificationPolicyLogicFake() =
    interface ICommentAnswerNotificationPolicyLogic with
        member this.From = "test@test.com"
        member this.Subject = "test_subject"
        member this.GetBody _ = "test_body"
        member this.IsSendNotification isCommentApproved userEmail =
            not(System.String.IsNullOrEmpty(userEmail)) && isCommentApproved