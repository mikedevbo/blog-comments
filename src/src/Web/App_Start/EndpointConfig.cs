namespace Web
{
    using System.Data.SqlClient;
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Components;
    using Components.GitHub;
    using Messages.Commands;
    using Messages.Events;
    using NServiceBus;
    using NServiceBus.Persistence.Sql;

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
            RegisterComponents(endpointConfiguration, configurationManager);

            // error & audit
            endpointConfiguration.SendFailedMessagesTo(configurationManager.NsbErrorQueueName);
            endpointConfiguration.AuditProcessedMessagesTo(configurationManager.NsbAuditQueueName);

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

            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }

        public static void RegisterComponents(
            EndpointConfiguration endpointConfiguration,
            IConfigurationManager configurationManager)
        {
            endpointConfiguration.RegisterComponents(reg => reg.ConfigureComponent<ConfigurationManager>(DependencyLifecycle.InstancePerCall));
            endpointConfiguration.RegisterComponents(reg => reg.ConfigureComponent<ComponentsConfigurationManager>(DependencyLifecycle.InstancePerCall));

            if (configurationManager.NsbIsIntegrationTests)
            {
                endpointConfiguration.RegisterComponents(reg => reg.ConfigureComponent<GitHubApiForTests>(DependencyLifecycle.InstancePerCall));
            }
            else
            {
                endpointConfiguration.RegisterComponents(reg => reg.ConfigureComponent<GitHubApi>(DependencyLifecycle.InstancePerCall));
            }

            if (configurationManager.DevMode == DevMode.Production)
            {
                endpointConfiguration.RegisterComponents(reg => reg.ConfigureComponent<EmailSender>(DependencyLifecycle.InstancePerCall));
            }
            else
            {
                endpointConfiguration.RegisterComponents(reg => reg.ConfigureComponent<EmailSenderForTests>(DependencyLifecycle.InstancePerCall));
            }
        }
    }
}