namespace Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Messages.Commands;
    using NServiceBus;
    using Web.Models;

    public class CommentController : Controller
    {
        private IEndpointInstance endpoint;

        public CommentController(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
        }

        [HttpPost]
        public Task RequestForComment(Comment comment)
        {
            return this.endpoint.Send<StartAddingComment>(command =>
             {
                 command.CommentId = Guid.NewGuid();
                 command.UserName = comment.UserName;
                 command.UserEmail = comment.UserEmail;
                 command.UserWebsite = comment.UserWebsite;
                 command.FileName = comment.FileName;
                 command.Content = comment.Content;
             });
        }
    }
}