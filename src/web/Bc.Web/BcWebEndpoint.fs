namespace Bc.Web

open Bc.Common.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
open NServiceBus
open NServiceBus.Persistence.Sql

type BcWebEndpoint() =

    static member GetEndpoint() =
        let endpointName = "Bc.WebEndpoint"
        let destinationEndpointName = "Bc.Endpoint"

        let endpoint = getEndpoint endpointName true

        // routing
        let setRouting (transport: TransportExtensions<'T>) =
           transport.Routing().RouteToEndpoint((typeof<TakeComment>).Assembly, destinationEndpointName)

        match ConfigurationProvider.isUseLearningTransportAndPersistence with
        | false -> setRouting (endpoint.UseTransport<SqlServerTransport>())
        | true -> setRouting (endpoint.UseTransport<LearningTransport>())

        endpoint