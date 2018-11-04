namespace Common
{
    using System.Security;

    public interface IConfigurationManager
    {
        string UserAgent { get; }

        string AuthorizationToken { get; }

        string RepositoryName { get; }

        string MasterBranchName { get; }

        int CommentResponseAddedSagaTimeoutInSeconds { get; }

        string NsbEndpointName { get; }

        string NsbTransportConnectionString { get; }

        string NsbErrorQueueName { get; }

        string NsbAuditQueueName { get; }

        bool NsbIsDisableRecoverability { get; }

        bool NsbIsSendHeartbeats { get; }

        bool NsbIsSendMetrics { get; }

        string NsbServiceControlQueueName { get; }

        string NsbServiceControlMetricsQueueName { get; }

        bool NsbIsIntegrationTests { get; }

        DevMode DevMode { get; }

        bool IsSendEmail { get; }

        string SmtpHost { get; }

        int SmtpPort { get; }

        string SmtpHostUserName { get; }

        SecureString SmtpHostPassword { get; }

        string SmtpFrom { get; }

        string BlogDomainName { get; }
    }
}
