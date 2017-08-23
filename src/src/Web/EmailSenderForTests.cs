namespace Web
{
    using Components;
    using Messages;
    using NServiceBus.Logging;
    using Simple.Data;

    public class EmailSenderForTests : IEmailSender
    {
        private static ILog log = LogManager.GetLogger<EmailSenderForTests>();
        private readonly IConfigurationManager configurationManager;

        public EmailSenderForTests(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public void Send(string userName, string userEmail, CommentResponseStatus status)
        {
            Database.OpenConnection(this.configurationManager.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 5);

            log.Info(string.Format("send e-mail: {0}", status));
        }
    }
}
