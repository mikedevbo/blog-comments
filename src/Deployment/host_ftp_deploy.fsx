#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fake.Net.Http
nuget WinSCP //"
#load "./.fake/host_ftp_deploy.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.Net
open WinSCP

// set proper values
let winSCPExecutablePath = @"..."
let ftpHostName = "..."
let ftpUserName = "..."
let ftpPassword = "..."
let ftpTlsHostCertificateFingerprint = "..."
let ftpEndpointPath = "..."
let ftpEndpointBackupPath = "..."
let ftpOfflineHtm = "..."
let ftpOnlineHtm = "..."

let buildArtifactsPath = @"..."

let settingsPath = @"..."
let nservicebusPath = @"..."
let deployArtifactsPath = @"..."

let endpointUrl = "..."

let makeFtpAction action =
    let sessionOptions = new SessionOptions()

    sessionOptions.Protocol <- Protocol.Ftp
    sessionOptions.FtpSecure <- FtpSecure.Explicit
    sessionOptions.TlsHostCertificateFingerprint <- ftpTlsHostCertificateFingerprint

    sessionOptions.HostName <- ftpHostName
    sessionOptions.UserName <- ftpUserName
    sessionOptions.Password <- ftpPassword

    use session = new Session()
    session.ExecutablePath <- winSCPExecutablePath
    session.Open(sessionOptions)
    action session

// Targets
Target.create "Create Directory" (fun _ ->
    Trace.trace (sprintf "Start creating directory %s" ftpEndpointPath)

    let isDirectoryExists = makeFtpAction (fun ftp -> ftp.FileExists(ftpEndpointPath))
    match isDirectoryExists with
    | true ->
        ()
    | false ->
        makeFtpAction (fun ftp -> ftp.CreateDirectory(ftpEndpointPath))

    Trace.trace (sprintf "Directory %s created successfully." ftpEndpointPath)
)

Target.create "Stop Endpoint" (fun _ ->

    Trace.trace (sprintf "Start stopping Endpoint %s" ftpEndpointPath)

    let offline = sprintf @"%s/%s" ftpEndpointPath ftpOfflineHtm
    let online = sprintf @"%s/%s" ftpEndpointPath ftpOnlineHtm

    let isDirectoryEmpty = makeFtpAction (fun ftp -> ftp.EnumerateRemoteFiles(ftpEndpointPath, null, EnumerationOptions.None) |> Seq.isEmpty)
    match isDirectoryEmpty with
    | true ->
        ()
    | false ->
        let isStopped = makeFtpAction (fun ftp -> ftp.FileExists(online))
        match isStopped with
        | true ->
            ()
        | false ->
            makeFtpAction (fun ftp -> ftp.MoveFile(offline, online))

            try
                Trace.trace (sprintf "call url %s" endpointUrl)
                Http.get "" "" endpointUrl |> ignore
            with
            | :? System.Net.Http.HttpRequestException as ex ->
                Trace.trace ex.Message

                let isStatusCorrect = ex.Message.Contains("503");
                match isStatusCorrect with
                | true ->
                    ()
                | false ->
                    reraise()

    Trace.trace (sprintf "Endpoint %s stopped successfully." ftpEndpointPath)
)

Target.create "Backup Endpoint" (fun _ ->
    Trace.trace (sprintf "Start backuping Endpoint %s" ftpEndpointPath)

    let isDirectoryExists = makeFtpAction (fun ftp -> ftp.FileExists(ftpEndpointBackupPath))
    match isDirectoryExists with
    | true ->
        makeFtpAction (fun ftp -> ftp.RemoveFiles(ftpEndpointBackupPath).Check())
        makeFtpAction (fun ftp -> ftp.MoveFile(ftpEndpointPath, ftpEndpointBackupPath))
        makeFtpAction (fun ftp -> ftp.CreateDirectory(ftpEndpointPath))
    | false ->
        makeFtpAction (fun ftp -> ftp.CreateDirectory(ftpEndpointBackupPath))

    Trace.trace (sprintf "Endpoint %s backuped successfully" ftpEndpointPath)
)

Target.create "Deploy Endpoint" (fun _ ->
    Trace.trace (sprintf "Start deploying Endpoint %s" ftpEndpointPath)

    Trace.trace "clean deploy arifacts path"
    Shell.cleanDir deployArtifactsPath

    Trace.trace "copy artifacts"
    Shell.copyDir deployArtifactsPath buildArtifactsPath FileFilter.allFiles

    Trace.trace "copy settings"
    Shell.copyDir deployArtifactsPath settingsPath FileFilter.allFiles

    Trace.trace "copy nservicebus"
    Shell.copyDir deployArtifactsPath nservicebusPath FileFilter.allFiles

    Trace.trace "upload artifacts"
    makeFtpAction (fun ftp -> ftp.PutFiles(deployArtifactsPath, ftpEndpointPath).Check())

    Trace.trace (sprintf "call url %s" endpointUrl)
    Http.get "" "" endpointUrl |> ignore

    Trace.trace (sprintf "Endpoint %s deployed successfully" ftpEndpointPath)
)

// Dependencies
open Fake.Core.TargetOperators

"Create Directory"
    ==>"Stop Endpoint"
    ==> "Backup Endpoint"
    ==> "Deploy Endpoint"


// start build
Target.runOrDefault "Deploy Endpoint"