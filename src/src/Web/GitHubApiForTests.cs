namespace Web
{
    using Components.GitHub;
    using Components.GitHub.Dto;
    using NServiceBus.Logging;
    using Simple.Data;

    /// <summary>
    /// The github api implementation for tests.
    /// </summary>
    /// <seealso cref="Components.GitHub.IGitHubApi" />
    public class GitHubApiForTests : IGitHubApi
    {
        private static ILog log = LogManager.GetLogger<GitHubApiForTests>();
        private readonly IConfigurationManager configurationManacger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubApiForTests"/> class.
        /// </summary>
        /// <param name="configurationManacger">The configuration manacger.</param>
        public GitHubApiForTests(IConfigurationManager configurationManacger)
        {
            this.configurationManacger = configurationManacger;
        }

        /// <summary>
        /// Creates the pull request.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="headBranchName">Name of the head branch.</param>
        /// <param name="baseBranchName">Name of the base branch.</param>
        public void CreatePullRequest(string userAgent, string authorizationToken, string repositoryName, string headBranchName, string baseBranchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 1);

            log.Info("CreatePullRequest");
        }

        /// <summary>
        /// Creates the repository branch.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="masterBranchName">Name of the master branch.</param>
        /// <param name="newBranchName">New name of the branch.</param>
        public void CreateRepositoryBranch(string userAgent, string authorizationToken, string repositoryName, string masterBranchName, string newBranchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 2);

            log.Info("CreateRepositoryBranch");
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="branchName">Name of the branch.</param>
        /// <returns>
        /// The repository.
        /// </returns>
        public Repository GetRepository(string userAgent, string authorizationToken, string repositoryName, string branchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 3);

            log.Info("GetRepository");
            return new Repository();
        }

        /// <summary>
        /// Updates the file.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="branchName">Name of the branch.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">The content.</param>
        public void UpdateFile(string userAgent, string authorizationToken, string repositoryName, string branchName, string fileName, string content)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 4);

            log.Info("UpdateFile");
        }
    }
}
