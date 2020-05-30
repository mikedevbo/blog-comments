namespace Bc.Contracts.Externals.Endpoint.CommentRegistration.Events

    open System

    type CommentRegistered(commentId: Guid, pullRequestUri: string) =
        member this.CommentId = commentId
        member this.PullRequestUri = pullRequestUri