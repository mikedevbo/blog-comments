using System;
using Microsoft.Data.SqlClient;
using NServiceBus;

namespace Bc.Common.Endpoint
{
    public static class EndpointCommon
    {
        private const string Schema = "nsb";

        public static EndpointConfiguration GetEndpoint(
            string endpointName,
            bool isSendOnlyEndpoint,
            IEndpointCommonConfigurationProvider configurationProvider)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            
            // basic configuration
            if (isSendOnlyEndpoint)
            {
                endpointConfiguration.SendOnly();
            }            
            
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            // host identifier
            endpointConfiguration.UniquelyIdentifyRunningInstance()
                .UsingNames(
                    instanceName: endpointName,
                    hostName: Environment.MachineName);

            // conventions
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => type.Name.EndsWith("Cmd") || type.IsAssignableFrom(typeof(ICommand)));
            conventions.DefiningEventsAs(type => type.Name.EndsWith("Evt") || type.IsAssignableFrom(typeof(IEvent)));
            conventions.DefiningMessagesAs(type => type.Name.EndsWith("Msg") || type.IsAssignableFrom(typeof(IMessage)));

            // transport
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(configurationProvider.TransportConnectionString);
            transport.DefaultSchema(Schema);

            // persistence
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.ConnectionBuilder(() => new SqlConnection(configurationProvider.TransportConnectionString));

            var dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>();
            dialect.Schema(Schema);
            
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.DisableCache();

            // outbox
            endpointConfiguration.EnableOutbox();

            // recoverability
            if (configurationProvider.IsDisableRecoverability)
            {
                var recoverability = endpointConfiguration.Recoverability();
                recoverability.Immediate(
                    immediate =>
                    {
                        immediate.NumberOfRetries(0);
                    });
                recoverability.Delayed(
                    delayed =>
                    {
                        delayed.NumberOfRetries(0);
                    });
            }

            // heartbeats
            if (configurationProvider.IsSendHeartbeats)
            {
                endpointConfiguration.SendHeartbeatTo(configurationProvider.ServiceControlAddress);
            }

            // metrics
            if (!isSendOnlyEndpoint)
            {
                if (configurationProvider.IsSendMetrics)
                {
                    var metrics = endpointConfiguration.EnableMetrics();

                    metrics.SendMetricDataToServiceControl(
                        serviceControlMetricsAddress: configurationProvider.ServiceControlMetricsAddress,
                        interval: TimeSpan.FromSeconds(1));
                }
            }

            // installers
            endpointConfiguration.EnableInstallers();            
            
            return endpointConfiguration;
        }
    }
}