namespace Bc.Logic.Endpoint.CommentRegistration

open System
open Bc.Contracts.Internals.Endpoint.CommentRegistration.Logic

type CommentRegistrationPolicyLogic() =
    interface ICommentRegistrationPolicyLogic with
        member this.FormatUserName userName userWebsite =
            match userWebsite with
            | null -> sprintf "**%s**" userName
            | "" -> sprintf "**%s**" userName
            | _ -> sprintf "[%s](%s)" userName userWebsite


        member this.FormatUserComment userName userComment commentAddedDate =
            sprintf "begin-%s-%s-%s UTC %s" userName userComment (commentAddedDate.ToString("yyyy-MM-dd HH:mm")) Environment.NewLine

type CommentRegistrationPolicyLogicFake() =
    interface ICommentRegistrationPolicyLogic with
        member this.FormatUserName userName _ =
            userName

        member this.FormatUserComment _ userComment _ =
            userComment