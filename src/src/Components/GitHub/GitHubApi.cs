namespace Components.GitHub
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Components.GitHub.Dto;

    public class GitHubApi : IGitHubApi
    {
        private const string ApiBaseUri = @"https://api.github.com/";

        public async Task<Repository> GetRepository(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName)
        {
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUri) };
            this.SetRequestHeaders(httpClient.DefaultRequestHeaders, userAgent, authorizationToken);

            var requestUri = string.Format(@"repos/{0}/{1}/git/refs/heads/{2}", userAgent, repositoryName, branchName);
            HttpResponseMessage response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var repo = await response.Content.ReadtAsJsonAsync<Repository>();
            return repo;
        }

        public async Task CreateRepositoryBranch(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string masterBranchName,
            string newBranchName)
        {
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUri) };
            this.SetRequestHeaders(httpClient.DefaultRequestHeaders, userAgent, authorizationToken);

            Repository masterRepo = await this.GetRepository(
                userAgent,
                authorizationToken,
                repositoryName,
                masterBranchName);

            var requestUri = string.Format(@"repos/{0}/{1}/git/refs", userAgent, repositoryName);
            var gitHubRef = new GitHubRef
            {
                Ref = string.Format(@"refs/heads/{0}", newBranchName),
                Sha = masterRepo.Object.Sha
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(requestUri, gitHubRef).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateFile(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName,
            string fileName,
            string content)
        {
            ////TODO: to implement
            await Task.CompletedTask;
        }

        public async Task CreatePullRequest(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string headBranchName,
            string baseBranchName)
        {
            ////TODO: to implement
            await Task.CompletedTask;
        }

        private void SetRequestHeaders(HttpRequestHeaders httpRequestHeaders, string userAgent, string authorizationToken)
        {
            httpRequestHeaders.Accept.Clear();
            httpRequestHeaders.Add("User-agent", userAgent);
            httpRequestHeaders.Add("Authorization", string.Format("Token {0}", authorizationToken));
            httpRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
