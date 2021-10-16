module Bc.Endpoint

open System
open System.Configuration
open System.Security
open System.IO
open System.Net
open System.Net.Mail
open System.Reflection
open Bc.Common.Endpoint
open Bc.Contracts.Internals.Endpoint.CommentTaking.Commands
open NServiceBus
open NServiceBus.Logging
open NServiceBus.Mailer
open NServiceBus.Persistence.Sql

[<assembly:SqlPersistenceSettings(MsSqlServerScripts = true)>]
do ()

[<RequireQualifiedAccessAttribute>]
module ConfigurationProvider =

    let isSendEmail = Convert.ToBoolean(ConfigurationManager.AppSettings.["IsSendEmail"])
    let smtpHost = ConfigurationManager.AppSettings.["SmtpHost"]
    let smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings.["SmtpPort"])
    let smtpHostUserName = ConfigurationManager.AppSettings.["SmtpHostUserName"]
    let smtpHostPassword = fun _ ->
            let pass = new SecureString()
            ConfigurationManager.AppSettings.["SmtpHostPassword"] |> String.iter (fun c -> pass.AppendChar(c))
            pass

let getEndpoint () =
    let endpointName = "Bc.Endpoint"

    let endpoint = getEndpoint endpointName false

    // logging
    let defaultFactory = LogManager.Use<DefaultFactory>()
    defaultFactory.Directory(AppDomain.CurrentDomain.BaseDirectory)

    // routing
    let setRouting (transport: TransportExtensions<'T>) =
       transport.Routing().RouteToEndpoint((typeof<TakeComment>).Assembly, endpointName)

    match ConfigurationProvider.isUseLearningTransportAndPersistence with
    | false -> setRouting (endpoint.UseTransport<SqlServerTransport>())
    | true -> setRouting (endpoint.UseTransport<LearningTransport>())

    // mailer
    let mailSettings = endpoint.EnableMailer()
    mailSettings.UseSmtpBuilder(fun _ ->
        let smtpClient = new SmtpClient()

        if  not ConfigurationProvider.isSendEmail then
            let directoryLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Emails")
            Directory.CreateDirectory(directoryLocation) |> ignore
            smtpClient.DeliveryMethod <- SmtpDeliveryMethod.SpecifiedPickupDirectory
            smtpClient.PickupDirectoryLocation <- directoryLocation
        else
            smtpClient.Host <- ConfigurationProvider.smtpHost
            smtpClient.Port <- ConfigurationProvider.smtpPort
            smtpClient.EnableSsl <- true
            smtpClient.DeliveryMethod <- SmtpDeliveryMethod.Network
            smtpClient.UseDefaultCredentials <- false
            smtpClient.Credentials <- NetworkCredential(
                ConfigurationProvider.smtpHostUserName,
                ConfigurationProvider.smtpHostPassword ())

        smtpClient
    )

    endpoint