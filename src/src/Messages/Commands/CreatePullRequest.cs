using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Commands
{
    public class CreatePullRequest
    {
        public Guid CommentId { get; set; }

        public string HeadBranchName { get; set; }

        public string BaseBranchName { get; set; }
    }
}
