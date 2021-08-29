namespace Bc.Host.AspNet.Endpoint

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Bc.Endpoint
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open NServiceBus

module Program =
    let exitCode = 0

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .UseNServiceBus(fun nsbBuilder ->
                BcEndpoint.GetEndpoint(EndpointConfigurationProvider()))
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseStartup<Startup>() |> ignore
            )

    [<EntryPoint>]
    let main args =
        CreateHostBuilder(args).Build().Run()

        exitCode
