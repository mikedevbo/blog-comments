namespace Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands
{
    public class CreatePullRequest
    {
        public CreatePullRequest(string branchName, CommentData commentData)
        {
            this.BranchName = branchName;
            this.CommentData = commentData;
        }
    
        public string BranchName { get; }
    
        public CommentData CommentData { get; }
    }
}