using Components.GitHub.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Components.GitHub
{
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
