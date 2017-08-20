namespace Components.GitHub
{
    using System.Threading.Tasks;
    using Components.GitHub.Dto;

    public interface IGitHubApi
    {
       Task<string> GetSha(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName);

        Task<bool> IsPullRequestExists(
            string userAgent,
            string authorizationToken,
            string pullRequestUrl);

        Task<bool> IsPullRequestMerged(
            string userAgent,
            string authorizationToken,
            string pullRequestUrl);

        Task CreateRepositoryBranch(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string masterBranchName,
            string newBranchName);

        Task UpdateFile(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName,
            string fileName,
            string content);

        Task<PullRequestResponse> CreatePullRequest(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string headBranchName,
            string baseBranchName);
    }
}
