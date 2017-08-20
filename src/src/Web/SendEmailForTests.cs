namespace Web
{
    using Components;
    using Messages;
    using NServiceBus.Logging;
    using Simple.Data;

    public class SendEmailForTests : ISendEmail
    {
        private static ILog log = LogManager.GetLogger<SendEmailForTests>();
        private readonly IConfigurationManager configurationManager;

        public SendEmailForTests(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public void Send(string emailAddress, CommentResponseStatus status)
        {
            Database.OpenConnection(this.configurationManager.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 5);

            log.Info(string.Format("send e-mail: {0}", status));
        }
    }
}
