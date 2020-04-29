using System;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands
{
    public class CheckCommentAnswer
    {
        public CheckCommentAnswer(Guid commentId, string commentUri)
        {
            this.CommentId = commentId;
            this.CommentUri = commentUri;
        }

        public Guid CommentId { get; }

        public string CommentUri { get; }
    }
}