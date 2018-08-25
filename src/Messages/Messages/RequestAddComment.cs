namespace Messages.Messages
{
    public class RequestAddComment
    {
        public string UserName { get; set; }

        public string BranchName { get; set; }

        public string FileName { get; set; }

        public string Content { get; set; }
    }
}
