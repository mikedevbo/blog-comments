using Components.GitHub;
using Messages.Commands;
using Messages.Events;
using NServiceBus.Testing;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Tests
{
    [TestFixture]
    public class HandlerCreatePullRequestTests
    {
        private IConfigurationManager configurationManager;
        private IGitHubApi gitHubApi;
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");

        [Test]
        public async Task Handle_CreatePullRequest_PublishProperEvent()
        {
            // Arrange
            var message = new CreatePullRequest { CommentId = id };
            var handler = this.GetHandlerCreatePullRequest();
            var context = this.GetTestableMessageHandlerContext();

            // Act
            await handler.Handle(message, context);

            // Assert
            var publishedMessage = context.PublishedMessages[0].Message as IPullRequestCreated;
            Assert.IsNotNull(publishedMessage);
            Assert.True(publishedMessage.CommentId == this.id);
        }

        private HandlerCreatePullRequest GetHandlerCreatePullRequest()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new HandlerCreatePullRequest(this.configurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetTestableMessageHandlerContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
