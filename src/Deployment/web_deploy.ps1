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
    [string]$ftpHostName,

    [Parameter(Mandatory=$True)]
    [string]$ftpUserName,

    [Parameter(Mandatory=$True)]
    [string]$ftpPassword,

    [Parameter(Mandatory=$True)]
    [string]$ftpDestinationPath,

    [Parameter(Mandatory=$True)]
    [string]$winscpDllPath,

    [Parameter(Mandatory=$True)]
    [string]$urlToWarmUp,

    [Parameter(Mandatory=$True)]
    [string]$mainWebConfigFilePath,

    [Parameter(Mandatory=$True)]
    [string]$urlRedirect,

    [Parameter(Mandatory=$True)]
    [string]$ftpMainWebConfigDestinationPath,

    [Parameter(Mandatory=$True)]
    [string]$mainUrlToWarmUp
)

$ErrorActionPreference = "Stop"

function prepareArtifactsToDeploy(
    $destination,
    $source,
    $nserviceBusLicenseSourcePath,
    $settingsSourcePath)
{
    Write-Host "->clean $destination directory"
    Remove-Item "$destination\*" -Recurse -Force
    
    Write-Host "->copy artifacts for $source to $destination"
    Copy-Item "$source\*" -Destination "$destination" -Recurse

    Write-Host "->copy NServiceBusLicense"
    Copy-Item "$nserviceBusLicenseSourcePath\*" -Destination "$destination" -Recurse

    Write-Host "->copy settings"
    Copy-Item "$settingsSourcePath\*" -Destination "$destination" -Recurse
}

function ftpCleanDestination($session, $ftpDestinationPath)
{
    Write-Host "->remove files from ftp $ftpDestinationPath"
    
    $removalResult = $session.RemoveFiles("$ftpDestinationPath/*")
    if (!$removalResult.IsSuccess)
    {
        throw "Removing files failed"
    }
}

function ftpCopyFiles($session, $from, $to)
{
    Write-Host "->copy files to ftp $to"
    $session.PutFiles("$from", "$to").Check()
}

function warmUpUri($Uri, $expectedStatusCode)
{
    Write-Host "->invoke $Uri"
        
    try
    {
        Invoke-WebRequest -Uri "$Uri" -Method HEAD
    }
    catch
    {
        $responseStatusCode = $_.exception.response.statuscode.value__
            
        if ($responseStatusCode -ne $expectedStatusCode)
        {
            throw "Deployed web host is broken -> response status code $responseStatusCode"
        }
    }
}

function setMainWebConfig($mainWebConfigFilePath, $urlRedirect)
{
    Write-Host "->set main web.config"
    $doc = New-Object System.Xml.XmlDocument
    $doc.Load($mainWebConfigFilePath)

    $book = $doc.SelectSingleNode("//action[@type = 'Redirect']")
    $book.url = "$urlRedirect"

    $doc.Save($mainWebConfigFilePath)
}

#main

try
{
    prepareArtifactsToDeploy $destination $source $nserviceBusLicenseSourcePath $settingsSourcePath

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

        ftpCleanDestination $session $ftpDestinationPath
        ftpCopyFiles $session "$destination\*" "$ftpDestinationPath/"

        warmUpUri $urlToWarmUp 404

        setMainWebConfig $mainWebConfigFilePath $urlRedirect
        ftpCopyFiles $session $mainWebConfigFilePath $ftpMainWebConfigDestinationPath
        
        warmUpUri $mainUrlToWarmUp 200
    }
    catch
    {
        throw;
    }
    finally
    {
        Write-Host "->dispose ftp session"
        $session.Dispose()
    }
}
catch
{
    throw;
}