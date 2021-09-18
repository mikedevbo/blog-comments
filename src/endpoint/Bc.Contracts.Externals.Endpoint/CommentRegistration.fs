namespace Bc.Contracts.Externals.Endpoint.CommentRegistration.Events

    open System

    type CommentRegistered(commentId: Guid, commentUri: string) =
        member this.CommentId = commentId
        member this.CommentUri = commentUri
