namespace Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification.Messages
{
    public class RequestCheckPullRequestStatus
    {
        public RequestCheckPullRequestStatus(string pullRequestUri, string etag)
        {
            this.PullRequestUri = pullRequestUri;
            this.Etag = etag;
        }
        
        public string PullRequestUri { get; }

        public string Etag { get; }
    }
}