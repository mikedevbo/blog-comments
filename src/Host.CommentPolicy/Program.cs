namespace Host.CommentPolicy
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var configuration = EndpointManager.BuildConfiguration(Directory.GetCurrentDirectory());
                var endpointInstance = EndpointManager.StartEndpoint(configuration);

                Console.WriteLine("Ready");
                Console.ReadKey();
                endpointInstance.Stop().GetAwaiter().GetResult();

                return;
            }

            CurrentDirectoryHelpers.SetCurrentDirectory();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
