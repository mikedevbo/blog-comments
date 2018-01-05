namespace Host
{
    using System;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using Common;
    using Components;
    using Components.GitHub;
    using NServiceBus;

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
            var endpointInitializer = new EndpointInitializer(configurationManager);
            var endpointConfiguration = new EndpointConfiguration(configurationManager.NsbEndpointName);

            // initialize endpoint
            endpointInitializer.Initialize(endpointConfiguration);

            // register components
            endpointConfiguration.RegisterComponents(reg =>
                reg.ConfigureComponent<ComponentsConfigurationManager>(DependencyLifecycle.InstancePerCall));

            endpointConfiguration.RegisterComponents(reg =>
                            reg.ConfigureComponent<GitHubApi>(DependencyLifecycle.InstancePerCall));

            endpointConfiguration.RegisterComponents(reg =>
                            reg.ConfigureComponent<ConfigurationManager>(DependencyLifecycle.InstancePerCall));

            endpointConfiguration.RegisterComponents(reg =>
                                        reg.ConfigureComponent<EmailSender>(DependencyLifecycle.InstancePerCall));

            // start endpoint
            this.endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
        }

        protected override void OnStop()
        {
            this.endpointInstance?.Stop().GetAwaiter().GetResult();
        }
    }
}
