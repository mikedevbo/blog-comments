namespace Components.GitHub
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Components.GitHub.Dto;

    public class GitHubApi : IGitHubApi
    {
        private readonly HttpClient httpClient;

        public GitHubApi()
        {
            this.httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.github.com/")
            };
        }

        public async Task<Repository> GetRepository(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName)
        {
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Add("User-agent", userAgent);
            this.httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Token {0}", authorizationToken));
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestUri = string.Format(@"repos/{0}/{1}/git/refs/heads/{2}", userAgent, repositoryName, branchName);
            HttpResponseMessage response = await this.httpClient.GetAsync(requestUri).ConfigureAwait(false);
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
            Repository masterRepo = await this.GetRepository(
                userAgent,
                authorizationToken,
                repositoryName,
                masterBranchName);

            string sha = masterRepo.Object.Sha;

            ////TODO: to implement
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
    }
}
