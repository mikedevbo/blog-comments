namespace Messages.Events
{
    using System;

    public interface ICommentResponseAdded
    {
        Guid CommentId { get; set; }

        CommentResponse CommentResponse { get; set; }
    }
}
