using System;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public class CheckCommentAnswerCmd
    {
        public CheckCommentAnswerCmd(Guid commentId, string commentUri)
        {
            this.CommentId = commentId;
            this.CommentUri = commentUri;
        }

        public Guid CommentId { get; }

        public string CommentUri { get; }        
    }
}