namespace Messages.Commands
{
    public class SendEmail
    {
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string FileName { get; set; }

        public CommentAnswerStatus CommentResponseStatus { get; set; }
    }
}
