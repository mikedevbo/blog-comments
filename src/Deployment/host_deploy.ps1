[CmdletBinding()]
Param(

    [Parameter(Mandatory=$True)]
    [string]$destination,

    [Parameter(Mandatory=$True)]
    [string]$source,

    [Parameter(Mandatory=$True)]
    [string]$settingsSourcePath,

    [Parameter(Mandatory=$True)]
    [string]$connectionstringsSourcePath,

    [Parameter(Mandatory=$True)]
    [string]$previousWindowsServiceName,
    
    [Parameter(Mandatory=$True)]
    [string]$newWindowsServiceName,

    [Parameter(Mandatory=$True)]
    [string]$newWindowsServiceDescription,

    [Parameter(Mandatory=$True)]
    [string]$windowsServiceBinPath
)

$ErrorActionPreference = "Stop"

function prepareArtifactsToDeploy(
    $destination,
    $source,
    $settingsSourcePath,
    $connectionstringsSourcePath)
{
    Write-Host "->clean $destination directory"
    Remove-Item "$destination\*" -Recurse -Force
    
    Write-Host "->copy artifacts for $source to $destination"
    Copy-Item "$source\*" -Destination "$destination" -Recurse

    Write-Host "->copy settings"
    Copy-Item "$settingsSourcePath\*" -Destination "$destination" -Recurse

    Write-Host "->copy connectionstrings"
    Copy-Item "$connectionstringsSourcePath\*" -Destination "$destination" -Recurse
}

function windowsServiceExists($serviceName)
{
    $service = Get-Service $serviceName -ErrorAction SilentlyContinue

    if ($service)
    {
        return $True;
    }

    return $False
}

function stopWindowService($serviceName)
{
    Write-Host "stop windows service $serviceName"
    $service = Get-Service $serviceName -ErrorAction SilentlyContinue
    
    if (!$service)
    {
        return;
    }

    if (!($service.Status -eq 'Running'))
    {
        return;
    }

    Stop-Service $serviceName
}

#main

try
{
    prepareArtifactsToDeploy $destination $source $settingsSourcePath $connectionstringsSourcePath
    stopWindowService $previousWindowsServiceName

    if (!(windowsServiceExists $newWindowsServiceName))
    {
        Write-Host "create windows service $newWindowsServiceName"
        sc.exe create $newWindowsServiceName start= demand binpath= "$windowsServiceBinPath"
        sc.exe description $newWindowsServiceName $newWindowsServiceDescription
        sc.exe failure $newWindowsServiceName reset= 3600 actions= restart/5000/restart/10000/restart/60000
    }

    Write-Host "start windows service $newWindowsServiceName"
    Start-Service $newWindowsServiceName
}
catch
{
    throw;
}