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
                Subject = this.GetSubject(message.CommentResponseStatus),
                Body = this.GetBody(this.configurationManager.BlogDomainName, message.FileName, message.CommentResponseStatus)
            };

            return context.SendMail(mail);
        }

        public string GetSubject(CommentResponseStatus status)
        {
            var subject = string.Format("{0} - {1}", Resource.BlogName, Resource.Comment);

            if (status == CommentResponseStatus.Approved)
            {
                return string.Format("{0} {1}", subject, Resource.Approved);
            }

            return string.Format("{0} {1}", subject, Resource.Rejected);
        }

        public string GetBody(string blogDomainName, string fileName, CommentResponseStatus status)
        {
            if (status != CommentResponseStatus.Approved)
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
