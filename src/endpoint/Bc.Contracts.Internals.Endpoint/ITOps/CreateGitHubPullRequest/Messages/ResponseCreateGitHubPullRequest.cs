using System;

namespace Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest.Messages
{
    public class ResponseCreateGitHubPullRequest
    {
        public ResponseCreateGitHubPullRequest(Guid commentId, string pullRequestUri)
        {
            this.CommentId = commentId;
            this.PullRequestUri = pullRequestUri;
        }

        public Guid CommentId { get; private set; }
        
        public string PullRequestUri { get; }
    }
}