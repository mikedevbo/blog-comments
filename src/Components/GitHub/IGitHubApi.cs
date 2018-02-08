namespace Components.GitHub
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
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

        Task<(bool result, string etag)> IsPullRequestOpen(
            string userAgent,
            string authorizationToken,
            string pullRequestUrl,
            string etag);

        Task<bool> IsPullRequestMerged(
            string userAgent,
            string authorizationToken,
            string pullRequestUrl);
    }
}
