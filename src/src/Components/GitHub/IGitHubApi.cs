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

        Task<string> CreatePullRequest(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string headBranchName,
            string baseBranchName);

        Task<bool> IsPullRequestOpen(
            string userAgent,
            string authorizationToken,
            string pullRequestUrl);

        Task<bool> IsPullRequestMerged(
            string userAgent,
            string authorizationToken,
            string pullRequestUrl);
    }
}
