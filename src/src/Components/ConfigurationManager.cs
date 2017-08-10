using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Components
{
    public class ConfigurationManager : IConfigurationManager
    {
        public string UserAgent { get; set; }

        public string AuthorizationToken { get; set; }

        public string RepositoryName { get; set; }

        public string MasterBranchName { get; set; }

        public string NsbEndpointName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbEndpointName"];
            }
        }

        public string NsbEndpointInstanceId
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbEndpointInstanceId"];
            }
        }

        public string NsbTransportConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings[@"NsbTransportConnectionString"].ConnectionString;
            }
        }

        public string NsbErrorQueueName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbErrorQueueName"];
            }
        }

        public string NsbAuditQueueName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbAuditQueueName"];
            }
        }

        public bool NsbIsIntegrationTests
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NsbIsIntegrationTests"]);
            }
        }

        public int CommentResponseAddedSagaTimeoutInSeconds
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommentResponseAddedSagaTimeoutInSeconds"]);
            }
        }
    }
}