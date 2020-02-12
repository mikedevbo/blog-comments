#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fake.Net.Http
nuget WinSCP
nuget Fake.Tools.Git
nuget Fake.Api.GitHub
nuget Fake.DotNet.Cli //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet

// outside script parameter names
let projectPathParamName = "projectPath"
let ridParamName = "rid"

let retrieveParam paramName =
    Environment.environVarOrFail paramName

// Targets
Target.create "Publish Artifacts" (fun _ ->
    let projectPath = retrieveParam projectPathParamName
    let rid = retrieveParam ridParamName
    
    Trace.trace ("-> Project " + projectPath)
        
    Trace.trace ("-> Publish " + projectPath)
    DotNet.publish (fun p -> 
        { p with
            Configuration = DotNet.BuildConfiguration.Release
            NoBuild = true
            Runtime = if rid = "empty" then p.Runtime else Some rid
        }) projectPath
    
    Trace.trace "-> Code published successfully."
)

//// Dependencies

// start build
Target.runOrDefaultWithArguments "Publish Artifacts"