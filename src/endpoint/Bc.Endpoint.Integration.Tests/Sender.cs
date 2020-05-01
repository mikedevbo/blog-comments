using System;
using System.Threading.Tasks;
using Bc.Common.Endpoint;
using Bc.Contracts.Internals.Endpoint.Operations;
using NServiceBus;
using NUnit.Framework;

namespace Bc.Endpoint.Integration.Tests
{
    public class Sender
    {
        private IEndpointInstance endpointInstance;

        [SetUp]
        public async Task SetUp()
        {
            var endpoint = EndpointCommon.GetEndpoint(
                "Sender.Tests",
                false,
                new EndpointCommonConfigurationProvider());

            var transport = endpoint.UseTransport<SqlServerTransport>();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(TakeCommentCmd).Assembly, "Bc.Endpoint");

            this.endpointInstance = await NServiceBus.Endpoint.Start(endpoint).ConfigureAwait(false);
        }

        [TearDown]
        public Task TearDown()
        {
            return this.endpointInstance.Stop();
        }

        [Test]
        public async Task TakeCommentCmd_Send_NoException()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            const string userName = "test_user";
            const string userEmail = "test_user_email";
            const string userWebsite = "test_user_website";
            const string fileName = "test_file_name";
            const string content = "test_comment";
            var addedDate = DateTime.Now;

            var message = new TakeCommentCmd(commentId, userName, userEmail, userWebsite, fileName, content, addedDate);

            // Act
            await this.endpointInstance.Send(message).ConfigureAwait(false);

            // Assert
            Assert.Pass();
        }
    }
}