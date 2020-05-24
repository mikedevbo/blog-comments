namespace Bc.Contracts.Internals.Endpoint.ITOps.CreateGitHubPullRequest
{
    public interface IConfigurationProvider
    {
        public string UserAgent { get; }

        public string AuthorizationToken { get; }

        public string RepositoryName { get; }

        public string MasterBranchName { get; }
    }
}