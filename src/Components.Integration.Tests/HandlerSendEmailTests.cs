namespace Components.Integration.Tests
{
    using System;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    [Ignore("only for manual tests")]
    public class HandlerSendEmailTests
    {
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

        private HandlerSendEmail GetHandlerSetEmail()
        {
            return new HandlerSendEmail(new Common.ConfigurationManager());
        }
    }
}
