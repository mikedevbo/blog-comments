namespace Components.Unit.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Messages;
    using Messages.Messages;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    public class RequestCheckCommentAnswerHandlerTests
    {
        private IConfigurationManager configurationManager;
        private IGitHubApi gitHubApi;

        [Test]
        public async Task Handle_RequestCheckCommentAnswer_ReplayProperEvent()
        {
            // Arrange
            var message = new RequestCheckCommentAnswer();
            var handler = this.GetHandler();
            var context = this.GetContext();

            // Act
            await handler.Handle(message, context).ConfigureAwait(false);

            // Assert
            var replieddMessage = context.RepliedMessages[0].Message as CheckCommentAnswerResponse;
            Assert.IsNotNull(replieddMessage);
        }

        [TestCase(false, false, CommentAnswerStatus.Rejected)]
        [TestCase(false, true, CommentAnswerStatus.Approved)]
        [TestCase(true, false, CommentAnswerStatus.NotAddded)]
        [TestCase(true, true, CommentAnswerStatus.NotAddded)]
        public async Task GetCommentAnswer_input_expectedResult(
            bool isPullRequestOpen,
            bool isPullRequestMerged,
            CommentAnswerStatus expectedResult)
        {
            // Arrange
            const string etagResult = "1234";
            Func<Task<(bool result, string etag)>> f1 = () => Task.Run(() => (isPullRequestOpen, etagResult));
            Func<Task<bool>> f2 = () => Task.Run(() => isPullRequestMerged);
            var handler = this.GetHandler();

            // Act
            var result = await handler.GetCommentAnswer(f1, f2).ConfigureAwait(false);

            // Assert
            Assert.That(result.Status, Is.EqualTo(expectedResult));
            Assert.That(result.ETag, Is.EqualTo(etagResult));
        }

        private RequestCheckCommentAnswerHandler GetHandler()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new RequestCheckCommentAnswerHandler(this.configurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
