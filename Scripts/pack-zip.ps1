param(
	[string] $Project,
	[string] $SolutionDir,
	[string] $TargetDir)

$SolutionDir = $SolutionDir.Trim()
$TargetDir = $TargetDir.Trim()

Remove-Item "$($SolutionDir)\$($Project).qlplugin" -ErrorAction SilentlyContinue
$files = Get-ChildItem -Path "$TargetDir" -Exclude *.pdb,*.xml
Compress-Archive $files "$($SolutionDir)\$($Project).zip"
Move-Item "$($SolutionDir)\$($Project).zip" "$($SolutionDir)\$($Project).qlplugin"
