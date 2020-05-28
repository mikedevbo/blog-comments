using System;

namespace Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
{
    public class CommentRejected
    {
        public CommentRejected(Guid commentId)
        {
            this.CommentId = commentId;
        }

        public Guid CommentId { get; }
    }
}