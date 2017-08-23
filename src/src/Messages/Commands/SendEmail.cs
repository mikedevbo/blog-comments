namespace Messages.Commands
{
    public class SendEmail
    {
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public CommentResponseStatus CommentResponseStatus { get; set; }
    }
}
