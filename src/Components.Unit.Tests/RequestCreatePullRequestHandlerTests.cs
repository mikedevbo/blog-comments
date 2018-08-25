namespace Components.Unit.Tests
{
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages.Messages;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class RequestCreatePullRequestHandlerTests
    {
        private IConfigurationManager configurationManager;
        private IGitHubApi gitHubApi;

        [Test]
        public async Task Handle_RequestCreatePullRequest_ReplayProperEvent()
        {
            // Arrange
            var message = new RequestCreatePullRequest();
            var handler = this.GetHandler();
            var context = this.GetContext();

            this.gitHubApi
                .CreatePullRequest(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
                .ReturnsForAnyArgs(@"https://test/test");

            // Act
            await handler.Handle(message, context).ConfigureAwait(false);

            // Assert
            var repliedMessage = context.RepliedMessages[0].Message as CreatePullRequestResponse;
            Assert.IsNotNull(repliedMessage);
        }

        private RequestCreatePullRequestHandler GetHandler()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new RequestCreatePullRequestHandler(this.configurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
