namespace Components
{
    using System.Threading.Tasks;
    using Common;
    using Messages;
    using Messages.Commands;
    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.Mailer;

    public class SendEmailHandler : IHandleMessages<SendEmail>
    {
        private static readonly ILog Log = LogManager.GetLogger<SendEmailHandler>();
        private readonly IConfigurationManager configurationManager;

        public SendEmailHandler(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            var mail = new Mail
            {
                From = this.configurationManager.SmtpFrom,
                To = message.UserEmail,
                Subject = this.GetSubject(message.CommentResponseStatus),
                Body = this.GetBody(this.configurationManager.BlogDomainName, message.FileName, message.CommentResponseStatus)
            };

            return context.SendMail(mail);
        }

        public string GetSubject(CommentAnswerStatus status)
        {
            if (status == CommentAnswerStatus.Approved)
            {
                return Resource.ResponseAdded;
            }

            return string.Format("{0} {1}", Resource.Comment, Resource.Rejected);
        }

        public string GetBody(string blogDomainName, string fileName, CommentAnswerStatus status)
        {
            if (status != CommentAnswerStatus.Approved)
            {
                return string.Empty;
            }

            // Depend on Jekyll file format
            var s = fileName.Split('-');

            return string.Format(
                "{0} - {1}/{2}/{3}/{4}/{5}.html",
                Resource.Check,
                blogDomainName,
                s[0],
                s[1],
                s[2],
                s[3].Split('.')[0]);
        }
    }
}
