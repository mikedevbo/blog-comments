using Messages;
using Messages.Messages;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Components.Logic.CSharp.Integration.Tests
{
    [TestFixture]
    public class CommentPolicyLogicTests
    {
        [Test]
        public async Task CreateRepositoryBranch_Execute_NoException()
        {
            // Arrange
            var message = new RequestCreateBranch();
            var api = this.GetCommentPolicyLogic();

            // Act
            var result = await api.CreateRepositoryBranch(message).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        [Test]
        public async Task UpdateFile_Execute_NoException()
        {
            // Arrange
            var message = new RequestAddComment();
            var api = this.GetCommentPolicyLogic();

            // Act
            var result = await api.UpdateFile(message).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        [Test]
        public async Task CreatePullRequest_Execute_NoException()
        {
            // Arrange
            var message = new RequestCreatePullRequest();
            var api = this.GetCommentPolicyLogic();

            // Act
            var result = await api.CreatePullRequest(message).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        [Test]
        public async Task CheckCommentAnswer_Execute_NoException()
        {
            // Arrange
            var message = new RequestCheckCommentAnswer();
            var api = this.GetCommentPolicyLogic();

            // Act
            var result = await api.CheckCommentAnswer(message).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Console.WriteLine(result);
        }

        private ICommentPolicyLogic GetCommentPolicyLogic()
        {
            return new CommentPolicyLogicFake();
        }
    }
}
