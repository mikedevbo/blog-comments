namespace Common
{
    using System;
    using System.Linq;
    using System.Security;
    using Microsoft.Extensions.Configuration;

    public class ConfigurationProvider : Messages.IConfigurationProvider
    {
        private readonly IConfiguration configuration;

        public ConfigurationProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string UserAgent
        {
            get
            {
                return this.configuration["UserAgent"];
            }
        }

        public string AuthorizationToken
        {
            get
            {
                return this.configuration["AuthorizationToken"];
            }
        }

        public string RepositoryName
        {
            get
            {
                return this.configuration["RepositoryName"];
            }
        }

        public string MasterBranchName
        {
            get
            {
                return this.configuration["MasterBranchName"];
            }
        }

        public int CommentResponseAddedSagaTimeoutInSeconds
        {
            get
            {
                return Convert.ToInt32(this.configuration["CommentResponseAddedSagaTimeoutInSeconds"]);
            }
        }

        public string NsbEndpointName
        {
            get
            {
                return this.configuration["NsbEndpointName"];
            }
        }

        public string NsbTransportConnectionString
        {
            get
            {
                return this.configuration.GetConnectionString("NsbTransportConnectionString");
            }
        }

        public string NsbErrorQueueName
        {
            get
            {
                return this.configuration["NsbErrorQueueName"];
            }
        }

        public string NsbAuditQueueName
        {
            get
            {
                return this.configuration["NsbAuditQueueName"];
            }
        }

        public bool NsbIsDisableRecoverability
        {
            get
            {
                return Convert.ToBoolean(this.configuration["NsbIsDisableRecoverability"]);
            }
        }

        public bool NsbIsSendHeartbeats
        {
            get
            {
                return Convert.ToBoolean(this.configuration["NsbIsSendHeartbeats"]);
            }
        }

        public bool NsbIsSendMetrics
        {
            get
            {
                return Convert.ToBoolean(this.configuration["NsbIsSendMetrics"]);
            }
        }

        public string NsbServiceControlQueueName
        {
            get
            {
                return this.configuration["NsbServiceControlQueueName"];
            }
        }

        public string NsbServiceControlMetricsQueueName
        {
            get
            {
                return this.configuration["NsbServiceControlMetricsQueueName"];
            }
        }

        public bool IsUseFakes
        {
            get
            {
                return Convert.ToBoolean(this.configuration["IsUseFakes"]);
            }
        }

        public bool IsSendEmail
        {
            get
            {
                return Convert.ToBoolean(this.configuration["IsSendEmail"]);
            }
        }

        public string SmtpHost
        {
            get
            {
                return this.configuration["SmtpHost"];
            }
        }

        public int SmtpPort
        {
            get
            {
                return Convert.ToInt32(this.configuration["SmtpPort"]);
            }
        }

        public string SmtpHostUserName
        {
            get
            {
                return this.configuration["SmtpHostUserName"];
            }
        }

        public SecureString SmtpHostPassword
        {
            get
            {
                var pass = new SecureString();
                this.configuration["SmtpHostPassword"]
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
                return this.configuration["SmtpFrom"];
            }
        }

        public string BlogDomainName
        {
            get
            {
                return this.configuration["BlogDomainName"];
            }
        }
    }
}