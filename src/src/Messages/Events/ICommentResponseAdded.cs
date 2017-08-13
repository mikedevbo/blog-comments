namespace Messages.Events
{
    using System;

    public interface ICommentResponseAdded
    {
        Guid CommentId { get; set; }

        CommentResponseState CommentResponseState { get; set; }
    }
}
