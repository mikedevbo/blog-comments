namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages
{
    public class RequestCheckCommentAnswer
    {
        public RequestCheckCommentAnswer(string commentUri, string etag)
        {
            this.CommentUri = commentUri;
            this.Etag = etag;
        }
        
        public string CommentUri { get; }

        public string Etag { get; }
    }
}