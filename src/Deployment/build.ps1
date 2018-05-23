[CmdletBinding()]
Param(

    [Parameter(Mandatory=$True)]
    [string]$solutionPath,

    [Parameter(Mandatory=$True)]
    [string]$binPath,

    [Parameter(Mandatory=$True)]
    [string]$msbuildPath

)

$ErrorActionPreference = "Stop"

try
{
    Write-Host "clean bin directory"
    Remove-Item "$binPath\*" -Recurse -Force

    Write-Host "build solution"
    $buildLogFile = "$binPath\build.log"
    $run = $msbuildPath
    $p1 = $solutionPath
    $p2 = "/p:Configuration=Release"
    $p3 = "/flp:logfile=$buildLogFile"

    & $run $p1 $p2 $p3

    if(!$?)
    {
        throw "Buit failed see $buildLogFile for details"
    }
}
catch [System.Exception]
{
    throw;
}