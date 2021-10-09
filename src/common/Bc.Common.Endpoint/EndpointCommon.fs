module Bc.Common.Endpoint

open System
open System.Configuration
open System.Data.Common
open Microsoft.Data.SqlClient
open NServiceBus


[<RequireQualifiedAccessAttribute>]
module ConfigurationProvider =

    let transportConnectionString = ConfigurationManager.AppSettings.["TransportConnectionString"]
    let isDisableRecoverability = Convert.ToBoolean(ConfigurationManager.AppSettings.["IsDisableRecoverability"])
    let isSendHeartbeats = Convert.ToBoolean(ConfigurationManager.AppSettings.["IsSendHeartbeats"])
    let serviceControlAddress = ConfigurationManager.AppSettings.["ServiceControlAddress"]
    let isSendMetrics = Convert.ToBoolean(ConfigurationManager.AppSettings.["IsSendMetrics"])
    let serviceControlMetricsAddress = ConfigurationManager.AppSettings.["ServiceControlMetricsAddress"]
    let isUseLearningTransportAndPersistence = Convert.ToBoolean(ConfigurationManager.AppSettings.["IsUseLearningTransportAndPersistence"])

let getEndpoint endpointName isSendOnlyEndpoint =

    let schema = "nsb"
    let endpointConfiguration = EndpointConfiguration(endpointName)

    // basic configuration
    match isSendOnlyEndpoint with
    | false -> ()
    | true -> endpointConfiguration.SendOnly()

    endpointConfiguration.SendFailedMessagesTo("error")
    endpointConfiguration.AuditProcessedMessagesTo("audit")
    endpointConfiguration.UseSerialization<NewtonsoftSerializer>() |> ignore

    // host identifier
    endpointConfiguration.UniquelyIdentifyRunningInstance()
                         .UsingNames(instanceName = endpointName, hostName = Environment.MachineName) |> ignore

    // conventions
    let conventions = endpointConfiguration.Conventions()
    conventions.Add(
        { new IMessageConvention with
            member this.Name = "Type name suffix"
            member this.IsCommandType(t) = not(isNull t.Namespace) && t.Namespace.EndsWith("Commands")
            member this.IsEventType(t) = not(isNull t.Namespace) && t.Namespace.EndsWith("Events")
            member this.IsMessageType(t) = not(isNull t.Namespace) && t.Namespace.EndsWith("Messages")}) |> ignore

    match ConfigurationProvider.isUseLearningTransportAndPersistence with
    | false ->
        // transport
        let transport = endpointConfiguration.UseTransport<SqlServerTransport>()
        transport.ConnectionString(ConfigurationProvider.transportConnectionString) |> ignore
        transport.DefaultSchema(schema) |> ignore

        // persistence
        let persistence = endpointConfiguration.UsePersistence<SqlPersistence>()
        persistence.ConnectionBuilder(fun _ -> new SqlConnection(ConfigurationProvider.transportConnectionString) :> DbConnection) |> ignore

        let dialect = persistence.SqlDialect<SqlDialect.MsSqlServer>()
        dialect.Schema(schema)

        let subscriptions = persistence.SubscriptionSettings()
        subscriptions.DisableCache()

        // outbox
        endpointConfiguration.EnableOutbox() |> ignore
    | true ->
        endpointConfiguration.UseTransport<LearningTransport>() |> ignore
        endpointConfiguration.UsePersistence<LearningPersistence>() |> ignore

    // recoverability
    match ConfigurationProvider.isDisableRecoverability with
    | false ->
        ()
    | true ->
        let recoverability = endpointConfiguration.Recoverability()
        recoverability.Immediate(fun immediate -> immediate.NumberOfRetries(0) |> ignore) |> ignore
        recoverability.Delayed(fun delayed -> delayed.NumberOfRetries(0) |> ignore) |> ignore
        ()

    // heartbeats
    match ConfigurationProvider.isSendHeartbeats with
    | false -> ()
    | true -> endpointConfiguration.SendHeartbeatTo(ConfigurationProvider.serviceControlAddress)

    // metrics
    match isSendOnlyEndpoint with
    | false ->
        match ConfigurationProvider.isSendMetrics with
        | false ->
            ()
        | true ->
            let metrics = endpointConfiguration.EnableMetrics()
            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress = ConfigurationProvider.serviceControlMetricsAddress,
                interval = TimeSpan.FromSeconds(float 1))
    | true ->
        ()

    // installers
    endpointConfiguration.EnableInstallers()

    endpointConfiguration