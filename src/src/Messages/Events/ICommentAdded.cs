namespace Messages.Events
{
    using System;

    public interface ICommentAdded
    {
        Guid CommentId { get; set; }
    }
}
