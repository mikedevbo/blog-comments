using System;

namespace Bc.Contracts.Externals.Endpoint.CommentAnswer.Events
{
    public class CommentApproved
    {
        public CommentApproved(Guid commentId)
        {
            this.CommentId = commentId;
        }

        public Guid CommentId { get; }
    }
}