using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    public class ConfigurationManager : IConfigurationManager
    {
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

        public DevMode DevMode
        {
            get
            {
                var devMode = System.Configuration.ConfigurationManager.AppSettings["DevMode"];
                switch(devMode)
                {
                    case "dev":
                        return DevMode.Dev;

                    case "test":
                        return DevMode.Test;

                    case "production":
                        return DevMode.Production;

                    default:
                        throw new ArgumentException("dev mode not implemented: {0}", devMode);
                }
            }
        }
    }
}