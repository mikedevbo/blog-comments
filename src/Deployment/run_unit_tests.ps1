[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$binPath,

    [Parameter(Mandatory=$True)]
    [string]$NunitExePath
)

$ErrorActionPreference = "Stop"

#main

try
{
    $tests = (Get-ChildItem $binPath -Recurse -Include *unit.tests.dll)

    & $NunitExePath $tests --noheader --work=$binPath
}
catch
{
    throw;
}