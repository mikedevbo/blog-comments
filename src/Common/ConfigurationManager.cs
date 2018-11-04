namespace Common
{
    using System;
    using System.Linq;
    using System.Security;

    public class ConfigurationManager : IConfigurationManager
    {
        public string UserAgent
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["UserAgent"];
            }
        }

        public string AuthorizationToken
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["AuthorizationToken"];
            }
        }

        public string RepositoryName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RepositoryName"];
            }
        }

        public string MasterBranchName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MasterBranchName"];
            }
        }

        public int CommentResponseAddedSagaTimeoutInSeconds
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CommentResponseAddedSagaTimeoutInSeconds"]);
            }
        }

        public string NsbEndpointName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbEndpointName"];
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

        public bool NsbIsDisableRecoverability
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings[@"NsbIsDisableRecoverability"]);
            }
        }

        public bool NsbIsSendHeartbeats
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings[@"NsbIsSendHeartbeats"]);
            }
        }

        public bool NsbIsSendMetrics
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings[@"NsbIsSendMetrics"]);
            }
        }

        public string NsbServiceControlQueueName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbServiceControlQueueName"];
            }
        }

        public string NsbServiceControlMetricsQueueName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[@"NsbServiceControlMetricsQueueName"];
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

                switch (devMode)
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

        public bool IsSendEmail
        {
            get
            {
                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsSendEmail"]);
            }
        }

        public string SmtpHost
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SmtpHost"];
            }
        }

        public int SmtpPort
        {
            get
            {
                return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SmtpPort"]);
            }
        }

        public string SmtpHostUserName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SmtpHostUserName"];
            }
        }

        public SecureString SmtpHostPassword
        {
            get
            {
                var pass = new SecureString();
                System.Configuration.ConfigurationManager.AppSettings["SmtpHostPassword"]
                    .ToCharArray()
                    .ToList()
                    .ForEach(c => pass.AppendChar(c));

                return pass;
            }
        }

        public string SmtpFrom
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SmtpFrom"];
            }
        }

        public string BlogDomainName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["BlogDomainName"];
            }
        }
    }
}