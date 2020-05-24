namespace Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands
{
    public class RegisterComment
    {
        public RegisterComment(CommentData commentData)
        {
            CommentData = commentData;
        }
        
        public CommentData CommentData { get; }
    }
}