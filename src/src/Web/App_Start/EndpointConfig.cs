using Autofac;
using Autofac.Integration.Mvc;
using NServiceBus;
using System.Web.Mvc;

namespace Web
{
    public class EndpointConfig
    {
        public static void RegisterEndpoint(IEndpointInstance endpoint)
        {
            var configurationManager = new ConfigurationManager();
            var builder = new ContainerBuilder();

            // Register the MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            // Register the endpoint as a factory as the instance isn't created yet.
            builder.Register(ctx => endpoint).SingleInstance();

            var container = builder.Build();

            // Set the dependency resolver to be Autofac.
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);
            endpointConfiguration.MakeInstanceUniquelyAddressable(configurationManager.NsbEndpointInstanceId);
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.EnableCallbacks();
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.UseContainer<AutofacBuilder>(
                customizations: customizations =>
                {
                    customizations.ExistingLifetimeScope(container);
                });
            
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

            var transport = endpointConfiguration.UseTransport<SqlServerTransport>();
            transport.ConnectionStringName(configurationManager.NsbTransportConnectionStringName);
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            endpointConfiguration.EnableOutbox();

            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }
    }
}