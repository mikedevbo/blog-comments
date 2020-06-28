using System;
using System.Threading.Tasks;
using Bc.Contracts.Internals.Endpoint.CommentTaking.Commands;
using Bc.Web.Models;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;

namespace Bc.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IMessageSession messageSession;

        public CommentController(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> PostTodoItem(Comment comment)
        {
            // guess what for ;)
            if (!string.IsNullOrEmpty(comment.UserPhone))
            {
                return Ok();
            }

            var command = new TakeComment(
                    Guid.NewGuid(),
                    comment.UserName,
                    comment.UserEmail,
                    comment.UserWebsite,
                    comment.UserComment,
                    comment.ArticleFileName,
                    DateTime.UtcNow
                );

            await this.messageSession.Send(command).ConfigureAwait(false);

            return Ok();
        }
    }
}