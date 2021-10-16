namespace Bc.Web

open Bc.Common.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
open NServiceBus
open NServiceBus.Logging
open NServiceBus.Persistence.Sql
open Microsoft.Extensions.Hosting
open System

type BcWebEndpoint() =

    static member GetEndpoint() =
        let endpointName = "Bc.WebEndpoint"
        let destinationEndpointName = "Bc.Endpoint"

        let endpoint = getEndpoint endpointName true

        // logging
        let defaultFactory = LogManager.Use<DefaultFactory>()
        defaultFactory.Directory(AppDomain.CurrentDomain.BaseDirectory)

        // routing
        let setRouting (transport: TransportExtensions<'T>) =
           transport.Routing().RouteToEndpoint((typeof<TakeComment>).Assembly, destinationEndpointName)

        match ConfigurationProvider.isUseLearningTransportAndPersistence with
        | false -> setRouting (endpoint.UseTransport<SqlServerTransport>())
        | true -> setRouting (endpoint.UseTransport<LearningTransport>())

        endpoint