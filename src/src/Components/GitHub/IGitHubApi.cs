namespace Components.GitHub
{
    using Components.GitHub.Dto;

    /// <summary>
    /// The github api interface.
    /// </summary>
    public interface IGitHubApi
    {
        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="branchName">Name of the branch.</param>
        /// <returns>The repository.</returns>
        Repository GetRepository(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName);

        /// <summary>
        /// Creates the repository branch.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="masterBranchName">Name of the master branch.</param>
        /// <param name="newBranchName">New name of the branch.</param>
        void CreateRepositoryBranch(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string masterBranchName,
            string newBranchName);

        /// <summary>
        /// Updates the file.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="branchName">Name of the branch.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">The content.</param>
        void UpdateFile(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName,
            string fileName,
            string content);

        /// <summary>
        /// Creates the pull request.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <param name="headBranchName">Name of the head branch.</param>
        /// <param name="baseBranchName">Name of the base branch.</param>
        void CreatePullRequest(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string headBranchName,
            string baseBranchName);
    }
}
