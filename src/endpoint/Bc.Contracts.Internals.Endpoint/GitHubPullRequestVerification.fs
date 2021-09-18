namespace Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification

    type PullRequestStatus = Open = 1  | Closed = 2 | Merged = 3

namespace Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages

    open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification

    type RequestCheckPullRequestStatus(pullRequestUri: string, etag: string) =
        member this.PullRequestUri = pullRequestUri
        member this.ETag = etag

    type ResponseCheckPullRequestStatus(pullRequestStatus: PullRequestStatus, etag: string) =
        member this.PullRequestStatus = pullRequestStatus
        member this.ETag = etag