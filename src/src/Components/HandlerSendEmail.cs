namespace Components
{
    using System.Threading.Tasks;
    using Messages.Commands;
    using NServiceBus;

    public class HandlerSendEmail : IHandleMessages<SendEmail>
    {
        private readonly ISendEmail sendEmail;

        public HandlerSendEmail(ISendEmail sendEmail)
        {
            this.sendEmail = sendEmail;
        }

        public Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            ////TODO: to implement -> can be awaitable ?
            this.sendEmail.Send(message.EmailAddress, message.CommentResponseStatus);
            return Task.CompletedTask;
        }
    }
}
