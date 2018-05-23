[CmdletBinding()]
Param(

    [Parameter(Mandatory=$True)]
    [string]$destination,

    [Parameter(Mandatory=$True)]
    [string]$source,

    [Parameter(Mandatory=$True)]
    [string]$nserviceBusLicenseSourcePath,

    [Parameter(Mandatory=$True)]
    [string]$settingsSourcePath,

    [Parameter(Mandatory=$True)]
    [string]$connectionstringsSourcePath,

    [Parameter(Mandatory=$True)]
    [string]$ftpHostName,

    [Parameter(Mandatory=$True)]
    [string]$ftpUserName,

    [Parameter(Mandatory=$True)]
    [string]$ftpPassword,

    [Parameter(Mandatory=$True)]
    [string]$ftpDestinationPath,

    [Parameter(Mandatory=$True)]
    [string]$winscpDllPath
)

$ErrorActionPreference = "Stop"

try
{
    Write-Host "->clean $destination directory"
    Remove-Item "$destination\*" -Recurse -Force
    
    Write-Host "->copy artifacts for $source to $destination"
    Copy-Item "$source" -Destination "$destination\bin" -Recurse
    
    Write-Host "->create app_data directory"
    New-Item -ItemType directory -Path "$destination\app_data"

    Write-Host "->copy NServiceBusLicense"
    Copy-Item "$nserviceBusLicenseSourcePath" -Destination "$destination\nservicebus" -Recurse

    Write-Host "->copy settings"
    Copy-Item "$settingsSourcePath\*" -Destination "$destination" -Recurse

    Write-Host "->copy connectionstrings"
    Copy-Item "$connectionstringsSourcePath\*" -Destination "$destination" -Recurse


    Add-Type -Path "$winscpDllPath"

    $sessionOptions = New-Object WinSCP.SessionOptions -Property @{
        Protocol = [WinSCP.Protocol]::Ftp
        HostName = "$ftpHostName"
        UserName = "$ftpUserName"
        Password = "$ftpPassword"
    }

    $session = New-Object WinSCP.Session
    try
    {
        Write-Host "->open ftp session"
        $session.Open($sessionOptions)
        
        Write-Host "->remove files from ftp $ftpDestinationPath"
        $removalResult = $session.RemoveFiles("$ftpDestinationPath/*")
        if (!$removalResult.IsSuccess)
        {
            throw "Removing files failed"
        }

        Write-Host "->copy files to ftp $ftpDestinationPath"
        $session.PutFiles("$destination\*", "$ftpDestinationPath/").Check()
    }
    catch [System.Exception]
    {
        throw;
    }
    finally
    {
        Write-Host "->dispose ftp session"
        $session.Dispose()
    }
}
catch [System.Exception]
{
    throw;
}