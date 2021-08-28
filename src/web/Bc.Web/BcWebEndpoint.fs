namespace Bc.Web

open Bc.Common.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
open NServiceBus
open NServiceBus.Persistence.Sql

type BcWebEndpoint() =

    static member GetEndpoint() =
        let endpointName = "Bc.WebEndpoint"
        let destinationEndpointName = "Bc.Endpoint"
            
        let endpoint = EndpointCommon.GetEndpoint(
                                        endpointName,
                                        true,
                                        EndpointCommonConfigurationProvider())

        let transport = endpoint.UseTransport<SqlServerTransport>()
        let routing = transport.Routing()
        routing.RouteToEndpoint((typeof<TakeComment>).Assembly, destinationEndpointName)

        endpoint