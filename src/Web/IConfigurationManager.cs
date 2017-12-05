namespace Web
{
    public interface IConfigurationManager
    {
        string NsbEndpointName { get; }

        string NsbTransportConnectionString { get; }

        string NsbErrorQueueName { get; }

        string NsbAuditQueueName { get; }

        bool NsbIsIntegrationTests { get; }

        DevMode DevMode { get; }
    }
}
