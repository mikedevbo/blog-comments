namespace Messages.Events
{
    using System;

    public interface IBranchCreated
    {
        Guid CommentId { get; set; }

        string CreatedBranchName { get; set; }
    }
}
