namespace Bc.Contracts.Internals.Endpoint.CommentRegistration
{
    public class CreatePullRequestCmd
    {
        public CreatePullRequestCmd(string branchName, CommentData commentData)
        {
            this.BranchName = branchName;
            this.CommentData = commentData;
        }

        public string BranchName { get; }

        public CommentData CommentData { get; }
    }
}