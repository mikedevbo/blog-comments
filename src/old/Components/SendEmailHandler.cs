namespace Components
{
    using System;
    using System.Linq;
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
            if (string.IsNullOrEmpty(message.UserEmail))
            {
                return Task.CompletedTask;
            }

            var mail = new Mail
            {
                From = this.configurationManager.SmtpFrom,
                To = message.UserEmail,
                Subject = this.GetSubject(message.CommentResponseStatus),
                Body = this.GetBody(this.configurationManager.BlogDomainName, message.FileName, message.CommentResponseStatus),
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

            // depend on Jekyll file format

            // the file name has format _posts/fileName.md
            // the first step is to remove _posts prefix
            var onlyFileName = fileName.Split('/')[1];

            var split = onlyFileName.Split('-').ToList();
            var year = split[0];
            var month = split[1];
            var day = split[2];

            // remove year, month and day
            split.RemoveRange(0, 3);

            var join = string.Join("-", split).Split('.')[0];
            var result = $"{Resource.Check} - {blogDomainName}/{year}/{month}/{day}/{join}.html";

            return result;
        }
    }
}
