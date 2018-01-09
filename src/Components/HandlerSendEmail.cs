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

        public HandlerSendEmail(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
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
        }

        public string GetBody(string userName, CommentResponseStatus status)
        {
            ////TOTO: to implement
            return "test mail body : " + userName + " " + status;
        }
    }
}
