#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fake.Net.Http
nuget WinSCP
nuget System.ServiceProcess.ServiceController //"
#load "./.fake/host_ws_deploy.fsx/intellisense.fsx"

open Fake.Core
open System.ServiceProcess
open Fake.IO

// outsite script parameter names
let deployEndpointPathParamName = "deployEndpointPath"

let buildArtifactsPathParamName = "buildArtifactsPath"

let settingsPathParamName = "settingsPath"
let previousWindowsServiceNameParamName = "previousWindowsServiceName"
let newWindowsServiceNameParamName = "newWindowsServiceName"
let newWindowsServiceDescriptionParamName = "newWindowsServiceDescription"
let newWindowsServiceBinPathParamName = "newWindowsServiceBinPath"
//

let retrieveParam paramName =
    Environment.environVarOrFail paramName

// Targets
Target.create "Deploy Endpoint" (fun _ ->
    let deployEndpointPath = retrieveParam deployEndpointPathParamName
    let buildArtifactsPath = retrieveParam buildArtifactsPathParamName
    let settingsPath = retrieveParam settingsPathParamName
    let previousWindowsServiceName = retrieveParam previousWindowsServiceNameParamName
    let newWindowsServiceName = retrieveParam newWindowsServiceNameParamName
    let newWindowsServiceBinPath = retrieveParam newWindowsServiceBinPathParamName
    let newWindowsServiceDescription = retrieveParam newWindowsServiceDescriptionParamName
    
    Trace.trace ("-> Endpoint " + deployEndpointPath)

    Trace.trace ("-> Clean " + deployEndpointPath)
    Shell.cleanDir deployEndpointPath

    Trace.trace ("-> Copy " + buildArtifactsPath)
    Shell.copyDir deployEndpointPath buildArtifactsPath FileFilter.allFiles

    Trace.trace ("-> Copy " + settingsPath)
    Shell.copyDir deployEndpointPath settingsPath FileFilter.allFiles

    Trace.trace ("-> Stop Windows Service " + previousWindowsServiceName)
    try
        let sc = new ServiceController(previousWindowsServiceName);
        if sc.CanStop then sc.Stop()
    with
    | :? System.InvalidOperationException as ex ->
        Trace.trace ("-> Windows Service " + previousWindowsServiceName + "doesn't exist.")
        Trace.trace ex.Message
    
    Trace.trace ("-> Create and start Windows Service " + newWindowsServiceName)
    let sc = new ServiceController(newWindowsServiceName);
    try
        if sc.Status = ServiceControllerStatus.Stopped
        then
            Trace.trace ("-> Start Windows Service " + newWindowsServiceName)
            sc.Start()
    with
    | :? System.InvalidOperationException as ex ->
        Trace.trace ex.Message
        Trace.trace ("-> Create Windows Service " + newWindowsServiceName)
        CreateProcess.fromRawCommandLine "sc.exe" (sprintf "create %s start= demand binpath= %s " newWindowsServiceName newWindowsServiceBinPath)
        |> Proc.run
        |> ignore

        CreateProcess.fromRawCommandLine "sc.exe" (sprintf "description %s \"%s\" " newWindowsServiceName newWindowsServiceDescription)
        |> Proc.run
        |> ignore

        CreateProcess.fromRawCommandLine "sc.exe" (sprintf "failure %s reset= 3600 actions= restart/5000/restart/10000/restart/60000" newWindowsServiceName)
        |> Proc.run
        |> ignore

        Trace.trace ("-> Start Windows Service " + newWindowsServiceName)
        sc.Start()

    Trace.trace (sprintf "-> Endpoint %s deployed successfully." deployEndpointPath)
)

// start build
Target.runOrDefaultWithArguments "Deploy Endpoint"