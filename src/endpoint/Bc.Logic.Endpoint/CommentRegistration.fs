module Bc.Logic.Endpoint.CommentRegistration

open Bc.Contracts.Internals.Endpoint.CommentRegistration.Logic

type CommentRegistrationPolicyLogic() =
    interface ICommentRegistrationPolicyLogic with
        member this.FormatUserName userName userWebsite =
            match userWebsite with
            | Some value -> sprintf "[%s](%s)" userName value
            | None -> sprintf "**%s**" userName