using System;

namespace Bc.Contracts.Internals.Endpoint.CommentRegistration
{
    public class CommentAddedEvt
    {
        public CommentAddedEvt(Guid commentId, string commentUri)
        {
            this.CommentId = commentId;
            this.CommentUri = commentUri;
        }

        public Guid CommentId { get; }

        public string CommentUri { get; }        
    }
}