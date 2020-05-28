namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Commands

type CheckCommentAnswer(commentId: string, commentUri: string) =
    member this.CommentId = commentId
    member this.CommentUri = commentUri 

namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages

type RequestCheckCommentAnswer(commentUri: string, etag: string) =
    member this.CommentUri = commentUri
    member this.ETag = etag

type AnswerStatus = NotAdded = 1  | Approved = 2 | Rejected = 3

type ResponseCheckCommentAnswer(answerStatus: AnswerStatus, etag: string) =
    member this.AnswerStatus = answerStatus
    member this.ETag = etag
    
type TimeoutCheckCommentAnswer() = class end
