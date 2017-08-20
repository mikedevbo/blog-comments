namespace Components.Integration.Tests
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Components.GitHub;
    using NUnit.Framework;

    [TestFixture]
    [Ignore("only for manual tests")]
    public class GitHubApiTests
    {
        private readonly IComponentsConfigurationManager configurationComponentsManager =
            new ComponentsConfigurationManager();

        [Test]
        public async Task GetSha_Execute_ProperResult()
        {
            // Arrange
            var api = this.GetGitHubApi();

            // Act
            var result = await api.GetSha(
                this.configurationComponentsManager.UserAgent,
                this.configurationComponentsManager.AuthorizationToken,
                this.configurationComponentsManager.RepositoryName,
                this.configurationComponentsManager.MasterBranchName).ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);

            Console.WriteLine(string.Format("{0}", result));
        }

        [Test]
        public async Task CreateRepositoryBranch_Execute_ProperResult()
        {
            // Arrange
            const string newBranchName = "c-15";
            var api = this.GetGitHubApi();

            // Act
            await api.CreateRepositoryBranch(
                this.configurationComponentsManager.UserAgent,
                this.configurationComponentsManager.AuthorizationToken,
                this.configurationComponentsManager.RepositoryName,
                this.configurationComponentsManager.MasterBranchName,
                newBranchName).ConfigureAwait(false);

            // Assert
            var result = await api.GetSha(
                this.configurationComponentsManager.UserAgent,
                this.configurationComponentsManager.AuthorizationToken,
                this.configurationComponentsManager.RepositoryName,
                newBranchName).ConfigureAwait(false);

            Assert.NotNull(result);

            Console.WriteLine(string.Format("{0}", result));
        }

        [Test]
        public Task UpdateFile_Execute_ProperResult()
        {
            // Arrange
            const string branchName = "c-14";
            var api = this.GetGitHubApi();

            // Act
            return api.UpdateFile(
                this.configurationComponentsManager.UserAgent,
                this.configurationComponentsManager.AuthorizationToken,
                this.configurationComponentsManager.RepositoryName,
                branchName,
                "test.txt",
                "\nnew comment - " + DateTime.Now);

            // Assert
            ////TODO: to implement
        }

        [Test]
        public async Task CreatePullRequest_Execute_ProperResult()
        {
            // Arrange
            const string branchName = "c-12";
            var api = this.GetGitHubApi();

            // Act
            var result = await api.CreatePullRequest(
                this.configurationComponentsManager.UserAgent,
                this.configurationComponentsManager.AuthorizationToken,
                this.configurationComponentsManager.RepositoryName,
                branchName,
                "master").ConfigureAwait(false);

            // Assert
            Assert.NotNull(result);
            Console.WriteLine(result);
        }

        [Test]
        public async Task IsPullRequestOpen_Execute_ProperResult()
        {
            // Arrange
            string pullRequestUri = ConfigurationManager.AppSettings["pullRequestUri"];
            var api = this.GetGitHubApi();

            // Act
            var result = await api.IsPullRequestOpen(
                this.configurationComponentsManager.UserAgent,
                this.configurationComponentsManager.AuthorizationToken,
                pullRequestUri).ConfigureAwait(false);

            // Assert
            Console.WriteLine(result);
        }

        [Test]
        public async Task IsPullRequestMerged_Execute_ProperResult()
        {
            // Arrange
            string pullRequestUri = ConfigurationManager.AppSettings["pullRequestUri"];
            var api = this.GetGitHubApi();

            // Act
            var result = await api.IsPullRequestMerged(
                this.configurationComponentsManager.UserAgent,
                this.configurationComponentsManager.AuthorizationToken,
                pullRequestUri).ConfigureAwait(false);

            // Assert
            Console.WriteLine(result);
        }

        private GitHubApi GetGitHubApi()
        {
            return new GitHubApi();
        }
    }
}
