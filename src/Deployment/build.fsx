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
open Fake.IO
open Fake.Tools.Git
open Fake.DotNet

// outside script parameter names
let buildArtifactsWorkingDirectoryPathParamName = "buildArtifactsWorkingDirectoryPath"
let gitRepositoryUrlParamName = "gitRepositoryUrl"
let buildArtifactsSubDirectoryNameParamName = "buildArtifactsSubDirectoryName"
let solutionRelativePathParamName = "solutionRelativePath"


let retrieveParam paramName =
    Environment.environVarOrFail paramName

// Targets
Target.create "Compile Code" (fun _ ->
    let buildArtifactsWorkingDirectoryPath = retrieveParam buildArtifactsWorkingDirectoryPathParamName
    let gitRepositoryUrl = retrieveParam gitRepositoryUrlParamName
    let buildArtifactsSubDirectoryName = retrieveParam buildArtifactsSubDirectoryNameParamName
    let solutionRelativePath = retrieveParam solutionRelativePathParamName
    
    let buildArtifactsPath = buildArtifactsWorkingDirectoryPath + "\\" + buildArtifactsSubDirectoryName
    let slnPath = buildArtifactsPath + "\\" + solutionRelativePath

    Trace.trace ("-> Solution " + slnPath)
    
    Trace.trace ("-> Clean " + buildArtifactsPath)
    Shell.cleanDir buildArtifactsPath

    Trace.trace ("-> Clone " + gitRepositoryUrl)
    Repository.clone buildArtifactsWorkingDirectoryPath gitRepositoryUrl buildArtifactsSubDirectoryName
    
    Trace.trace ("-> Build " + slnPath)
    DotNet.build (fun p -> { p with Configuration = DotNet.BuildConfiguration.Release }) slnPath
    
    Trace.trace "-> Code built successfully."
)

Target.create "Run Unit Tests" (fun _ ->
    let buildArtifactsWorkingDirectoryPath = retrieveParam buildArtifactsWorkingDirectoryPathParamName
    let buildArtifactsSubDirectoryName = retrieveParam buildArtifactsSubDirectoryNameParamName
    let solutionRelativePath = retrieveParam solutionRelativePathParamName

    let buildArtifactsPath = buildArtifactsWorkingDirectoryPath + "\\" + buildArtifactsSubDirectoryName
    let slnPath = buildArtifactsPath + "\\" + solutionRelativePath

    Trace.trace ("-> Solution " + slnPath)

    DotNet.test (fun p -> 
        { p with
            NoBuild = true
            Configuration = DotNet.BuildConfiguration.Release
        }) slnPath

    Trace.trace "-> Unit Tests run successfully."
)

//// Dependencies
open Fake.Core.TargetOperators

"Compile Code"
    ==> "Run Unit Tests"

// start build
Target.runOrDefaultWithArguments "Run Unit Tests"