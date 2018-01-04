namespace Host
{
    using System;
    using System.Data.SqlClient;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Common;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;
    using NServiceBus.Persistence.Sql;

    public class ProgramService : ServiceBase
    {
        private IEndpointInstance endpointInstance;

        public static void Main()
        {
            using (var service = new ProgramService())
            {
                if (ServiceHelper.IsService())
                {
                    Run(service);
                    return;
                }

                Console.Title = "Host";
                service.OnStart(null);

                Console.WriteLine("Endpoint started. Press any key to exit");
                Console.ReadKey();

                service.OnStop();
            }
        }

        protected override void OnStart(string[] args)
        {
            this.AsyncOnStart().GetAwaiter().GetResult();
        }

        protected async Task AsyncOnStart()
        {
            var configurationManager = new ConfigurationManager();

            // endpoint configuration
            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);
            endpointConfiguration.SendFailedMessagesTo(configurationManager.NsbErrorQueueName);
            endpointConfiguration.AuditProcessedMessagesTo(configurationManager.NsbAuditQueueName);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.SendOnly();

            // conventions
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(
                type =>
                {
                    return type.Namespace == typeof(CreateBranch).Namespace;
                });
            conventions.DefiningEventsAs(
                type =>
                {
                    return type.Namespace == typeof(IBranchCreated).Namespace;
                });

            // transport
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(configurationManager.NsbTransportConnectionString);
            var routing = transport.Routing();
            routing.RouteToEndpoint(
                assembly: typeof(CreateBranch).Assembly,
                destination: configurationManager.NsbEndpointName);
            routing.RegisterPublisher(
                assembly: typeof(IBranchCreated).Assembly,
                publisherEndpoint: configurationManager.NsbEndpointName);

            // persistence
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlVariant(SqlVariant.MsSqlServer);
            persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(configurationManager.NsbTransportConnectionString);
                });

            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.DisableCache();

            // outbox
            endpointConfiguration.EnableOutbox();

            // recoverability
            if (configurationManager.DevMode == DevMode.Dev)
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

            // installers
            endpointConfiguration.EnableInstallers();

            // endpoint start
            this.endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
        }

        protected override void OnStop()
        {
            this.endpointInstance?.Stop().GetAwaiter().GetResult();
        }
    }
}
