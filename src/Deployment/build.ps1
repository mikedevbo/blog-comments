[CmdletBinding()]
Param(

    [Parameter(Mandatory=$True)]
    [string]$gitExePath,
    
    [Parameter(Mandatory=$True)]
    [string]$nugetExePath,

    [Parameter(Mandatory=$True)]
    [string]$msbuildExePath,

    [Parameter(Mandatory=$True)]
    [string]$buildArtifactsPath,

    [Parameter(Mandatory=$True)]
    [string]$solutionPath,

    [Parameter(Mandatory=$True)]
    [string]$binPath
)

$ErrorActionPreference = "Stop"

try
{
    Write-Host "clean artifacts directory"
    Remove-Item "$buildArtifactsPath\*" -Recurse -Force
    
    Write-Host "clone repository"
    & $gitExePath "clone" "-q" "https://github.com/mikedevbo/blog-comments.git" "$buildArtifactsPath"
    
    Write-Host "restore nuget packages"
    & $nugetExePath "restore" "$solutionPath"

    Write-Host "build solution"
    $buildLogFile = "$binPath\build.log"
    $run = $msbuildExePath
    $p1 = $solutionPath
    $p2 = "/p:Configuration=Release"
    $p3 = "/flp:logfile=$buildLogFile"

    & $run $p1 $p2 $p3

    if(!$?)
    {
        throw "Buit failed see $buildLogFile for details"
    }
}
catch
{
    throw;
}