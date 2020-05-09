using System.Threading.Tasks;

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public interface ICommentAnswerPolicyLogic
    {
        Task<CheckCommentAnswerMsgResponseMsg> CheckAnswer(string commentUri, string etag);
    }
}