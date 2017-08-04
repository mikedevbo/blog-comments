using Components.GitHub.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.GitHub
{
    public class GitHubApi : IGitHubApi
    {
        public void CreateRepositoryBranch(
            string userAgent, 
            string authorizationToken, 
            string masterRepositoryName,
            string masterBranchName,
            string newBranchName)
        {
            Repository masterRepo = this.GetRepository(
                userAgent,
                authorizationToken,
                masterRepositoryName);

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

        public Repository GetRepository(
            string userAgent,
            string authorizationToken,
            string repositoryName)
        {
            ////TODO: to implement
            return new Repository();
        }
    }
}
