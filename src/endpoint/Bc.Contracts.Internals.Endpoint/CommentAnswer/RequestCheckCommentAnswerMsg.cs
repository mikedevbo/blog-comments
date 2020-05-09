namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public class RequestCheckCommentAnswerMsg
    {
        public RequestCheckCommentAnswerMsg(string commentUri, string etag)
        {
            this.CommentUri = commentUri;
            this.Etag = etag;
        }
        
        public string CommentUri { get; }

        public string Etag { get; }
    }
}