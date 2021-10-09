module Bc.Endpoint.Integration.Tests.EndpointFactory

open System.Threading.Tasks
open Bc.Common.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
open Bc.Contracts.Internals.Endpoint.GitHubPullRequestCreation.Messages
open NServiceBus

let getSenderEndpoint () =
    let bcEndpointAssemblyName = "Bc.Endpoint"

    let endpoint = getEndpoint "Sender.Tests" false

    let scanner = endpoint.AssemblyScanner()
    scanner.ExcludeAssemblies(bcEndpointAssemblyName)

    // routing
    let setRouting (transport: TransportExtensions<'T>) =
       transport.Routing().RouteToEndpoint((typeof<TakeComment>).Assembly, bcEndpointAssemblyName)

    match ConfigurationProvider.isUseLearningTransportAndPersistence with
    | false -> setRouting (endpoint.UseTransport<SqlServerTransport>())
    | true -> setRouting (endpoint.UseTransport<LearningTransport>())

    NServiceBus.Endpoint.Start(endpoint)