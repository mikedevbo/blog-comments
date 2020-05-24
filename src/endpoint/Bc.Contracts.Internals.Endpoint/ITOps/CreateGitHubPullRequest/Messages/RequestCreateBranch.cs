using System;

namespace Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest.Messages
{
    public class RequestCreateBranch
    {
        public RequestCreateBranch(DateTime addedDate)
        {
            AddedDate = addedDate;
        }
        
        public DateTime AddedDate { get; }
    }
}