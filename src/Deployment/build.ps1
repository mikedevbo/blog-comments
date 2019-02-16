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

function cloneRepository(
    $git,
	$repository,
	$destination)
{
    #Write-Host "clean artifacts directory"
    #Remove-Item "$destination\*" -Recurse -Force
    
    #Write-Host "clone repository"
    #& $git "clone" "-q" $repository $destination
}

function buildSolution(
	$dotnet,
	$destination)
{
	& $dotnet build $destination --configuration Release
	if ($lastexitcode -ne 0)
	{
		throw $errorMessage
	}
}

function runUnitTests(
	$dotnet,
	$destination)
{
	& $dotnet test $destination --no-build --configuration Release
	if ($lastexitcode -ne 0)
	{
		throw $errorMessage
	}
}

function publishrtifacts(
	$dotnet,
	$destination)
{
	& $dotnet publish $destination --no-build --configuration Release
	if ($lastexitcode -ne 0)
	{
		throw $errorMessage
	}
}

#main

try
{
    ##Write-Host "clean artifacts directory"
    ##Remove-Item "$buildArtifactsPath\*" -Recurse -Force
    
    ##Write-Host "clone repository"
    ##& $gitExePath "clone" "-q" $gitRepositoryUrl $buildArtifactsPath
	
	#cloneRepository $buildArtifactsPath $gitRepositoryUrl $gitExePath
 
    Write-Host "start build"
	
	$path = "$buildArtifactsPath\$solutionRelativePath"

	buildSolution $dotnetExePath $path
	runUnitTests $dotnetExePath $path
	publishrtifacts $dotnetExePath $path

    if(!$?)
    {
        throw $errorMessage
    }
}
catch
{
    throw;
}