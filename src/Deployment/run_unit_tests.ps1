[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$NunitExePath,
	
    [Parameter(Mandatory=$True)]
    [string]$binPath
)

$ErrorActionPreference = "Stop"

try
{
    $tests = (Get-ChildItem $binPath -Recurse -Include *unit.tests.dll)

    & $NunitExePath $tests --noheader --work=$binPath
}
catch
{
    throw;
}