##############################################################################
##
## New-SelfSignedCertificate
##
## From Windows PowerShell Cookbook (O'Reilly)
## by Lee Holmes (http://www.leeholmes.com/guide)
##
##############################################################################

<#

.SYNOPSIS

Generate a new self-signed certificate. The certificate generated by these
commands allow you to sign scripts on your own computer for protection
from tampering. Files signed with this signature are not valid on other
computers.

.EXAMPLE

New-SelfSignedCertificate.ps1
Creates a new self-signed certificate

#>

Set-StrictMode -Version Latest

## Ensure we can find makecert.exe
if(-not (Get-Command makecert.exe -ErrorAction SilentlyContinue))
{
    $errorMessage = "Could not find makecert.exe. " +
        "This tool is available as part of Visual Studio, or the Windows SDK."

    Write-Error $errorMessage
    return
}

$keyPath = Join-Path ([IO.Path]::GetTempPath()) "root.pvk"

## Generate the local certification authority
makecert -n "CN=PowerShell Local Certificate Root" -a sha1 `
    -eku 1.3.6.1.5.5.7.3.3 -r -sv $keyPath root.cer `
    -ss Root -sr localMachine

## Use the local certification authority to generate a self-signed
## certificate
makecert -pe -n "CN=PowerShell User" -ss MY -a sha1 `
    -eku 1.3.6.1.5.5.7.3.3 -iv $keyPath -ic root.cer

## Remove the private key from the filesystem.
Remove-Item $keyPath

## Retrieve the certificate
Get-ChildItem cert:\currentuser\my -codesign |
    Where-Object { $_.Subject -match "PowerShell User" }
