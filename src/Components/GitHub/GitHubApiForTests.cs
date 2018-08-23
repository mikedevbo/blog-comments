namespace Components.GitHub
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Common;
    using NServiceBus.Logging;
    using Simple.Data;

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    public class GitHubApiForTests : IGitHubApi
    {
        private static ILog log = LogManager.GetLogger<GitHubApiForTests>();
        private readonly IConfigurationManager configurationManager;

        public GitHubApiForTests(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public Task<string> GetSha(string userAgent, string authorizationToken, string repositoryName, string branchName)
        {
            log.Info("GetRepository");

            return Task.Run(() => @"1234");
        }

        public Task CreateRepositoryBranch(string userAgent, string authorizationToken, string repositoryName, string masterBranchName, string newBranchName)
        {
            log.Info("CreateRepositoryBranch");

            return Task.CompletedTask;
        }

        public Task UpdateFile(string userAgent, string authorizationToken, string repositoryName, string branchName, string fileName, string content)
        {
            log.Info("UpdateFile");

            return Task.CompletedTask;
        }

        public Task<string> CreatePullRequest(string userAgent, string authorizationToken, string repositoryName, string headBranchName, string baseBranchName)
        {
            log.Info("CreatePullRequest");

            return Task.Run(() => @"https://test/test");
        }

        public Task<(bool result, string etag)> IsPullRequestOpen(string userAgent, string authorizationToken, string pullRequestUrl, string etag)
        {
            log.Info("IsPullRequestOpen");

            return Task.Run(() => (false, "1234"));
        }

        public Task<bool> IsPullRequestMerged(string userAgent, string authorizationToken, string pullRequestUrl)
        {
            log.Info("IsPullRequestMerged");

            return Task.Run(() => true);
        }
    }
}
