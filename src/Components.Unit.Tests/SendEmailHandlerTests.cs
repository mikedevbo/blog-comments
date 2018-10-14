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

        [TestCase("blogDomainName", "2018-02-10-title_subtitle_something.md", "blogDomainName/2018/02/10/title_subtitle_something.html")]
        [TestCase("blogDomainName", "2018-02-10-title-subtitle-something.md", "blogDomainName/2018/02/10/title-subtitle-something.html")]
        public void GetBody_Execute_ShowResult(string blogDomainName, string fileName, string linkResult)
        {
            // Arrange
            var expectedResult = $"{Resource.Check} - {linkResult}";
            var handler = this.GetHandlerSetEmail();

            // Act
            var result = handler.GetBody(blogDomainName, fileName, CommentAnswerStatus.Approved);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private SendEmailHandler GetHandlerSetEmail()
        {
            this.configurationManager = Substitute.For<IConfigurationManager>();

            return new SendEmailHandler(this.configurationManager);
        }
    }
}
