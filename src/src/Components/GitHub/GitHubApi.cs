namespace Components.GitHub
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Components.GitHub.Dto;

    public class GitHubApi : IGitHubApi
    {
        private const string ApiBaseUri = @"https://api.github.com/";

        public async Task<RepositoryResponse> GetRepository(
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

            var repo = await response.Content.ReadtAsJsonAsync<RepositoryResponse>().ConfigureAwait(false);
            return repo;
        }

        public async Task<bool> IsPullRequestExists(
            string userAgent,
            string authorizationToken,
            string pullRequestUrl)
        {
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUri) };
            this.SetRequestHeaders(httpClient.DefaultRequestHeaders, userAgent, authorizationToken);

            HttpResponseMessage response = await httpClient.GetAsync(pullRequestUrl).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsPullRequestMerged(
            string userAgent,
            string authorizationToken,
            string pullRequestUrl)
        {
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUri) };
            this.SetRequestHeaders(httpClient.DefaultRequestHeaders, userAgent, authorizationToken);

            var requestUri = string.Format(@"{0}/merge", pullRequestUrl);
            HttpResponseMessage response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            var exception = new HttpRequestException(string.Format(@"Response bad satus code: {0}", response.StatusCode));
            exception.Data.Add("response", response);
            throw exception;
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

            var requestUri = string.Format(@"repos/{0}/{1}/git/refs", userAgent, repositoryName);

            var masterRepo = await this.GetRepository(
                userAgent,
                authorizationToken,
                repositoryName,
                masterBranchName).ConfigureAwait(false);

            var branchRequest = new BranchRequest
            {
                Ref = string.Format(@"refs/heads/{0}", newBranchName),
                Sha = masterRepo.Object.Sha
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(requestUri, branchRequest).ConfigureAwait(false);
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
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUri) };
            this.SetRequestHeaders(httpClient.DefaultRequestHeaders, userAgent, authorizationToken);

            // get file to update
            var requestUri = string.Format(@"repos/{0}/{1}/contents/{2}?ref={3}", userAgent, repositoryName, fileName, branchName);
            HttpResponseMessage response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var fileToUpdate = await response.Content.ReadtAsJsonAsync<FileContentResponse>().ConfigureAwait(false);
            byte[] data = Convert.FromBase64String(fileToUpdate.Content);
            string decodedData = Encoding.UTF8.GetString(data);

            // update file content
            decodedData += content;

            // update file
            requestUri = string.Format(@"repos/{0}/{1}/contents/{2}", userAgent, repositoryName, fileName);
            var newFile = new FileContentRequest
            {
                Message = "add comment",
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(decodedData)),
                Sha = fileToUpdate.Sha,
                Branch = branchName
            };

            response = await httpClient.PutAsJsonAsync(requestUri, newFile).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task<PullRequestResponse> CreatePullRequest(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string headBranchName,
            string baseBranchName)
        {
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri(ApiBaseUri) };
            this.SetRequestHeaders(httpClient.DefaultRequestHeaders, userAgent, authorizationToken);

            var requestUri = string.Format(@"repos/{0}/{1}/pulls", userAgent, repositoryName);

            var pullRequest = new PullRequestRequest
            {
                Title = headBranchName,
                Body = "comment to merge",
                Head = headBranchName,
                Base = baseBranchName
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(requestUri, pullRequest).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return new PullRequestResponse { Location = response.Headers.Location.AbsoluteUri };
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
