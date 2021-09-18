namespace Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands

    open System

    type RegisterComment(
                        commentId: Guid,
                        userName: string,
                        userWebsite: string,
                        userComment: string,
                        articleFileName: string,
                        commentAddedDate: DateTime
                    ) =
        member this.CommentId = commentId
        member this.UserName = userName
        member this.UserWebsite = userWebsite
        member this.UserComment = userComment
        member this.ArticleFileName = articleFileName
        member this.CommentAddedDate = commentAddedDate