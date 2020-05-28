namespace Bc.Contracts.Internals.Endpoint.CommentTaking.Commands

open System

type TakeComment(
                    commentId: Guid,
                    userName: string,
                    userEmail: string,
                    userWebsite: string,
                    userComment: string,
                    articleFileName: string,
                    commentAddedDate: DateTime
                ) =
    member this.CommentId = commentId
    member this.UserName = userName
    member this.UserEmail = userEmail
    member this.UserWebsite = userWebsite
    member this.UserComment = userComment
    member this.ArticleFileName = articleFileName
    member this.CommentAddedDate = commentAddedDate


