namespace Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification.Messages
{
    public class ResponseCheckPullRequestStatus
    {
        public ResponseCheckPullRequestStatus(PullRequestStatus pullRequestStatus, string etag)
        {
            this.PullRequestStatus = pullRequestStatus;
            this.ETag = etag;
        }

        public PullRequestStatus PullRequestStatus { get; }

        public string ETag { get;  }
    }
}