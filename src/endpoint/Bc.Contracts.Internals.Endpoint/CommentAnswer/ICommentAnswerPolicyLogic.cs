using System.Threading.Tasks;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public interface ICommentAnswerPolicyLogic
    {
        Task<CommentAnswerStatus> CheckAnswer(string commentUri, string etag);
    }
}