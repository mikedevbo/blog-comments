namespace Components.GitHub
{
    using System;
    using System.Net.Http;
    using Components.GitHub.Dto;

    /// <summary>
    /// The github api.
    /// </summary>
    /// <seealso cref="Components.GitHub.IGitHubApi" />
    public class GitHubApi : IGitHubApi
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubApi"/> class.
        /// </summary>
        public GitHubApi()
        {
            this.httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.github.com/")
            };
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
        public Repository GetRepository(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName)
        {
            ////TODO: to implement
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Add("User-agent", userAgent);
            this.httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Token {0}", authorizationToken));
            return new Repository();
        }

        /// <summary>
        /// Creates the repository branch.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="masterBranchName">Name of the master branch.</param>
        /// <param name="newBranchName">New name of the branch.</param>
        public void CreateRepositoryBranch(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string masterBranchName,
            string newBranchName)
        {
            Repository masterRepo = this.GetRepository(
                userAgent,
                authorizationToken,
                repositoryName,
                masterBranchName);

            string sha = masterRepo.Object.Sha;

            ////TODO: to implement
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
        public void UpdateFile(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName,
            string fileName,
            string content)
        {
            ////TODO: to implement
        }

        /// <summary>
        /// Creates the pull request.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="headBranchName">Name of the head branch.</param>
        /// <param name="baseBranchName">Name of the base branch.</param>
        public void CreatePullRequest(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string headBranchName,
            string baseBranchName)
        {
            ////TODO: to implement
        }
    }
}
