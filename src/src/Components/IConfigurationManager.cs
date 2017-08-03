using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public interface IConfigurationManager
    {
        string UserAgent { get; set; }

        string AuthorizationToken { get; set; }

        string MasterRepositoryName { get; set; }
    }
}
