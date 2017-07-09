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
            var builder = new ContainerBuilder();

            // Register the MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            // Register the endpoint as a factory as the instance isn't created yet.
            builder.Register(ctx => endpoint).SingleInstance();

            var container = builder.Build();

            // Set the dependency resolver to be Autofac.
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var endpointConfiguration = new EndpointConfiguration("blogcomments");
            endpointConfiguration.MakeInstanceUniquelyAddressable("1");
            endpointConfiguration.EnableCallbacks();
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.UseContainer<AutofacBuilder>(
                customizations: customizations =>
                {
                    customizations.ExistingLifetimeScope(container);
                });
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(
                type =>
                {
                    return type.Namespace == "Messages.Commands";
                });

            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }
    }
}