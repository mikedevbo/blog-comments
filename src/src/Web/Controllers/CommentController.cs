using Messages.Commands;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class CommentController : Controller
    {
        IEndpointInstance endpoint;

        public CommentController(IEndpointInstance endpoint)
        {
            this.endpoint = endpoint;
        }

        [HttpPost]
        public async Task RequestForComment(Comment comment)
        {
            var sendOptions = new SendOptions();
            sendOptions.SetDestination("blogcomments");
            await this.endpoint.Send<CommentCommand>(cmd => cmd.Id = comment.Id, sendOptions)
                .ConfigureAwait(false);

            ////throw new Exception("test");
            Console.WriteLine("test");
        }
    }
}