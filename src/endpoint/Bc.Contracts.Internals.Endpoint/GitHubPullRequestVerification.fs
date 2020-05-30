namespace Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages

type RequestCheckPullRequestStatus(pullRequestUri: string, etag: string) =
    member this.PullRequestUri = pullRequestUri
    member this.ETag = etag

type PullRequestStatus = Open = 1  | Closed = 2 | Merged = 2

type ResponseCheckPullRequestStatus(pullRequestStatus: PullRequestStatus, etag: string) =
    member this.PullRequestStatus = pullRequestStatus
    member this.ETag = etag
    
namespace Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification

open System.Threading.Tasks
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestVerification.Messages

type IPolicyLogic =
    abstract member CheckPullRequestStatus : pullRequestUri:string -> etag:string -> Task<ResponseCheckPullRequestStatus>