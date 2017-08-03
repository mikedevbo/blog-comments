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
            string repositoryName);

        void CreateRepositoryBranch(
            string userAgent,
            string authorizationToken,
            string masterRepositoryName,
            string masterRepositorySha,
            string branchName);
    }
}
