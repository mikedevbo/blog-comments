using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components.GitHub.Dto;
using NServiceBus.Logging;
using Simple.Data;
using Components.GitHub;

namespace Web
{
    public class GitHubApiForTests : IGitHubApi
    {
        private static ILog log = LogManager.GetLogger<GitHubApiForTests>();
        private readonly IConfigurationManager configurationManacger;

        public GitHubApiForTests(IConfigurationManager configurationManacger)
        {
            this.configurationManacger = configurationManacger;
        }

        public void CreatePullRequest(string userAgent, string authorizationToken, string repositoryName, string headBranchName, string baseBranchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 1);

            log.Info("CreatePullRequest");
        }

        public void CreateRepositoryBranch(string userAgent, string authorizationToken, string repositoryName, string masterBranchName, string newBranchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 2);

            log.Info("CreateRepositoryBranch");
        }

        public Repository GetRepository(string userAgent, string authorizationToken, string repositoryName, string branchName)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 3);

            log.Info("GetRepository");
            return new Repository();
        }

        public void UpdateFile(string userAgent, string authorizationToken, string repositoryName, string branchName, string fileName, string content)
        {
            Database.OpenConnection(this.configurationManacger.NsbTransportConnectionString)
                    .SagaTestResults
                    .Insert(Result: 4);

            log.Info("UpdateFile");
        }
    }
}
