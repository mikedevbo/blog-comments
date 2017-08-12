namespace Components.Integration.Tests
{
    using Components.GitHub;
    using NUnit.Framework;

    [TestFixture]
    public class GitHubApiTests
    {
        ////private readonly IConfigurationManager configurationManager =

        [Test]
        public void T()
        {
            // Arrange
            var api = this.GetGitHubApi();

            // Act
            ////api.GetRepository()

            // Assert
        }

        private GitHubApi GetGitHubApi()
        {
            return new GitHubApi();
        }
    }
}
