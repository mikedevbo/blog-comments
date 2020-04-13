[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$dotnetExePath,
	
    [Parameter(Mandatory=$True)]
    [string]$solutionPath
)

$ErrorActionPreference = "Stop"

try
{
    & $dotnetExePath test $solutionPath --no-build --configuration Release
	
    if(!$?)
    {
        throw "Unit tests failed."
    }	
}
catch
{
    throw;
}