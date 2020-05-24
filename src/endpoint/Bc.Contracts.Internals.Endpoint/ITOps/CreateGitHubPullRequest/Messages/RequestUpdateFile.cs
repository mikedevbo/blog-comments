namespace Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest.Messages
{
    public class RequestUpdateFile
    {
        public RequestUpdateFile(string branchName, string fileName, string content)
        {
            this.BranchName = branchName;
            this.FileName = fileName;
            this.Content = content;
        }

        public string BranchName { get; }
        
        public string FileName { get; }
        
        public string Content { get; }
    }
}