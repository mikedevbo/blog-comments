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

        string RepositoryName { get; set; }

        string MasterBranchName { get; set; }

        string NsbEndpointName { get; }

        string NsbEndpointInstanceId { get; }

        string NsbTransportConnectionString { get; }

        string NsbErrorQueueName { get; }

        string NsbAuditQueueName { get; }
    }
}
