namespace Components.Integration.Tests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Common;
    using Components.GitHub;
    using Microsoft.Extensions.Configuration;
    using NUnit.Framework;

    [TestFixture]
    //[Ignore("only for manual tests")]
    public class GitHubApiTests
    {
        private readonly IConfiguration config;
        private readonly IConfigurationManager configurationManager;

        public GitHubApiTests()
        {
            this.config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.components.integration.tests.json", false, true)
                            .Build();

            this.configurationManager = new ConfigurationManager(this.config);
        }

        [Test]
        public async Task GetSha_Execute_ProperResult()
        {
            // Arrange
            var api = this.GetGitHubApi();

            // Act
            var result = await api.GetSha(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                this.configurationManager.MasterBranchName).ConfigureAwait(false);

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
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                this.configurationManager.MasterBranchName,
                newBranchName).ConfigureAwait(false);

            // Assert
            var result = await api.GetSha(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                newBranchName).ConfigureAwait(false);

            Assert.NotNull(result);

            Console.WriteLine(string.Format("{0}", result));
        }

        [Test]
        public Task UpdateFile_Execute_ProperResult()
        {
            // Arrange
            const string branchName = "c-15";
            var api = this.GetGitHubApi();

            // Act
            return api.UpdateFile(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
                branchName,
                "_posts/test.md",
                "\nnew comment - " + DateTime.Now);

            // Assert
        }

        [Test]
        public async Task CreatePullRequest_Execute_ProperResult()
        {
            // Arrange
            const string branchName = "c-15";
            var api = this.GetGitHubApi();

            // Act
            var result = await api.CreatePullRequest(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                this.configurationManager.RepositoryName,
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
            string pullRequestUri = this.config["pullRequestUri"];
            var api = this.GetGitHubApi();

            // Act
            var result = await api.IsPullRequestOpen(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
                pullRequestUri,
                "\"96ac3062f47cab793ff0aea264498eb4\"").ConfigureAwait(false);

            // Assert
            Console.WriteLine(result);
        }

        [Test]
        public async Task IsPullRequestMerged_Execute_ProperResult()
        {
            // Arrange
            string pullRequestUri = this.config["pullRequestUri"];
            var api = this.GetGitHubApi();

            // Act
            var result = await api.IsPullRequestMerged(
                this.configurationManager.UserAgent,
                this.configurationManager.AuthorizationToken,
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
