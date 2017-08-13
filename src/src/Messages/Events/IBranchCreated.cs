namespace Messages.Events
{
    using System;

    public interface IBranchCreated
    {
        Guid CommentId { get; set; }
    }
}
