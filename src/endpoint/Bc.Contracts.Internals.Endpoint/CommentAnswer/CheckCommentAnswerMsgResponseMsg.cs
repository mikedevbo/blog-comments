namespace Bc.Contracts.Internals.Endpoint.CommentAnswer
{
    public class CheckCommentAnswerMsgResponseMsg
    {
        public CheckCommentAnswerMsgResponseMsg(CommentAnswerStatus answerStatus)
        {
            this.AnswerStatus = answerStatus;
        }

        public CommentAnswerStatus AnswerStatus { get; }
    }
}