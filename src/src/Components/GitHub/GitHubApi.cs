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
            string masterRepositoryName)
        {
            Repository masterRepo = this.GetRepository(
                userAgent,
                authorizationToken,
                masterRepositoryName);

            string sha = masterRepo.Object.Sha;

            var sb = new StringBuilder();
            sb.Append(DateTime.UtcNow).Append(Guid.NewGuid());
            string branchName = sb.ToString();

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
