namespace Components
{
    using System.Threading.Tasks;
    using Messages;

    public interface IEmailSender
    {
        Task Send(string userName, string userEmail, CommentResponseStatus status);
    }
}
