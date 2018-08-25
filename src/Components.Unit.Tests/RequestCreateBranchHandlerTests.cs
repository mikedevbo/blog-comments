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
    public class RequestCreateBranchHandlerTests
    {
        private IConfigurationManager configurationManager;
        private IGitHubApi gitHubApi;

        [Test]
        public async Task Handle_RequestCreateBranch_ReplayProperResponse()
        {
            // Arrange
            var message = new RequestCreateBranch();
            var handler = this.GetHandler();
            var context = this.GetContext();

            // Act
            await handler.Handle(message, context).ConfigureAwait(false);

            // Assert
            var repliedMessage = context.RepliedMessages[0].Message as CreateBranchResponse;
            Assert.IsNotNull(repliedMessage);
        }

        private RequestCreateBranchHandler GetHandler()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new RequestCreateBranchHandler(this.configurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
