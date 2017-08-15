namespace Components
{
    using System;
    using NServiceBus;

    public class CommentSagaData : ContainSagaData
    {
        public Guid CommentId { get; set; }

        public string FileName { get; set; }

        public string Content { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserWebsite { get; set; }

        public string BranchName { get; set; }
    }
}
