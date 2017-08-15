namespace Components.Tests
{
    using System;
    using System.Threading.Tasks;
    using Components.GitHub;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HandlerCreatePullRequestTests
    {
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");
        private IComponentsConfigurationManager componentsConfigurationManager;
        private IGitHubApi gitHubApi;

        [Test]
        public async Task Handle_CreatePullRequest_PublishProperEvent()
        {
            // Arrange
            var message = new CreatePullRequest { CommentId = this.id };
            var handler = this.GetHandler();
            var context = this.GetContext();

            // Act
            await handler.Handle(message, context).ConfigureAwait(false);

            // Assert
            var publishedMessage = context.PublishedMessages[0].Message as IPullRequestCreated;
            Assert.IsNotNull(publishedMessage);
            Assert.True(publishedMessage.CommentId == this.id);
        }

        private HandlerCreatePullRequest GetHandler()
        {
            this.componentsConfigurationManager = Substitute.For<IComponentsConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new HandlerCreatePullRequest(this.componentsConfigurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
