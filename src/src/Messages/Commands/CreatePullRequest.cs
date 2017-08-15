namespace Messages.Commands
{
    using System;

    public class CreatePullRequest
    {
        public Guid CommentId { get; set; }

        public string CommentBranchName { get; set; }

        public string BaseBranchName { get; set; }
    }
}
