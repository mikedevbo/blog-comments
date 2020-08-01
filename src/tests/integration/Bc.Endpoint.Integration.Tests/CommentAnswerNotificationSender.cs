using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswerNotification.Commands;
using NServiceBus;
using NUnit.Framework;

namespace Bc.Endpoint.Integration.Tests
{
    [TestFixture]
    [Ignore("Only for manual tests")]
    public class CommentAnswerNotificationSender
    {
        private IEndpointInstance endpointInstance;
        private readonly Guid commentId = Guid.Parse("9a3a0fdc-3a5a-4862-845c-09792d2b6f9a");

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
        public async Task RegisterCommentNotification_Send_NoException()
        {
            // Arrange
            const string userEmail = "test@test.com";
            const string articleFileName = @"_posts/2018-05-27-test.md";

            var message = new RegisterCommentNotification(
                commentId,
                userEmail,
                articleFileName);

            // Act
            await this.endpointInstance.Send(message).ConfigureAwait(false);

            // Assert
            Assert.Pass();
        }

        [Test]
        public async Task NotifyAboutCommentAnswer_Send_NoException()
        {
            // Arrange
            const bool isApproved = true;

            var message = new NotifyAboutCommentAnswer(commentId, isApproved);

            // Act
            await this.endpointInstance.Send(message).ConfigureAwait(false);

            // Assert
            Assert.Pass();
        }
    }
}