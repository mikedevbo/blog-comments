namespace Components.Integration.Tests
{
    using System.IO;
    using Common;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    [Ignore("only for manual tests")]
    public class SendEmailHandlerTests
    {
        private readonly IConfiguration config;
        private readonly IConfigurationManager configurationManager;

        public SendEmailHandlerTests()
        {
            this.config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", true, true)
                            .Build();

            this.configurationManager = new ConfigurationManager(this.config);
        }

        private SendEmailHandler GetHandlerSetEmail()
        {
            return new SendEmailHandler(this.configurationManager);
        }
    }
}
