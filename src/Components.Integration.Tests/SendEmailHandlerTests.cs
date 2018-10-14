namespace Components.Integration.Tests
{
    using NUnit.Framework;

    [TestFixture]
    [Ignore("only for manual tests")]
    public class SendEmailHandlerTests
    {
        private SendEmailHandler GetHandlerSetEmail()
        {
            return new SendEmailHandler(new Common.ConfigurationManager());
        }
    }
}
