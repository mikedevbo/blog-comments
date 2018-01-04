namespace Host
{
    using System;
    using System.ServiceProcess;
    using System.Threading.Tasks;
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
            var endpointConfiguration = new EndpointConfiguration("Samples.WindowsServiceAndConsole");
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.EnableInstallers();
            this.endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
        }

        protected override void OnStop()
        {
            this.endpointInstance?.Stop().GetAwaiter().GetResult();
        }
    }
}
