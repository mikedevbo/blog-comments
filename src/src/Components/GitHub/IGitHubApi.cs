using Components.GitHub.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.GitHub
{
    public interface IGitHubApi
    {
        Repository GetRepository(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName);

        void CreateRepositoryBranch(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string masterBranchName,
            string newBranchName);

        void UpdateFile(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string branchName,
            string fileName,
            string content);

        void CreatePullRequest(
            string userAgent,
            string authorizationToken,
            string repositoryName,
            string headBranchName,
            string baseBranchName);
    }
}
