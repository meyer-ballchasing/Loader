if (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator))
{
  Start-Process powershell.exe "-File",('"{0}"' -f $MyInvocation.MyCommand.Path) -Verb RunAs;
  exit;
}

$url = "https://wslstorestorage.blob.core.windows.net/wslblob/wsl_update_x64.msi";
$outpath = "$PSScriptRoot/wsl_update_x64.msi";
Invoke-WebRequest -Uri $url -OutFile $outpath;

$args = @("/passive");
Start-Process -Filepath $outpath -ArgumentList $args -Wait;

wsl --set-default-version 2