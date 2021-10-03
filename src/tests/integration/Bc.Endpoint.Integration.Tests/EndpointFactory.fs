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

    let transport = endpoint.UseTransport<SqlServerTransport>()

    let routing = transport.Routing()
    routing.RouteToEndpoint((typeof<RequestCreateGitHubPullRequest>).Assembly, bcEndpointAssemblyName)
    routing.RouteToEndpoint((typeof<TakeComment>.Assembly), bcEndpointAssemblyName)

    NServiceBus.Endpoint.Start(endpoint)