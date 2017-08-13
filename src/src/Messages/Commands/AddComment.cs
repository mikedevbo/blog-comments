namespace Messages.Commands
{
    using System;

    public class AddComment
    {
        public Guid CommentId { get; set; }

        public string BranchName { get; set; }

        public string FileName { get; set; }

        public string Comment { get; set; }
    }
}
