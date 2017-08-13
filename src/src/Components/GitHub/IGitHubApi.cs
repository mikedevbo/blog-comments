namespace Components.GitHub
{
    using Components.GitHub.Dto;

    public interface IGitHubApi
    {
        Repository GetRepository(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName);

        void CreateRepositoryBranch(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string masterBranchName,
            string newBranchName);

        void UpdateFile(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName,
            string fileName,
            string content);

        void CreatePullRequest(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string headBranchName,
            string baseBranchName);
    }
}
