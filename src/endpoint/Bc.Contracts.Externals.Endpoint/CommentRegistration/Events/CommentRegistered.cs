using System;

namespace Bc.Contracts.Externals.Endpoint.CommentRegistration.Events
{
    public class CommentRegistered
    {
        public CommentRegistered(Guid commentId, string commentUri)
        {
            this.CommentId = commentId;
            this.CommentUri = commentUri;
        }

        public Guid CommentId { get; }

        public string CommentUri { get; }        
    }
}