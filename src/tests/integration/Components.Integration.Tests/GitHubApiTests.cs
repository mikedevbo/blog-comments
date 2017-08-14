namespace Components.Integration.Tests
{
    using Components.GitHub;
    using NUnit.Framework;

    [TestFixture]
    public class GitHubApiTests
    {
        private readonly IComponentsConfigurationManager configurationComponentsManager =
            new ComponentsConfigurationManager();

        [Test]
        [Ignore("only for manual tests")]
        public void GetRepository_Execute_ProperResult()
        {
            // Arrange
            var api = this.GetGitHubApi();

            // Act
            var result = api.GetRepository(
                this.configurationComponentsManager.UserAgent,
                this.configurationComponentsManager.AuthorizationToken,
                this.configurationComponentsManager.RepositoryName,
                "test"
                );

            // Assert
            Assert.NotNull(result.Ref);
            Assert.NotNull(result.Url);
            Assert.NotNull(result.Object.Sha);

            System.Console.WriteLine(string.Format("{0} {1} {2}", result.Ref, result.Url, result.Object.Sha));
        }

        private GitHubApi GetGitHubApi()
        {
            return new GitHubApi();
        }
    }
}
