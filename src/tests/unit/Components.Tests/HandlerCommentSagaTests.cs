namespace Components.Tests
{
    using System;
    using System.Threading.Tasks;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HandlerCommentSagaTests
    {
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");
        private readonly int timeoutMinutes = 30;

        [Test]
        public async Task Handle_StartAddingComment_SendCreateBranchWithProperData()
        {
            // Arrange
            var message = new StartAddingComment { CommentId = this.id };
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<CreateBranch>(context);
            Assert.IsNotNull(sentMessage);
            Assert.True(sentMessage.CommentId == this.id);
        }

        [Test]
        public async Task Handle_GitHubBranchCreated_SendAddCommentWithProperData()
        {
            // Arrange
            var message = Substitute.For<IBranchCreated>();
            message.CommentId = this.id;
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<AddComment>(context);
            Assert.IsNotNull(sentMessage);
            Assert.True(sentMessage.CommentId == this.id);
        }

        [Test]
        public async Task Handle_CommentAdded_CreatePullRequestWithProperData()
        {
            // Arrange
            var message = Substitute.For<ICommentAdded>();
            message.CommentId = this.id;
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<CreatePullRequest>(context);
            Assert.IsNotNull(sentMessage);
            Assert.True(sentMessage.CommentId == this.id);
        }

        [Test]
        public void Handle_PullRequestCreated_SendCheckCommentResponseTimeoutWithProperData()
        {
            Test.Saga<HandlerCommentSaga>()
                .ExpectTimeoutToBeSetIn<IPullRequestCreated>((state, span) =>
                    {
                        return span == TimeSpan.FromDays(this.timeoutMinutes);
                    });
        }

        [Test]
        public void Handle_SagaTimeout_SendCheckCommentResponseWithProperData()
        {
            Test.Saga<HandlerCommentSaga>()
                .WhenSagaTimesOut()
                .ExpectSend<CheckCommentResponse>();
        }

        [Test]
        public void Handle_CommentResponseAdded_WhenCommentAddedThenSendEmailWithProperDataAndCompleteSaga()
        {
            var message = Substitute.For<ICommentResponseAdded>();
            message.CommentResponseState = CommentResponseState.Added;

            Test.Saga<HandlerCommentSaga>()
                .ExpectSend<SendEmail>()
                .When(
                    sagaIsInvoked: (saga, context) =>
                    {
                        return saga.Handle(message, context);
                    })
                .AssertSagaCompletionIs(true);
        }

        [Test]
        public void Handle_CommentResponseAdded_WhenCommentNotAddedThenSendCheckCommentResponseTimeoutWithProperData()
        {
            var message = Substitute.For<ICommentResponseAdded>();
            message.CommentResponseState = CommentResponseState.NotAddded;

            Test.Saga<HandlerCommentSaga>()
                .ExpectTimeoutToBeSetIn<ICommentResponseAdded>((state, span) =>
                {
                    return span == TimeSpan.FromDays(this.timeoutMinutes);
                });
        }

        private HandlerCommentSaga GetHandler()
        {
            return new HandlerCommentSaga();
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }

        private TSentMessage GetSentMessage<TSentMessage>(TestableMessageHandlerContext context)
            where TSentMessage : class
        {
            return context.SentMessages[0].Message as TSentMessage;
        }
    }
}
