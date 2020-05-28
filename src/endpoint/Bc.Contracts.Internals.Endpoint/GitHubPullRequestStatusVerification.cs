using Bc.Contracts.Internals.Endpoint.ITOps.GitHub;

namespace Bc.Contracts.Internals.Endpoint
{
    namespace Messages
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
}