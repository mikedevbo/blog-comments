using System;
using System.Threading.Tasks;

namespace Bc.Contracts.Internals.Endpoint.ITOps.GitHubPullRequest
{
    public interface IPolicyLogic
    {
        Task<string> CreateBranch(DateTime creationDate);

        Task UpdateFile(string branchName, string fileName, string content);

        Task<string> CreatePullRequest(string branchName);
    }
}