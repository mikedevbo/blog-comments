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
        public async Task RequestForComment(Comment comment)
        {
            var sendOptions = new SendOptions();

            await this.endpoint.Send<StartAddingComment>(cmd => cmd.CommentId = comment.Id)
                .ConfigureAwait(false);
        }
    }
}