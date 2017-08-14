namespace Web.Models
{
    using System;

    public class Comment
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserWebsite { get; set; }

        public string Content { get; set; }
    }
}