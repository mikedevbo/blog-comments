namespace Messages.Commands
{
    using System;

    public class CheckCommentResponse
    {
        public Guid CommentId { get; set; }

        public string BranchName { get; set; }
    }
}
