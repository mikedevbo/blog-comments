#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fake.Net.Http
nuget WinSCP //"
#load "./.fake/web_ftp_deploy.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.Net
open WinSCP
open System.Xml

// outsite script parameter names
let winSCPExecutablePathParamName = "winSCPExecutablePath"
let ftpHostNameParamName = "ftpHostName"
let ftpUserNameParamName = "ftpUserName"
let ftpPasswordParamName = "ftpPassword"
let ftpTlsHostCertificateFingerprintParamName = "ftpTlsHostCertificateFingerprint"
let ftpWebPathParamName = "ftpWebPath"
let ftpMainWebConfigFilePathParamName = "ftpmainWebConfigFilePath"

let buildArtifactsPathParamName = "buildArtifactsPath"

let settingsPathParamName = "settingsPath"
let nservicebusPathParamName = "nservicebusPath"
let deployArtifactsPathParamName = "deployArtifactsPath"

let webUrlRedirectValueParamName = "webUrlRedirectValue"
let webUrlRedirectParamName = "webUrlRedirect"
let mainWebConfigFilePathParamName = "mainWebConfigFilePath"
let webUrlMainParamName = "webUrlMain"
//

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
Target.create "Deploy Web" (fun _ ->
    let ftpWebPath = retrieveParam ftpWebPathParamName
    let deployArtifactsPath = retrieveParam deployArtifactsPathParamName
    let buildArtifactsPath = retrieveParam buildArtifactsPathParamName
    let settingsPath = retrieveParam settingsPathParamName
    let nservicebusPath = retrieveParam nservicebusPathParamName
    let webUrlRedirectValue = retrieveParam webUrlRedirectValueParamName
    let webUrlRedirect = retrieveParam webUrlRedirectParamName
    let mainWebConfigFilePath = retrieveParam mainWebConfigFilePathParamName
    let ftpMainWebConfigFilePath = retrieveParam ftpMainWebConfigFilePathParamName
    let webUrlMain = retrieveParam webUrlMainParamName
    
    Trace.trace ("-> Web " + ftpWebPath)

    Trace.trace ("-> Clean " + deployArtifactsPath)
    Shell.cleanDir deployArtifactsPath

    Trace.trace ("-> Copy " + buildArtifactsPath)
    Shell.copyDir deployArtifactsPath buildArtifactsPath FileFilter.allFiles

    Trace.trace ("-> Copy " + settingsPath)
    Shell.copyDir deployArtifactsPath settingsPath FileFilter.allFiles

    Trace.trace ("-> Copy " + nservicebusPath)
    Shell.copyDir deployArtifactsPath nservicebusPath FileFilter.allFiles

    Trace.trace ("-> Clean " + ftpWebPath)
    makeFtpAction (fun ftp -> ftp.RemoveFiles(ftpWebPath + "/*").Check())

    Trace.trace ("-> Upload files to " + ftpWebPath)
    makeFtpAction (fun ftp -> ftp.PutFiles(deployArtifactsPath + "\*", ftpWebPath + "/").Check())

    Trace.trace ("-> Call URL " + webUrlRedirect)
    try
        Http.get "" "" webUrlRedirect |> ignore
    with
    | :? System.Net.Http.HttpRequestException as ex ->
        Trace.trace ex.Message

        let isStatusCorrect = ex.Message.Contains("404");
        match isStatusCorrect with
        | true -> ()
        | false -> reraise()

    Trace.trace ("-> Set " + mainWebConfigFilePath)
    let doc = new XmlDocument()
    doc.Load(mainWebConfigFilePath)
    let redirectNode = doc.SelectSingleNode("//action[@type = 'Redirect']")
    redirectNode.Attributes.["url"].Value <- webUrlRedirectValue
    doc.Save(mainWebConfigFilePath)

    Trace.trace ("-> Upload file to " + ftpMainWebConfigFilePath)
    makeFtpAction (fun ftp -> ftp.PutFiles(mainWebConfigFilePath, ftpMainWebConfigFilePath).Check())

    Trace.trace ("-> Call URL " + webUrlMain)
    Http.get "" "" webUrlMain |> ignore

    Trace.trace (sprintf "-> Web %s deployed successfully." ftpWebPath)
)

// start build
Target.runOrDefaultWithArguments "Deploy Web"