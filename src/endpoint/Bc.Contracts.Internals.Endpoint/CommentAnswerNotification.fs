namespace Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands

    open System

    type RegisterCommentNotification(commentId: Guid, userEmail: string) =
        member this.CommentId = commentId
        member this.UserEmail = userEmail
        
    type NotifyAboutCommentAnswer(commentId: Guid, isApproved: bool) =
        member this.CommentId = commentId
        member this.IsApproved = isApproved

