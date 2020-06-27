namespace Components.Unit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages.Messages;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class RequestAddCommentHandlerTests
    {
        private IConfigurationManager configurationManager;
        private IGitHubApi gitHubApi;

        [Test]
        public async Task Handle_RequestAddComment_ReplayProperEvent()
        {
            // Arrange
            var message = new RequestAddComment();
            var handler = this.GetHandler();
            var context = this.GetContext();

            // Act
            await handler.Handle(message, context).ConfigureAwait(false);

            // Assert
            var repliedMessage = context.RepliedMessages[0].Message as AddCommentResponse;
            Assert.IsNotNull(repliedMessage);
        }

        [TestCase("user", null, "**user**")]
        [TestCase("user", "", "**user**")]
        [TestCase("user", "webSite", "[user](webSite)")]
        public void FormatUserName_ForParameters_ExpectedResult(string userName, string userWebSite, string expectedResult)
        {
            // Arrange
            var handler = this.GetHandler();

            // Act
            var result = handler.FormatUserName(userName, userWebSite);

            // Assert
            Assert.True(result == expectedResult);
        }

        private RequestAddCommentHandler GetHandler()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new RequestAddCommentHandler(this.configurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
