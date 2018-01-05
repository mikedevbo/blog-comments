namespace Components
{
    using System.Threading.Tasks;
    using Common;
    using Messages.Commands;
    using NServiceBus;
    using NServiceBus.Logging;

    public class HandlerSendEmail : IHandleMessages<SendEmail>
    {
        private static ILog log = LogManager.GetLogger<HandlerSendEmail>();
        private readonly IConfigurationManager configurationManager;
        private readonly IEmailSender sendEmail;

        public HandlerSendEmail(IConfigurationManager configurationManager, IEmailSender sendEmail)
        {
            this.configurationManager = configurationManager;
            this.sendEmail = sendEmail;
        }

        public Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            if (this.configurationManager.NsbIsIntegrationTests)
            {
                log.Info($"Response status: {message.CommentResponseStatus}, Email sent to: {message.UserName}");
                return Task.CompletedTask;
            }

            return this.sendEmail.Send(message.UserName,  message.UserEmail, message.CommentResponseStatus);
        }
    }
}
