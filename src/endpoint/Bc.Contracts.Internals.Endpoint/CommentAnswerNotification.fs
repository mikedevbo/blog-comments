namespace Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands

    open System

    type RegisterCommentNotification(commentId: Guid, userEmail: string, articleFileName: string) =
        member this.CommentId = commentId
        member this.UserEmail = userEmail
        member this.ArticleFileName = articleFileName
        
    type NotifyAboutCommentAnswer(commentId: Guid, isApproved: bool) =
        member this.CommentId = commentId
        member this.IsApproved = isApproved
        
namespace Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Logic

    open Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands

    type ICommentAnswerNotificationPolicyLogic =
        abstract member From: string
        abstract member Subject: string
        abstract member GetBody: articleFileName: string -> string
        abstract member IsSendNotification: message: NotifyAboutCommentAnswer -> userEmail: string -> bool

