namespace Components.Unit.Tests
{
    using System;
    using Common;
    using Messages;
    using NSubstitute;
    using NUnit.Framework;

    public class SendEmailHandlerTests
    {
        private IConfigurationManager configurationManager;

        [TestCase(CommentAnswerStatus.Approved)]
        [TestCase(CommentAnswerStatus.Rejected)]
        public void GetSubject_Execute_ShowResult(CommentAnswerStatus responseStatus)
        {
            // Arrange
            var handler = this.GetHandlerSetEmail();

            // Act
            var result = handler.GetSubject(responseStatus);

            // Assert
            Console.WriteLine(result);
        }

        [Test]
        public void GetBody_Execute_ShowResult()
        {
            // Arrange
            var handler = this.GetHandlerSetEmail();

            // Act
            var result = handler.GetBody("someBlogDomainName", "2018-02-10-someFileName.md", CommentAnswerStatus.Approved);

            // Assert
            Console.WriteLine(result);
        }

        private SendEmailHandler GetHandlerSetEmail()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();

            return new SendEmailHandler(this.configurationManager);
        }
    }
}
