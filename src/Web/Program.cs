namespace Web
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.ConfigureAppConfiguration((hostingContext, config) =>
                //{
                //    config.AddJsonFile("appsettings.web.dev.json", optional: true, reloadOnChange: true);
                //})
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
