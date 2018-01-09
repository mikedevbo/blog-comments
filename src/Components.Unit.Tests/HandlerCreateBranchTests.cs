namespace Components.Unit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HandlerCreateBranchTests
    {
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");
        private IConfigurationManager configurationManager;
        private IGitHubApi gitHubApi;

        [Test]
        public async Task Handle_CreateBranch_PublishProperEvent()
        {
            // Arrange
            var message = new CreateBranch { CommentId = this.id };
            var handler = this.GetHandler();
            var context = this.GetContext();

            // Act
            await handler.Handle(message, context).ConfigureAwait(false);

            // Assert
            var publishedMessage = context.PublishedMessages[0].Message as IBranchCreated;
            Assert.IsNotNull(publishedMessage);
            Assert.True(publishedMessage.CommentId == this.id);
        }

        private HandlerCreateBranch GetHandler()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new HandlerCreateBranch(this.configurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
