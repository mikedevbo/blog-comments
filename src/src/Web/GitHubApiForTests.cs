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

            return Task.Run(() => @"1234");
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

            return Task.Run(() => @"https://test/test");
        }

        public Task<bool> IsPullRequestOpen(string userAgent, string authorizationToken, string pullRequestUrl)
        {
            log.Info("IsPullRequestOpen");

            return Task.Run(() => false);
        }

        public Task<bool> IsPullRequestMerged(string userAgent, string authorizationToken, string pullRequestUrl)
        {
            log.Info("IsPullRequestMerged");

            return Task.Run(() => true);
        }
    }
}
