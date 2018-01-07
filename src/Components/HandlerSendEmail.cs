namespace Components
{
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Messages;
    using Messages.Commands;
    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.Mailer;

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
            var mail = new Mail
            {
                From = this.configurationManager.SmtpFrom,
                To = message.UserEmail,
                Subject = "[testSubject]",
                Body = this.GetBody(message.UserName, message.CommentResponseStatus)
            };

            return context.SendMail(mail);

            /*
            if (this.configurationManager.NsbIsIntegrationTests)
            {
                log.Info($"Response status: {message.CommentResponseStatus}, Email sent to: {message.UserName}");
                return Task.CompletedTask;
            }

            return this.sendEmail.Send(message.UserName,  message.UserEmail, message.CommentResponseStatus);
            */
        }

        public string GetBody(string userName, CommentResponseStatus status)
        {
            ////TOTO: to implement
            return "test mail body : " + userName + " " + status;
        }
    }
}
