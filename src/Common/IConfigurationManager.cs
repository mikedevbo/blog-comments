namespace Common
{
    using System.Security;

    public interface IConfigurationManager
    {
        string NsbEndpointName { get; }

        string NsbTransportConnectionString { get; }

        string NsbErrorQueueName { get; }

        string NsbAuditQueueName { get; }

        bool NsbIsIntegrationTests { get; }

        DevMode DevMode { get; }

        string SmtpHost { get; }

        int SmtpPort { get; }

        string SmtpHostUserName { get; }

        SecureString SmtpHostPassword { get; }

        string SmtpFrom { get; }
    }
}
