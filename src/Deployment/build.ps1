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
    [string]$gitRepositoryUrl,	
	
    [Parameter(Mandatory=$True)]
    [string]$solutionRelativePath,

    [Parameter(Mandatory=$True)]
    [string]$binRelativePath
)

$ErrorActionPreference = "Stop"

try
{
    Write-Host "clean artifacts directory"
    Remove-Item "$buildArtifactsPath\*" -Recurse -Force
    
    Write-Host "clone repository"
    & $gitExePath "clone" "-q" $gitRepositoryUrl $buildArtifactsPath
    
    Write-Host "restore nuget packages"
    & $nugetExePath "restore" "$buildArtifactsPath\$solutionRelativePath"

    Write-Host "build solution"
    $buildLogFile = "$buildArtifactsPath\$binRelativePath\build.log"
    $run = $msbuildExePath
    $p1 = "$buildArtifactsPath\$solutionRelativePath"
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