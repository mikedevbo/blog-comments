namespace Common
{
    using System;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;
    using Messages.Commands;
    using Messages.Messages;
    using NServiceBus;
    using NServiceBus.Mailer;
    using NServiceBus.Persistence.Sql;

    public class EndpointInitializer : IEndpointInitializer
    {
        private readonly IConfigurationManager configurationManager;

        public EndpointInitializer(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public void Initialize(EndpointConfiguration endpointConfiguration, bool isSendOnlyEndpoint)
        {
            // endpoint configuration
            endpointConfiguration.SendFailedMessagesTo(this.configurationManager.NsbErrorQueueName);
            endpointConfiguration.AuditProcessedMessagesTo(this.configurationManager.NsbAuditQueueName);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            if (isSendOnlyEndpoint)
            {
                endpointConfiguration.SendOnly();
            }

            // host indentifier
            endpointConfiguration.UniquelyIdentifyRunningInstance()
                .UsingNames(
                    instanceName: this.configurationManager.NsbEndpointName,
                    hostName: Environment.MachineName);

            // conventions
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(
                type =>
                {
                    return type.Namespace == typeof(StartAddingComment).Namespace;
                });
            conventions.DefiningMessagesAs(
                type =>
                {
                    return
                        type.Namespace == typeof(RequestCreateBranch).Namespace ||
                        type == typeof(NServiceBus.Mailer.MailMessage);
                });

            // transport
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(this.configurationManager.NsbTransportConnectionString);
            var routing = transport.Routing();

            routing.RouteToEndpoint(
                assembly: typeof(StartAddingComment).Assembly,
                destination: this.configurationManager.NsbEndpointName);

            // persistence
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(this.configurationManager.NsbTransportConnectionString);
                });

            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.DisableCache();

            // outbox
            endpointConfiguration.EnableOutbox();

            // recoverability
            if (this.configurationManager.NsbIsDisableRecoverability)
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

            // mailer
            var mailSettings = endpointConfiguration.EnableMailer();
            mailSettings.UseSmtpBuilder(buildSmtpClient: () =>
            {
                var smtpClient = new SmtpClient();

                if (this.configurationManager.DevMode != DevMode.Production)
                {
                    var directoryLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Emails");
                    Directory.CreateDirectory(directoryLocation);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = directoryLocation;
                }
                else
                {
                    smtpClient.Host = this.configurationManager.SmtpHost;
                    smtpClient.Port = this.configurationManager.SmtpPort;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(
                        this.configurationManager.SmtpHostUserName,
                        this.configurationManager.SmtpHostPassword);
                }

                return smtpClient;
            });

            // heartbeats
            if (this.configurationManager.NsbIsSendHeartbeats)
            {
                endpointConfiguration.SendHeartbeatTo(serviceControlQueue: this.configurationManager.NsbServiceControlQueueName);
            }

            // metrics
            if (!isSendOnlyEndpoint)
            {
                if (this.configurationManager.NsbIsSendMetrics)
                {
                    var metrics = endpointConfiguration.EnableMetrics();

                    metrics.SendMetricDataToServiceControl(
                        serviceControlMetricsAddress: this.configurationManager.NsbServiceControlMetricsQueueName,
                        interval: TimeSpan.FromSeconds(10));
                }
            }

            // installers
            endpointConfiguration.EnableInstallers();
        }
    }
}
