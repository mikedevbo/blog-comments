namespace Messages.Messages
{
    public class RequestCreateBranch
    {
        public string UserName { get; set; }

        public string BranchName { get; set; }

        public string FileName { get; set; }

        public string Content { get; set; }
    }
}
