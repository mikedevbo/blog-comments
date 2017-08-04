using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
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

        public string NsbTransportConnectionStringName
        {
            get
            {
                return @"NsbTransportConnectionStringName";
            }
        }
    }
}