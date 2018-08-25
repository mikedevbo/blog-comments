namespace Components.Unit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Messages;
    using Messages.Commands;
    using Messages.Events;
    using Messages.Messages;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HandlerCommentSagaTests
    {
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");
        private readonly int timeoutMinutes = 30;
        private IConfigurationManager configurationManager;

        [Test]
        public async Task Handle_StartAddingComment_SendRequestCreateBranchWithProperData()
        {
            // Arrange
            var message = new StartAddingComment { CommentId = this.id };
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<RequestCreateBranch>(context);
            Assert.IsNotNull(sentMessage);
        }

        [Test]
        public async Task Handle_CreateBranchResponse_SendRequestAddCommentWithProperData()
        {
            // Arrange
            var message = Substitute.For<CreateBranchResponse>();
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<RequestAddComment>(context);
            Assert.IsNotNull(sentMessage);
        }

        [Test]
        public async Task Handle_AddCommentResponse_CreatePullRequestWithProperData()
        {
            // Arrange
            var message = Substitute.For<AddCommentResponse>();
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<RequestCreatePullRequest>(context);
            Assert.IsNotNull(sentMessage);
        }

        [Test]
        public void Handle_CreatePullRequestResponse_SendCheckCommentResponseTimeoutWithProperData()
        {
            Test.Saga<HandlerCommentSaga>()
                .ExpectTimeoutToBeSetIn<CreatePullRequestResponse>((state, span) =>
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
        public void Handle_CommentResponseStatusApproved_SendEmailWithProperDataAndCompleteSaga()
        {
            var message = Substitute.For<ICommentResponseAdded>();
            message.CommentResponse = new CommentResponse
            {
                ResponseStatus = CommentResponseStatus.Approved
            };

            Test.Saga<HandlerCommentSaga>()
                .ExpectSend<SendEmail>()
                .When(
                    sagaIsInvoked: (saga, context) =>
                    {
                        return saga.Handle(message, context);
                    })
                    .ExpectSagaCompleted();
        }

        [Test]
        public void Handle_CommentResponseStatusNotAdded_SendCheckCommentResponseTimeoutWithProperData()
        {
            var message = Substitute.For<ICommentResponseAdded>();
            message.CommentResponse = new CommentResponse
            {
                ResponseStatus = CommentResponseStatus.NotAddded
            };

            Test.Saga<HandlerCommentSaga>()
                .ExpectTimeoutToBeSetIn<ICommentResponseAdded>((state, span) =>
                {
                    return span == TimeSpan.FromDays(this.timeoutMinutes);
                });
        }

        [Test]
        public void Handle_CommentResponseStatusRejected_SendEmailWithProperDataAndCompleteSaga()
        {
            var message = Substitute.For<ICommentResponseAdded>();
            message.CommentResponse = new CommentResponse
            {
                ResponseStatus = CommentResponseStatus.Rejected
            };

            Test.Saga<HandlerCommentSaga>()
                .ExpectSend<SendEmail>()
                .When(
                    sagaIsInvoked: (saga, context) =>
                    {
                        return saga.Handle(message, context);
                    })
                .ExpectSagaCompleted();
        }

        private HandlerCommentSaga GetHandler()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();

            return new HandlerCommentSaga(this.configurationManager)
            {
                Data = new CommentSagaData
                {
                    CommentId = this.id,
                    UserName = @"test",
                    UserEmail = @"test@test.com",
                    UserWebsite = @"test.com",
                    FileName = @"test.txt",
                    Content = @"test comment",
                    BranchName = @"testBranch"
                }
            };
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
