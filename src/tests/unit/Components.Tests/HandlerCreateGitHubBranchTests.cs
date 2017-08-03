using Components.GitHub;
using Components.GitHub.Dto;
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
    public class HandlerCreateGitHubBranchTests
    {
        private IConfigurationManager configurationManager;
        private IGitHubApi gitHubApi;
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");

        [Test]
        public async Task Handle_CreateGitHubBranch_PublishProperEvent()
        {
            // Arrange
            var message = new CreateGitHubBranch { CommentId = id };
            var handler = this.GetHandlerCreateGitHubBranch();
            var context = this.GetTestableMessageHandlerContext();

            // Act
            await handler.Handle(message, context);

            // Assert
            var publishedMessage = context.PublishedMessages[0].Message as IGitHubBranchCreated;
            Assert.IsNotNull(publishedMessage);
            Assert.True(publishedMessage.CommentId == this.id);
        }

        private HandlerCreateGitHubBranch GetHandlerCreateGitHubBranch()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();
            //this.gitHubApi
            //    .GetRepository(string.Empty, string.Empty, string.Empty)
            //    .ReturnsForAnyArgs(new Repository
            //    {
            //        Ref = "test",
            //        Url = "test",
            //        Object = new GitHub.Dto.Object
            //        {
            //            Sha = "test"
            //        }
            //    });

            return new HandlerCreateGitHubBranch(this.configurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetTestableMessageHandlerContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
