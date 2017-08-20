namespace Components
{
    using Messages;

    public interface ISendEmail
    {
        void Send(string emailAddress, CommentResponseStatus status);
    }
}
