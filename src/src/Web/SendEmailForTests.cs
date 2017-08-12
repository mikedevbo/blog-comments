namespace Web
{
    using Components;
    using NServiceBus.Logging;
    using Simple.Data;

    /// <summary>
    /// The send email implementation for tests.
    /// </summary>
    /// <seealso cref="Components.ISendEmail" />
    public class SendEmailForTests : ISendEmail
    {
        private static ILog log = LogManager.GetLogger<SendEmailForTests>();
        private readonly IConfigurationManager configurationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailForTests"/> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        public SendEmailForTests(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        /// <summary>
        /// Sends this instance.
        /// </summary>
        public void Send()
        {
            Database.OpenConnection(this.configurationManager.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 5);

            log.Info("send e-mail");
        }
    }
}
