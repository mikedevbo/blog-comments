namespace Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest.Messages
{
    public class ResponseCreatePullRequest
    {
        public ResponseCreatePullRequest(string pullRequestUri)
        {
            this.PullRequestUri = pullRequestUri;
        }

        public string PullRequestUri { get;  }
    }
}