namespace Bc.Contracts.Internals.Endpoint.ITOps.GitHubPullRequest.Messages
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