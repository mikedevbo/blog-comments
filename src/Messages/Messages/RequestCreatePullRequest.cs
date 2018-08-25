namespace Messages.Messages
{
    public class RequestCreatePullRequest
    {
        public string CommentBranchName { get; set; }

        public string BaseBranchName { get; set; }
    }
}
