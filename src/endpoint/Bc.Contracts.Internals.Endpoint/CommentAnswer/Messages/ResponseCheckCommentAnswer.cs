namespace Bc.Contracts.Internals.Endpoint.CommentAnswer.Messages
{
    public class ResponseCheckCommentAnswer
    {
        public ResponseCheckCommentAnswer(CommentAnswerStatus answerStatus, string etag)
        {
            this.AnswerStatus = answerStatus;
            this.ETag = etag;
        }

        public CommentAnswerStatus AnswerStatus { get; }

        public string ETag { get;  }
    }
}