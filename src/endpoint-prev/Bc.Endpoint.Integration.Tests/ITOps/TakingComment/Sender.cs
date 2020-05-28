using System;
using System.Threading.Tasks;
using Bc.Common.Endpoint;
using Bc.Contracts.Internals.Endpoint.ITOps.TakeComment.Commands;
using NServiceBus;
using NUnit.Framework;

namespace Bc.Endpoint.Integration.Tests.ITOps.TakingComment
{
    [TestFixture]
    public class Sender
    {
        private IEndpointInstance endpointInstance;

        [SetUp]
        public async Task SetUp()
        {
            const string bcEndpointAssemblyName = "Bc.Endpoint";

            var endpoint = EndpointCommon.GetEndpoint(
                "Sender.Tests",
                false,
                new EndpointCommonConfigurationProvider());

            var scanner = endpoint.AssemblyScanner();
            scanner.ExcludeAssemblies(bcEndpointAssemblyName);

            var transport = endpoint.UseTransport<SqlServerTransport>();
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(TakeComment).Assembly, bcEndpointAssemblyName);

            this.endpointInstance = await NServiceBus.Endpoint.Start(endpoint).ConfigureAwait(false);
        }

        [TearDown]
        public Task TearDown()
        {
            return this.endpointInstance.Stop();
        }

        [Test]
        public async Task TakeComment_Send_NoException()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            const string userName = "test_user";
            const string userEmail = "test_user_email";
            const string userWebsite = "test_user_website";
            const string fileName = @"_posts/2018-05-27-test.md";
            const string content = "new_comment";
            var addedDate = DateTime.UtcNow;

            var message = new TakeComment(commentId, userName, userEmail, userWebsite, fileName, content, addedDate);

            // Act
            await this.endpointInstance.Send(message).ConfigureAwait(false);

            // Assert
            Assert.Pass();
        }
    }
}