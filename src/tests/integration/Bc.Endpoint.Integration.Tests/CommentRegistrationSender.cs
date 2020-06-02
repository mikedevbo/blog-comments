using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentRegistration.Commands;
using NServiceBus;
using NUnit.Framework;

namespace Bc.Endpoint.Integration.Tests
{
    [TestFixture]
    public class CommentRegistrationSender
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
        public async Task RegisterComment_Send_NoException()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            const string userName = "test_user";
            const string userWebsite = "test_user_website";
            const string userComment = "new_comment";
            const string articleFileName = @"_posts/2018-05-27-test.md";
            var addedDate = DateTime.UtcNow;

            var message = new RegisterComment(
                commentId,
                userName,
                userWebsite,
                userComment,
                articleFileName,
                addedDate);

            // Act
            await this.endpointInstance.Send(message).ConfigureAwait(false);

            // Assert
            Assert.Pass();
        }        
    }
}