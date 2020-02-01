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

// outsite script parameter names
let winSCPExecutablePathParamName = "winSCPExecutablePath"
let ftpHostNameParamName = "ftpHostName"
let ftpUserNameParamName = "ftpUserName"
let ftpPasswordParamName = "ftpPassword"
let ftpTlsHostCertificateFingerprintParamName = "ftpTlsHostCertificateFingerprint"
let ftpEndpointPathParamName = "ftpEndpointPath"
let ftpEndpointBackupPathParamName = "ftpEndpointBackupPath"
let ftpOfflineHtmParamName = "ftpOfflineHtm"
let ftpOnlineHtmParamName = "ftpOnlineHtm"

let buildArtifactsPathParamName = @"buildArtifactsPath"

let settingsPathParamName = "settingsPath"
let nservicebusPathParamName = "nservicebusPath"
let deployArtifactsPathParamName = "deployArtifactsPath"

let endpointUrlParamName = "endpointUrl"

let retrieveParam paramName =
    Environment.environVarOrFail paramName

let makeFtpAction action =
    let winSCPExecutablePath = retrieveParam winSCPExecutablePathParamName
    let ftpHostName = retrieveParam ftpHostNameParamName
    let ftpUserName = retrieveParam ftpUserNameParamName
    let ftpPassword = retrieveParam ftpPasswordParamName
    let ftpTlsHostCertificateFingerprint = retrieveParam ftpTlsHostCertificateFingerprintParamName
    
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
    let ftpEndpointPath = retrieveParam ftpEndpointPathParamName
    
    Trace.trace (sprintf "Directory %s." ftpEndpointPath)

    let isDirectoryExists = makeFtpAction (fun ftp -> ftp.FileExists(ftpEndpointPath))
    match isDirectoryExists with
    | true ->
        Trace.trace (sprintf "Directory %s already exists." ftpEndpointPath)
    
    | false ->
        makeFtpAction (fun ftp -> ftp.CreateDirectory(ftpEndpointPath))
        Trace.trace (sprintf "Directory %s created successfully." ftpEndpointPath)
)

Target.create "Stop Endpoint" (fun _ ->
    let ftpEndpointPath = retrieveParam ftpEndpointPathParamName
    let ftpOfflineHtm = retrieveParam ftpOfflineHtmParamName
    let ftpOnlineHtm = retrieveParam ftpOnlineHtmParamName
    let endpointUrl = retrieveParam endpointUrlParamName
    
    Trace.trace (sprintf "Endpoint %s." ftpEndpointPath)

    let offline = sprintf @"%s/%s" ftpEndpointPath ftpOfflineHtm
    let online = sprintf @"%s/%s" ftpEndpointPath ftpOnlineHtm

    let isDirectoryEmpty = makeFtpAction (fun ftp -> ftp.EnumerateRemoteFiles(ftpEndpointPath, null, EnumerationOptions.None) |> Seq.isEmpty)
    match isDirectoryEmpty with
    | true ->
        Trace.trace (sprintf "Endpoint %s is not started yet." ftpEndpointPath)
    
    | false ->
        let isStopped = makeFtpAction (fun ftp -> ftp.FileExists(online))
        match isStopped with
        | true ->
            Trace.trace (sprintf "Endpoint %s is already stopped." ftpEndpointPath)
        
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
    let ftpEndpointPath = retrieveParam ftpEndpointPathParamName
    let ftpEndpointBackupPath = retrieveParam ftpEndpointBackupPathParamName
    
    Trace.trace (sprintf "Endpoint %s." ftpEndpointPath)

    let isDirectoryExists = makeFtpAction (fun ftp -> ftp.FileExists(ftpEndpointBackupPath))
    match isDirectoryExists with
    | true ->
        ()
    
    | false ->
        makeFtpAction (fun ftp -> ftp.CreateDirectory(ftpEndpointBackupPath))
        Trace.trace (sprintf "Directory %s created successfully." ftpEndpointPath)

    makeFtpAction (fun ftp -> ftp.RemoveFiles(ftpEndpointBackupPath).Check())
    makeFtpAction (fun ftp -> ftp.MoveFile(ftpEndpointPath, ftpEndpointBackupPath))
    makeFtpAction (fun ftp -> ftp.CreateDirectory(ftpEndpointPath))
    
    Trace.trace (sprintf "Endpoint %s backuped successfully." ftpEndpointPath)
)

Target.create "Deploy Endpoint" (fun _ ->
    let ftpEndpointPath = retrieveParam ftpEndpointPathParamName
    let deployArtifactsPath = retrieveParam deployArtifactsPathParamName
    let buildArtifactsPath = retrieveParam buildArtifactsPathParamName
    let settingsPath = retrieveParam settingsPathParamName
    let nservicebusPath = retrieveParam nservicebusPathParamName
    let endpointUrl = retrieveParam endpointUrlParamName
    
    Trace.trace (sprintf "Endpoint %s." ftpEndpointPath)

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

    Trace.trace (sprintf "Endpoint %s deployed successfully." ftpEndpointPath)
)

// Dependencies
open Fake.Core.TargetOperators

"Create Directory"
    ==>"Stop Endpoint"
    ==> "Backup Endpoint"
    ==> "Deploy Endpoint"


// start build
Target.runOrDefaultWithArguments "Deploy Endpoint"