namespace Components
{
    using Messages;

    public interface IEmailSender
    {
        void Send(string userName, string userEmail, CommentResponseStatus status);
    }
}
