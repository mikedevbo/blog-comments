namespace Components.Unit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Messages;
    using Messages.Commands;
    using Messages.Messages;
    using NServiceBus.Testing;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class CommentPolicyTests
    {
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");
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
        public async Task Handle_AddCommentResponse_SendRequestCreatePullRequestWithProperData()
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
        public async Task Handle_CreatePullRequestResponse_TimeoutCheckCommentAnswerTimeoutWithProperData()
        {
            // Arrange
            var message = Substitute.For<CreatePullRequestResponse>();
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var timeoutMessage = this.GetTimeoutMessage<CheckCommentAnswerTimeout>(context);
            Assert.IsNotNull(timeoutMessage);
        }

        [Test]
        public async Task Handle_CheckCommentAnswerTimeout_SendRequestCheckCommentAnswerWithProperData()
        {
            // Arrange
            var message = Substitute.For<CheckCommentAnswerTimeout>();
            var saga = this.GetHandler();
            saga.Data.ETag = "testETag";
            saga.Data.PullRequestLocation = "testLocation";
            var context = this.GetContext();

            // Act
            await saga.Timeout(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<RequestCheckCommentAnswer>(context);
            Assert.IsNotNull(sentMessage);
            Assert.True(sentMessage.Etag == saga.Data.ETag);
            Assert.True(sentMessage.PullRequestUri == saga.Data.PullRequestLocation);
        }

        [TestCase(CommentAnswerStatus.Rejected)]
        public async Task Handle_CheckCommentAnswerResponse_CompleteSaga(CommentAnswerStatus status)
        {
            // Arrange
            var message = Substitute.For<CheckCommentAnswerResponse>();
            message.Status = status;
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            Assert.True(saga.Completed);
        }

        [TestCase(CommentAnswerStatus.Approved)]
        public async Task Handle_CheckCommentAnswerResponse_SendEmailWithProperDataAndCompleteSaga(CommentAnswerStatus status)
        {
            // Arrange
            var message = Substitute.For<CheckCommentAnswerResponse>();
            message.Status = status;
            var saga = this.GetHandler();
            saga.Data.UserName = "testUser";
            saga.Data.UserEmail = "testEmail";
            saga.Data.FileName = "testFileName";
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<SendEmail>(context);
            Assert.IsNotNull(sentMessage);
            Assert.True(sentMessage.UserName == saga.Data.UserName);
            Assert.True(sentMessage.UserEmail == saga.Data.UserEmail);
            Assert.True(sentMessage.FileName == saga.Data.FileName);
            Assert.True(sentMessage.CommentResponseStatus == message.Status);
            Assert.True(saga.Completed);
        }

        [TestCase(CommentAnswerStatus.NotAddded)]
        public async Task Handle_CheckCommentAnswerResponse_TimeoutCheckCommentAnswerTimeoutWithProperDataAndContinueSaga(CommentAnswerStatus status)
        {
            // Arrange
            var message = Substitute.For<CheckCommentAnswerResponse>();
            message.Status = status;
            var saga = this.GetHandler();
            var context = this.GetContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var timeoutMessage = this.GetTimeoutMessage<CheckCommentAnswerTimeout>(context);
            Assert.IsNotNull(timeoutMessage);
            Assert.False(saga.Completed);
        }

        private CommentPolicy GetHandler()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();

            return new CommentPolicy(this.configurationManager)
            {
                Data = new CommentPolicy.CommentPolicyData
                {
                    CommentId = this.id,
                    UserName = @"test",
                    UserEmail = @"test@test.com",
                    UserWebsite = @"test.com",
                    FileName = @"test.txt",
                    Content = @"test comment",
                    BranchName = @"testBranch",
                },
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

        private TSentMessage GetTimeoutMessage<TSentMessage>(TestableMessageHandlerContext context)
            where TSentMessage : class
        {
            return context.TimeoutMessages[0].Message as TSentMessage;
        }
    }
}
