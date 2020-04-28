using System;
using Common;
using Messages.Messages;

namespace Components.Integration.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using NServiceBus;
    using NUnit.Framework;

    [TestFixture]
    //[Ignore("only for manual tests")]
    public class SendTestMessages
    {
        private IEndpointInstance endpointInstance;

        [SetUp]
        public async Task SetUp()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.send_test_message.json", false, true)
                .Build();

            var configurationManager = new ConfigurationManager(configuration);
            var endpointInitializer = new EndpointInitializer(configurationManager);
            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);
            endpointInitializer.Initialize(endpointConfiguration, false);

            this.endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        [TearDown]
        public Task TearDown()
        {
            return this.endpointInstance.Stop();
        }

        [Test]
        public async Task RequestCreateBranch_Send_NoExceptions()
        {
            // Arrange
            var message = new RequestCreateBranch(DateTime.Now);

            // Act
            await this.endpointInstance.Send(message).ConfigureAwait(false);

            // Assert
            Assert.Pass();
        }
    }
}