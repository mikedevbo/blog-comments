using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components.GitHub.Dto;
using NServiceBus.Logging;

namespace Components.GitHub
{
    public class GitHubApiForTests : IGitHubApi
    {
        private ILog log = LogManager.GetLogger<GitHubApiForTests>();

        public void CreatePullRequest(string userAgent, string authorizationToken, string repositoryName, string headBranchName, string baseBranchName)
        {
            log.Info("CreatePullRequest");
        }

        public void CreateRepositoryBranch(string userAgent, string authorizationToken, string repositoryName, string masterBranchName, string newBranchName)
        {
            log.Info("CreateRepositoryBranch");
        }

        public Repository GetRepository(string userAgent, string authorizationToken, string repositoryName, string branchName)
        {
            log.Info("GetRepository");
            return new Repository();
        }

        public void UpdateFile(string userAgent, string authorizationToken, string repositoryName, string branchName, string fileName, string content)
        {
            log.Info("UpdateFile");
        }
    }
}
