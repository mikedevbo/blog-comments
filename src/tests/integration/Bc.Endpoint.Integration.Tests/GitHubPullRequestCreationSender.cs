using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages;
using NServiceBus;
using NUnit.Framework;

namespace Bc.Endpoint.Integration.Tests
{
    [TestFixture]
    [Ignore("Only for manual tests")]
    public class GitHubPullRequestCreationSender
    {
        private IEndpointInstance endpointInstance;

        [SetUp]
        public async Task SetUp()
        {
            this.endpointInstance = await EndpointFactory.GetSenderEndpoint().ConfigureAwait(false);
        }

        [TearDown]
        public Task TearDown()
        {
            return this.endpointInstance.Stop();
        }

        [Test]
        public async Task RequestCreateGitHubPullRequest_Send_NoException()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            const string fileName = @"_posts/2018-05-27-test.md";
            const string content = "new_comment";
            var addedDate = DateTime.UtcNow;

            var message = new RequestCreateGitHubPullRequest(
                commentId,
                fileName,
                content,
                addedDate);

            // Act
            await this.endpointInstance.Send(message).ConfigureAwait(false);

            // Assert
            Assert.Pass();
        }
    }
}