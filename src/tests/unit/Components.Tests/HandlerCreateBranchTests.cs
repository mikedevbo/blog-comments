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
    public class HandlerCreateBranchTests
    {
        private IComponentsConfigurationManager componentsConfigurationManager;
        private IGitHubApi gitHubApi;
        private readonly Guid id = Guid.Parse(@"0C242B08-7704-499D-A9D8-184ED6D93988");

        [Test]
        public async Task Handle_CreateBranch_PublishProperEvent()
        {
            // Arrange
            var message = new CreateBranch { CommentId = id };
            var handler = this.GetHandler();
            var context = this.GetContext();

            // Act
            await handler.Handle(message, context);

            // Assert
            var publishedMessage = context.PublishedMessages[0].Message as IBranchCreated;
            Assert.IsNotNull(publishedMessage);
            Assert.True(publishedMessage.CommentId == this.id);
        }

        private HandlerCreateBranch GetHandler()
        {
            this.componentsConfigurationManager = Substitute.For<IComponentsConfigurationManager>();
            this.gitHubApi = Substitute.For<IGitHubApi>();

            return new HandlerCreateBranch(this.componentsConfigurationManager, this.gitHubApi);
        }

        private TestableMessageHandlerContext GetContext()
        {
            return new TestableMessageHandlerContext();
        }
    }
}
