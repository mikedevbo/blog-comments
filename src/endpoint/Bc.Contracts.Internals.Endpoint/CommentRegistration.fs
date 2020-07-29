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

namespace Bc.Contracts.Internals.Endpoint.CommentRegistration.Logic

    open System

    type ICommentRegistrationPolicyLogic =
        abstract member FormatUserName: userName: string -> userWebsite: string -> string

        abstract member FormatUserComment: userName: string -> userComment: string -> commentAddedDate: DateTime -> string