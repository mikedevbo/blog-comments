[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$dotnetExePath,
	
    [Parameter(Mandatory=$True)]
    [string]$projectPath,

    [string]$rid
)

$ErrorActionPreference = "Stop"

try
{
	if ($rid -eq "")
	{
		Write-Host "publish without RID"
		& $dotnetExePath publish $projectPath --configuration Release
	}
	else
	{
		Write-Host "publish with RID: $rid"
		& $dotnetExePath publish $projectPath --configuration Release --runtime $rid
	}
	
    if(!$?)
    {
        throw "Publish failed."
    }	
}
catch
{
    throw;
}