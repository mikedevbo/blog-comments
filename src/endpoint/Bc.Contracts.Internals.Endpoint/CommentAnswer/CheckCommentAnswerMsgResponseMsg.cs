namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public class CheckCommentAnswerMsgResponseMsg
    {
        public CheckCommentAnswerMsgResponseMsg(CommentAnswerStatus answerStatus, string etag)
        {
            this.AnswerStatus = answerStatus;
            this.ETag = etag;
        }

        public CommentAnswerStatus AnswerStatus { get; }

        public string ETag { get;  }
    }
}