namespace Web
{
    using System;
    using System.Threading.Tasks;
    using Components.GitHub;
    using Components.GitHub.Dto;
    using NServiceBus.Logging;
    using Simple.Data;

    public class GitHubApiForTests : IGitHubApi
    {
        private static ILog log = LogManager.GetLogger<GitHubApiForTests>();
        private readonly IConfigurationManager configurationManacger;

        public GitHubApiForTests(IConfigurationManager configurationManacger)
        {
            this.configurationManacger = configurationManacger;
        }

        public Task<string> GetSha(string userAgent, string authorizationToken, string repositoryName, string branchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 3);

            log.Info("GetRepository");

            return new Task<string>(() => @"1234");
        }

        public Task CreateRepositoryBranch(string userAgent, string authorizationToken, string repositoryName, string masterBranchName, string newBranchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 2);

            log.Info("CreateRepositoryBranch");

            return Task.CompletedTask;
        }

        public Task UpdateFile(string userAgent, string authorizationToken, string repositoryName, string branchName, string fileName, string content)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 4);

            log.Info("UpdateFile");

            return Task.CompletedTask;
        }

        public Task<string> CreatePullRequest(string userAgent, string authorizationToken, string repositoryName, string headBranchName, string baseBranchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 1);

            log.Info("CreatePullRequest");

            return new Task<string>(() => @"https://test/test");
        }

        public Task<bool> IsPullRequestOpen(string userAgent, string authorizationToken, string pullRequestUrl)
        {
            return new Task<bool>(() => false);
        }

        public Task<bool> IsPullRequestMerged(string userAgent, string authorizationToken, string pullRequestUrl)
        {
            return new Task<bool>(() => true);
        }
    }
}
