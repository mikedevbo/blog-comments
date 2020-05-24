using System;
using System.Threading.Tasks;
using Bc.Contracts.Externals.Endpoint.ITOps.CreateGitHubPullRequest.Messages;
using NServiceBus;
using NUnit.Framework;

namespace Bc.Endpoint.Integration.Tests.ITOps.CreateGitHubPullRequest
{
    [TestFixture]
    public class Sender
    {
        private IEndpointInstance endpointInstance;

        [SetUp]
        public async Task SetUp()
        {
            this.endpointInstance = await Factory.GetSenderEndpoint().ConfigureAwait(false);
        }

        [TearDown]
        public Task TearDown()
        {
            return this.endpointInstance.Stop();
        }

        [Test]
        public async Task RequestCreatePullRequest_Send_NoException()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            const string userName = "test_user";
            const string userWebsite = "test_user_website";
            const string fileName = @"_posts/2018-05-27-test.md";
            const string content = "new_comment";
            var addedDate = DateTime.UtcNow;

            var message = new RequestCreatePullRequest(commentId, userName, userWebsite, fileName, content, addedDate);

            // Act
            await this.endpointInstance.Send(message).ConfigureAwait(false);

            // Assert
            Assert.Pass();
        }        
    }
}