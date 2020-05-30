namespace Bc.Contracts.Externals.Endpoint.CommentAnswer.Events

    open System

    type CommentApproved(commentId: Guid) =
        member this.CommentId = commentId

    type CommentRejected(commentId: Guid) =
        member this.CommentId = commentId
