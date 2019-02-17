[CmdletBinding()]
Param(

    [Parameter(Mandatory=$True)]
    [string]$gitExePath,

    [Parameter(Mandatory=$True)]
    [string]$dotnetExePath,

    [Parameter(Mandatory=$True)]
    [string]$buildArtifactsPath,

    [Parameter(Mandatory=$True)]
    [string]$gitRepositoryUrl,	
	
    [Parameter(Mandatory=$True)]
    [string]$solutionRelativePath
)

$ErrorActionPreference = "Stop"

#main

try
{
    #Write-Host "clean artifacts directory"
    #Remove-Item "$buildArtifactsPath\*" -Recurse -Force
    
    #Write-Host "clone repository"
    #& $gitExePath "clone" "-q" $gitRepositoryUrl $buildArtifactsPath
 
    Write-Host "build solution"
    $buildLogFile = "$buildArtifactsPath\$solutionRelativePath\bin\build.log"
	
	$path = "$buildArtifactsPath\$solutionRelativePath"

	& $dotnetExePath build $path --configuration Release "/flp:logfile=$buildLogFile"

    if(!$?)
    {
        throw "Build failed see $buildLogFile for details"
    }
}
catch
{
    throw;
}