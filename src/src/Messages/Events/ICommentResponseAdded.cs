namespace Messages.Events
{
    using System;

    public interface ICommentResponseAdded
    {
        Guid CommentId { get; set; }

        CommentResponseStatus CommentResponseStatus { get; set; }
    }
}
