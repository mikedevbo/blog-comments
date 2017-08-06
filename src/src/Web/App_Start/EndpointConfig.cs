using Autofac;
using Autofac.Integration.Mvc;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Web
{
    public class EndpointConfig
    {
        public static void RegisterEndpoint(IEndpointInstance endpoint)
        {
            var configurationManager = new ConfigurationManager();
            var builder = new ContainerBuilder();

            // container
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.Register(ctx => endpoint).SingleInstance();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);

            // container
            endpointConfiguration.UseContainer<AutofacBuilder>(
                customizations: customizations =>
                {
                    customizations.ExistingLifetimeScope(container);
                });

            // error & audit
            endpointConfiguration.SendFailedMessagesTo(configurationManager.NsbErrorQueueName);
            endpointConfiguration.AuditProcessedMessagesTo(configurationManager.NsbAuditQueueName);

            // callbacks
            endpointConfiguration.EnableCallbacks();
            endpointConfiguration.MakeInstanceUniquelyAddressable(configurationManager.NsbEndpointInstanceId);

            // serialization
            endpointConfiguration.UseSerialization<JsonSerializer>();

            // convenstions
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(
                type =>
                {
                    return type.Namespace == "Messages.Commands";
                });
            conventions.DefiningEventsAs(
                type =>
                {
                    return type.Namespace == "Messages.Events";
                });

            // transport
            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionString(configurationManager.NsbTransportConnectionString);

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


            // installers
            endpointConfiguration.EnableInstallers();

            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }
    }
}