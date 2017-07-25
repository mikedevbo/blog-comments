using Messages.Commands;
using Messages.Events;
using NServiceBus.Testing;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Tests
{
    [TestFixture]
    public class HandlerCommentSagaTests
    {
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");

        [Test]
        public async Task Handle_StartAddingComment_SendCreateGitHubBranchWithProperData()
        {
            // Arrange
            var message = new StartAddingComment { CommentId = this.id };
            var saga = this.GetHandlerCommentSaga();
            var context = this.GetTestableMessageHandlerContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<CreateGitHubBranch>(context);
            Assert.IsNotNull(sentMessage);
            Assert.True(sentMessage.CommentId == this.id);
        }

        [Test]
        public async Task Handle_GitHubBranchCreated_SendAddCommentWithProperData()
        {
            // Arrange
            var message = Substitute.For<IGitHubBranchCreated>();
            message.CommentId = this.id;
            var saga = this.GetHandlerCommentSaga();
            var context = this.GetTestableMessageHandlerContext();

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = this.GetSentMessage<AddComment>(context);
            Assert.IsNotNull(sentMessage);
            Assert.True(sentMessage.CommentId == this.id);
        }

        private HandlerCommentSaga GetHandlerCommentSaga()
        {
            return new HandlerCommentSaga();
        }

        private TestableMessageHandlerContext GetTestableMessageHandlerContext()
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
