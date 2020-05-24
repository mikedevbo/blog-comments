namespace Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest.Messages
{
    public class ResponseCreateBranch
    {
        public ResponseCreateBranch(string branchName)
        {
            this.BranchName = branchName;
        }

        public string BranchName { get; }
    }
}