namespace Components
{
    using System.Threading.Tasks;
    using Messages.Commands;
    using NServiceBus;

    public class HandlerSendEmail : IHandleMessages<SendEmail>
    {
        private readonly IEmailSender sendEmail;

        public HandlerSendEmail(IEmailSender sendEmail)
        {
            this.sendEmail = sendEmail;
        }

        public Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            return this.sendEmail.Send(message.UserName,  message.UserEmail, message.CommentResponseStatus);
        }
    }
}
