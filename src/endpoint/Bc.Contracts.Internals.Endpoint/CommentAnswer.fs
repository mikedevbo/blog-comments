namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
    type AnswerStatus = NotAdded = 1  | Approved = 2 | Rejected = 3

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands

    open System

    type CheckCommentAnswer(commentId: Guid, commentUri: string) =
        member this.CommentId = commentId
        member this.CommentUri = commentUri 

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages
    type TimeoutCheckCommentAnswer() = class end
    
namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Logic
    type ICommentAnswerPolicyLogic =
        abstract member CheckCommentAnswerTimeoutInSeconds: int