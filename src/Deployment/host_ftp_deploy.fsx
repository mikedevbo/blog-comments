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

// outside script parameter names
let winSCPExecutablePathParamName = "winSCPExecutablePath"
let ftpHostNameParamName = "ftpHostName"
let ftpUserNameParamName = "ftpUserName"
let ftpPasswordParamName = "ftpPassword"
let ftpTlsHostCertificateFingerprintParamName = "ftpTlsHostCertificateFingerprint"
let ftpEndpointPathParamName = "ftpEndpointPath"
let ftpEndpointBackupPathParamName = "ftpEndpointBackupPath"
let ftpOfflineHtmParamName = "ftpOfflineHtm"
let ftpOnlineHtmParamName = "ftpOnlineHtm"

let buildArtifactsPathParamName = "buildArtifactsPath"
let localEndpointBackupPathParameName = "localEndpointBackupPath"

let settingsPathParamName = "settingsPath"
let nservicebusPathParamName = "nservicebusPath"
let deployArtifactsPathParamName = "deployArtifactsPath"

let endpointUrlParamName = "endpointUrl"
//

type EndpointState = NotExists | Stopped | Running

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
Target.create "Stop Endpoint" (fun _ ->
    let ftpEndpointPath = retrieveParam ftpEndpointPathParamName
    let ftpOfflineHtm = retrieveParam ftpOfflineHtmParamName
    let ftpOnlineHtm = retrieveParam ftpOnlineHtmParamName
    let endpointUrl = retrieveParam endpointUrlParamName
    
    Trace.trace ("-> Endpoint " + ftpEndpointPath)

    let offline = sprintf @"%s/%s" ftpEndpointPath ftpOfflineHtm
    let online = sprintf @"%s/%s" ftpEndpointPath ftpOnlineHtm

    let getEndpointState =
        let isDirectoryEmpty = makeFtpAction (fun ftp -> ftp.EnumerateRemoteFiles(ftpEndpointPath, null, EnumerationOptions.None) |> Seq.isEmpty)
        match isDirectoryEmpty with
        | true -> NotExists
        | false ->
            let isStopped = makeFtpAction (fun ftp -> ftp.FileExists(online))
            match isStopped with
            | true -> Stopped
            | false -> Running

    let stopEndpoint =
        makeFtpAction (fun ftp -> ftp.MoveFile(offline, online))
        try
            Trace.trace ("-> Call URL " + endpointUrl)
            Http.get "" "" endpointUrl |> ignore
        with
        | :? System.Net.Http.HttpRequestException as ex ->
            Trace.trace ex.Message

            let isStatusCorrect = ex.Message.Contains("503");
            match isStatusCorrect with
            | true -> ()
            | false -> reraise()

    match getEndpointState with
    | NotExists -> Trace.trace (sprintf "-> Endpoint %s is not exists yet." ftpEndpointPath)
    | Stopped -> Trace.trace (sprintf "-> Endpoint %s is already stopped." ftpEndpointPath)
    | Running ->
        Trace.trace ("-> Stop Endpoint " + ftpEndpointPath)
        stopEndpoint
        Trace.trace (sprintf "-> Endpoint %s stopped successfully." ftpEndpointPath)
)

Target.create "Backup Endpoint" (fun _ ->
    let localEndpointBackupPath = retrieveParam localEndpointBackupPathParameName
    let ftpEndpointPath = retrieveParam ftpEndpointPathParamName
    let ftpEndpointBackupPath = retrieveParam ftpEndpointBackupPathParamName

    Trace.trace ("-> Endpoint " + ftpEndpointPath)

    Trace.trace ("-> Clean " + localEndpointBackupPath)
    Shell.cleanDir localEndpointBackupPath

    Trace.trace ("-> Download files from " + ftpEndpointPath)
    makeFtpAction (fun ftp -> ftp.GetFiles(ftpEndpointPath, localEndpointBackupPath).Check())

    Trace.trace ("-> Remove files from " + ftpEndpointBackupPath)
    makeFtpAction (fun ftp -> ftp.RemoveFiles(ftpEndpointBackupPath + "/*").Check())

    Trace.trace ("-> Upload files to " + ftpEndpointBackupPath)
    makeFtpAction (fun ftp -> ftp.PutFiles(localEndpointBackupPath, ftpEndpointBackupPath).Check())

    Trace.trace (sprintf "-> Endpoint %s backuped successfully." ftpEndpointPath)
)

Target.create "Deploy Endpoint" (fun _ ->
    let ftpEndpointPath = retrieveParam ftpEndpointPathParamName
    let deployArtifactsPath = retrieveParam deployArtifactsPathParamName
    let buildArtifactsPath = retrieveParam buildArtifactsPathParamName
    let settingsPath = retrieveParam settingsPathParamName
    let nservicebusPath = retrieveParam nservicebusPathParamName
    let endpointUrl = retrieveParam endpointUrlParamName

    let removeRemoteItem item =
        let isItemExists = makeFtpAction (fun ftp -> ftp.FileExists(item))
        if isItemExists then makeFtpAction (fun ftp -> ftp.RemoveFiles(item).Check())
    
    Trace.trace (sprintf "-> Endpoint %s." ftpEndpointPath)

    Trace.trace ("-> Clean " + deployArtifactsPath)
    Shell.cleanDir deployArtifactsPath

    Trace.trace ("-> Copy " + buildArtifactsPath)
    Shell.copyDir deployArtifactsPath buildArtifactsPath FileFilter.allFiles

    Trace.trace ("-> Copy " + settingsPath)
    Shell.copyDir deployArtifactsPath settingsPath FileFilter.allFiles

    Trace.trace ("-> Copy " + nservicebusPath)
    Shell.copyDir deployArtifactsPath nservicebusPath FileFilter.allFiles

    Trace.trace ("-> Clean " + ftpEndpointPath)
    // protects against starting Endpoint after removing offline htm
    removeRemoteItem(ftpEndpointPath + "/web.config")
    makeFtpAction (fun ftp -> ftp.RemoveFiles(ftpEndpointPath + "/*").Check())

    Trace.trace ("-> Upload files to  " + ftpEndpointPath)
    makeFtpAction (fun ftp -> ftp.PutFiles(deployArtifactsPath, ftpEndpointPath).Check())

    Trace.trace ("-> Call URL " + endpointUrl)
    Http.get "" "" endpointUrl |> ignore

    Trace.trace (sprintf "-> Endpoint %s deployed successfully." ftpEndpointPath)
)

// Dependencies
open Fake.Core.TargetOperators

"Stop Endpoint"
    ==> "Backup Endpoint"
    ==> "Deploy Endpoint"

// start build
Target.runOrDefaultWithArguments "Deploy Endpoint"