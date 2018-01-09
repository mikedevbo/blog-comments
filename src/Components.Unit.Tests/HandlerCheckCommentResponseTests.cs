namespace Components.Unit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HandlerCheckCommentResponseTests
    {
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");
        private IConfigurationManager configurationManager;
        private IGitHubApi gitHubApi;

        [Test]
        public async Task Handle_CheckCommentResponse_PublishProperEvent()
        {
            // Arrange
            var message = new CheckCommentResponse { CommentId = this.id };
            var handler = this.GetHandler();
            var context = this.GetContext();

            // Act
            await handler.Handle(message, context).ConfigureAwait(false);

            // Assert
            var publishedMessage = context.PublishedMessages[0].Message as ICommentResponseAdded;
            Assert.IsNotNull(publishedMessage);
            Assert.True(publishedMessage.CommentId == this.id);
        }

        [TestCase(false, false, CommentResponseStatus.Rejected)]
        [TestCase(false, true, CommentResponseStatus.Approved)]
        [TestCase(true, false, CommentResponseStatus.NotAddded)]
        [TestCase(true, true, CommentResponseStatus.NotAddded)]
        public async Task GetCommentResponseStatus_input_expectedResult(
            bool isPullRequestOpen,
            bool isPullRequestMerged,
            CommentResponseStatus expectedResult)
        {
            // Arrange
            Func<Task<bool>> f1 = () => Task.Run(() => isPullRequestOpen);
            Func<Task<bool>> f2 = () => Task.Run(() => isPullRequestMerged);
            var handler = this.GetHandler();

            // Act
            var result = await handler.GetCommentResponseStatus(f1, f2).ConfigureAwait(false);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private HandlerCheckCommentResponse GetHandler()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new HandlerCheckCommentResponse(this.configurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
