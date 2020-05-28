using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification.Messages;

namespace Bc.Contracts.Internals.Endpoint.ITOps.GitHub.PullRequestStatusVerification
{
    public interface IPolicyLogic
    {
        Task<ResponseCheckPullRequestStatus> CheckPullRequestStatus(string pullRequestUri, string etag);
    }
}