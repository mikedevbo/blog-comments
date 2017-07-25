using Messages.Commands;
using NServiceBus.Testing;
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
        [Test]
        public async Task Handle_StartAddingComment_SendCreateGitHubBranchWithProperData()
        {
            // Arrange
            Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");
            var message = new StartAddingComment { CommentId = id };
            var saga = this.GetHandlerCommentSaga();
            var context = new TestableMessageHandlerContext();
            

            // Act
            await saga.Handle(message, context).ConfigureAwait(false);

            // Assert
            var sentMessage = context.SentMessages[0].Message as CreateGitHubBranch;
            Assert.IsNotNull(sentMessage);
            Assert.True(sentMessage.CommentId == id);
        }

        private HandlerCommentSaga GetHandlerCommentSaga()
        {
            return new HandlerCommentSaga();
        }
    }
}
