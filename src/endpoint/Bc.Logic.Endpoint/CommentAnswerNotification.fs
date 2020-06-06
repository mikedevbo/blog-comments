namespace Bc.Logic.Endpoint.CommentAnswerNotification

open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Logic

type CommentAnswerNotificationPolicyLogic() =
    interface ICommentAnswerNotificationPolicyLogic with
        member this.From = "From"
        member this.Subject = "Subject"
        member this.GetBody fileName = "Body"
        member this.IsSendNotification message userEmail =
            true        
        
type CommentAnswerNotificationPolicyLogicFake() =
    interface ICommentAnswerNotificationPolicyLogic with
        member this.From = "test@test.com"
        member this.Subject = "test_subject"
        member this.GetBody fileName = "test_body"
        member this.IsSendNotification message userEmail =
            true