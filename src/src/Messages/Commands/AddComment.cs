using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Commands
{
    public class AddComment
    {
        public Guid CommentId { get; set; }

        public string BranchName { get; set; }

        public string FileName { get; set; }

        public string Comment { get; set; }
    }
}
