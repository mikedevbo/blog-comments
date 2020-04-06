namespace Host.CommentPolicy
{
    using Common;
    using Components.GitHub;
    using Components.Logic;
    using Microsoft.Extensions.Configuration;
    using NServiceBus;

    public static class EndpointManager
    {
        public static IConfiguration BuildConfiguration(string basePath)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.host.commentpolicy.dev.json", true, true)
            .AddJsonFile("appsettings.host.commentpolicy.test.json", true, true)
            .AddJsonFile("appsettings.host.commentpolicy.production.json", true, true)
            .Build();

            return configuration;
        }

        public static IEndpointInstance StartEndpoint(IConfiguration configuration)
        {
            // initialize endpoint
            var configurationManager = new ConfigurationManager(configuration);
            var configurtionProvider = new Common.ConfigurationProvider(configuration);
            var endpointInitializer = new EndpointInitializer(configurationManager);
            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);
            endpointInitializer.Initialize(endpointConfiguration, false);

            // register components
            if (configurationManager.IsUseFakes)
            {
                endpointConfiguration.RegisterComponents(reg =>
                {
                    //reg.ConfigureComponent<GitHubApiForTests>(DependencyLifecycle.InstancePerCall);
                    reg.ConfigureComponent<CommentPolicyLogicFake>(DependencyLifecycle.InstancePerCall);
                });


            }
            else
            {
                endpointConfiguration.RegisterComponents(reg =>
                    reg.ConfigureComponent<GitHubApi>(DependencyLifecycle.InstancePerCall));
            }

            endpointConfiguration.RegisterComponents(reg =>
            {
                reg.ConfigureComponent(() => configurationManager, DependencyLifecycle.InstancePerCall);
                reg.ConfigureComponent(() => configurtionProvider, DependencyLifecycle.InstancePerCall);
            });

            // start endpoint
            return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        }
    }
}
