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

            // // conventions
            // var conventions = endpointConfiguration.Conventions();
            // conventions.DefiningCommandsAs(
            //     type =>
            //     {
            //         return type.Namespace == typeof(StartAddingComment).Namespace;
            //     });
            // conventions.DefiningMessagesAs(
            //     type =>
            //     {
            //         return
            //             type.Namespace == typeof(RequestCreateBranch).Namespace ||
            //             type == typeof(NServiceBus.Mailer.MailMessage);
            //     });

            // transport
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(configurationProvider.TransportConnectionString);
            transport.DefaultSchema(Schema);

            // // routing
            // var routing = transport.Routing();
            // routing.RouteToEndpoint(
            //     assembly: typeof(StartAddingComment).Assembly,
            //     destination: this.configurationManager.NsbEndpointName);

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

            // // mailer
            // var mailSettings = endpointConfiguration.EnableMailer();
            // mailSettings.UseSmtpBuilder(buildSmtpClient: () =>
            // {
            //     var smtpClient = new SmtpClient();
            //
            //     if (!this.configurationManager.IsSendEmail)
            //     {
            //         var directoryLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Emails");
            //         Directory.CreateDirectory(directoryLocation);
            //         smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            //         smtpClient.PickupDirectoryLocation = directoryLocation;
            //     }
            //     else
            //     {
            //         smtpClient.Host = this.configurationManager.SmtpHost;
            //         smtpClient.Port = this.configurationManager.SmtpPort;
            //         smtpClient.EnableSsl = true;
            //         smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //         smtpClient.UseDefaultCredentials = false;
            //         smtpClient.Credentials = new NetworkCredential(
            //             this.configurationManager.SmtpHostUserName,
            //             this.configurationManager.SmtpHostPassword);
            //     }
            //
            //     return smtpClient;
            // });

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