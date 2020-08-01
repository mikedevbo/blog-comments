using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentTaking.Commands;
using NServiceBus;
using NUnit.Framework;

namespace Bc.Endpoint.Integration.Tests
{
    [TestFixture]
    [Ignore("Only for manual tests")]
    public class CommentTakingSender
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