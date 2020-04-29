using System;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Events
{
    public class CommentAnswerAdded
    {
        public CommentAnswerAdded(Guid commentId)
        {
            CommentId = commentId;
        }

        public Guid CommentId { get; }
    }
}