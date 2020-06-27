namespace Messages.Messages
{
    using System;

    public class RequestAddComment
    {
        public string UserName { get; set; }

        public string UserWebSite { get; set; }

        public string BranchName { get; set; }

        public string FileName { get; set; }

        public string Content { get; set; }

        public DateTime AddedDate { get; set; }
    }
}
