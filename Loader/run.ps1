param
(
	[Parameter(Position=0)]
	[string]$action,
	[Parameter(Mandatory=$true)]
	[string]$key
)

$dirName = $(Get-Location).Path.Split('\') | select -Last 1;

$ar = "run -it --volume $(Get-Location):/${dirName} meyer.ballchasing.loader:1.0 ${action} -d /$dirName -key ${key}";

Write-Output "Executing docker with arguments: $ar"

$psi = New-object System.Diagnostics.ProcessStartInfo

$psi.CreateNoWindow = $true
$psi.UseShellExecute = $false
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$psi.FileName = "docker"
$psi.Arguments = $ar
$psi.WorkingDirectory = $(Get-Location).Path
$psi.LoadUserProfile = $true

$process = New-Object System.Diagnostics.Process
$process.StartInfo = $psi

[void]$process.Start()

$output = $process.StandardOutput.ReadToEnd()
$process.WaitForExit()
$output