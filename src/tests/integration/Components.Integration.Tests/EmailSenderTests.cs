namespace Components.Integration.Tests
{
    using System.Configuration;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    [Ignore("only for manual tests")]
    public class EmailSenderTests
    {
        [Test]
        public void Send_ForSampleData_NoException()
        {
            // Arrange
            var userName = ConfigurationManager.AppSettings["userName"];
            var userEmail = ConfigurationManager.AppSettings["userEmail"];
            var sender = this.GetEmailSender();

            // Act
            sender.Send(userName, userEmail, CommentResponseStatus.Approved);

            // Assert
        }

        private EmailSender GetEmailSender()
        {
            return new EmailSender(new ComponentsConfigurationManager());
        }
    }
}
