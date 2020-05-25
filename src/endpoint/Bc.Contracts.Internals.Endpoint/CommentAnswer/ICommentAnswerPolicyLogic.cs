using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public interface ICommentAnswerPolicyLogic
    {
        Task<ResponseCheckCommentAnswer> CheckAnswer(string commentUri, string etag);
    }
}