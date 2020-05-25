using System;
using System.Threading.Tasks;

namespace Bc.Contracts.Internals.Endpoint.CommentRegistration
{
    public interface IPolicyLogic
    {
        Task<string> CreateBranch(DateTime creationDate);

        Task AddComment(string branchName, CommentData commentData);

        Task<string> CreatePullRequest(string branchName);
    }
}